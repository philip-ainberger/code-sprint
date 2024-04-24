using CodeSprint.Commom.Exceptions;
using FluentValidation;
using Grpc.Core.Interceptors;
using Grpc.Core;

namespace CodeSprint.Api.Interceptors;

public class ValidationInterceptor : Interceptor
{
    public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            var validator = context.GetHttpContext().RequestServices.GetService<IValidator<TRequest>>();

            if (validator == null)
            {
                throw new MissingValidatorException<TRequest>();
            }

            var validation = validator.Validate(request);

            if (!validation.IsValid)
            {
                var metadata = new Metadata();
                foreach (var error in validation.Errors)
                {
                    metadata.Add("error-detail", error.ErrorMessage);
                }

                throw new RpcException(new Status(StatusCode.InvalidArgument, "Validation failed"), metadata);
            }

            // TODO log debug
        }
        catch (RpcException rpcException)
        {
            // TODO log
            throw;
        }
        catch (Exception ex)
        {
            // TODO log
            throw new RpcException(new Status(StatusCode.Internal, "An internal error occurred."));
        }

        return base.UnaryServerHandler(request, context, continuation);
    }
}