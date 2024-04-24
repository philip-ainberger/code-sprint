using CodeSprint.Common.Exceptions;
using Grpc.Core.Interceptors;
using Grpc.Core;

namespace CodeSprint.Api.Interceptors;

public class ExceptionHandlingInterceptor(ILogger<ExceptionHandlingInterceptor> logger) : Interceptor
{
    private readonly ILogger<ExceptionHandlingInterceptor> _logger = logger;

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
            _logger.LogInformation(customException, "[GRPC][ExceptionHandlingInterceptor][{GrpcRequestType}] Requested entity not found.", typeof(TRequest));
            throw new RpcException(new Status(StatusCode.NotFound, customException.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[GRPC][ExceptionHandlingInterceptor][{GrpcRequestType}] Exception was thrown!", typeof(TRequest));

            if (ex is RpcException)
                throw;

            throw new RpcException(new Status(StatusCode.Unknown, "An unexpected error occurred."));
        }
    }
}