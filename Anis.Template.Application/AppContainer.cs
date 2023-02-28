using Microsoft.Extensions.DependencyInjection;
using MediatR;
using System.Reflection;

namespace Anis.Template.Application
{
    public static class AppContainer
    {
        public static IServiceCollection RegisterAppServices(this IServiceCollection services)
        {

            services.AddMediatR(Assembly.GetExecutingAssembly());
            return services;
        }

    }
}
