using Anis.Template.Application.Contracts.Services.BaseServices;
using Anis.Template.Grpc.Protos.Rebuild;
using Anis.Template.Grpc.Extensions.Rebuild;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Anis.Template.Grpc.Protos.Client;

namespace Anis.Template.Grpc.Services
{
    public class RebuildService : Rebuild.RebuildBase
    {
        private readonly EventsHistory.EventsHistoryClient _client;
        private readonly ILogger<RebuildService> _logger;

        public RebuildService(
            EventsHistory.EventsHistoryClient client,
            ILogger<RebuildService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async override Task<Empty> Rebuild(RebuildRequest request, ServerCallContext context)
        {
            if (request.Page < 1 || request.Size < 1)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Page and size should be greater than zero."));

            _logger.LogInformation("start rebuild excute ");

            for (var i = request.Page; i > 0; i++)
            {
                //TODO: fetch messages to rebuild
            }

            return new Empty();
        }

        private async Task HandelResponseAsync(Response response, int page)
        {
            _logger.LogWarning("start handle messages, count {0}, page {1}", response.Messages.Count, page);

            //TODO: put your handler method for each message you fetched

            _logger.LogWarning("end handel messages");

        }

    }
}
