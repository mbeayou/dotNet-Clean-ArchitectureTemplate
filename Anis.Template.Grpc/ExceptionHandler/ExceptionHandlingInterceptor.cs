using Anis.Template.Domain.Exceptions;
using Anis.Template.Grpc.Extensions;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Anis.Template.Grpc.ExceptionHandler
{
    public class ExceptionHandlingInterceptor : Interceptor
    {
        private readonly ILogger<ExceptionHandlingInterceptor> _logger;

        public ExceptionHandlingInterceptor(ILogger<ExceptionHandlingInterceptor> logger)
        {
            _logger = logger;
        }

        public async override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return await continuation(request, context);
            }
            catch (Exception e)
            {
                switch (e)
                {
                    case AppException appException:
                        throw new RpcException(new Status(appException.StatusCode.ToRpcStatuseCode(), appException.Message));

                    default:
                        _logger.LogError(e, $"An error occured when calling {context.Method}");
                        throw new RpcException(new Status(StatusCode.Unknown, e.Message));
                }
            }
        }
    }
}
