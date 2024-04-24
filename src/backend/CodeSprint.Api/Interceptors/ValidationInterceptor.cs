using CodeSprint.Common.Exceptions;
using FluentValidation;
using Grpc.Core.Interceptors;
using Grpc.Core;

namespace CodeSprint.Api.Interceptors;

public class ValidationInterceptor : Interceptor
{
    private readonly ILogger<ValidationInterceptor> _logger;

    public ValidationInterceptor(ILogger<ValidationInterceptor> logger)
    {
        _logger = logger;
    }

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
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[GRPC][ValidationInterceptor][{GrpcRequestType}] Exception was thrown!", typeof(TRequest));

            if (ex is RpcException)
                throw;

            throw new RpcException(new Status(StatusCode.Internal, "An internal error occurred."));
        }

        return base.UnaryServerHandler(request, context, continuation);
    }
}