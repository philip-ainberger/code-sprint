using CodeSprint.Commom.Exceptions;
using Grpc.Core.Interceptors;
using Grpc.Core;

namespace CodeSprint.Api.Interceptors;

public class ExceptionHandlingInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (EntityNotFoundException customException)
        {
            throw new RpcException(new Status(StatusCode.NotFound, customException.Message));
        }
        catch (RpcException rpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            // TODO LOG

            throw new RpcException(new Status(StatusCode.Unknown, "An unexpected error occurred."));
        }
    }
}