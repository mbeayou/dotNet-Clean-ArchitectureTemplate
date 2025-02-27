﻿using Grpc.Core;
using Grpc.Core.Interceptors;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Anis.Template.Grpc.Interceptors
{
    public class ThreadCultureInterceptor : Interceptor
    {
        public async override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            var headerLanguage = context.RequestHeaders.FirstOrDefault(t => t.Key == "language");

            if (headerLanguage != null)
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(headerLanguage.Value);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(headerLanguage.Value);
            }

            return await continuation(request, context);
        }
    }
}
