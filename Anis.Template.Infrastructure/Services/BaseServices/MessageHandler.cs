using Anis.Template.Application.Contracts.Services.BaseServices;
using Anis.Template.Domain.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace Anis.Template.Infrastructure.Services.BaseServices
{
    public class MessageHandler : IMessageHandler
    {
        private readonly IServiceProvider _provider;

        public MessageHandler(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task<bool> HandleAsync<T>(MessageBody<T> message)
        {
            using var scope = _provider.CreateScope();

            var medaiator = scope.ServiceProvider.GetRequiredService<IMediator>();

            return await medaiator.Send(message);
        }
    }
}
