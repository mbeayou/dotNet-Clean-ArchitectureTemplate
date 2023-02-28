using Anis.Template.Application.Contracts.Services.BaseServices;
using Anis.Template.Domain.Models;
using Anis.Template.Infrastructure.Services.Const;
using Azure.Messaging.ServiceBus;
using Anis.Template.Domain.Events.DataTypes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using Serilog.Core.Enrichers;
using System.Text;
using Newtonsoft.Json;

namespace Infrastructure.MessageBus.Listener
{
    public class EcomCardListener : IHostedService
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<EcomCardListener> _logger;
        private readonly ServiceBusSessionProcessor _processor;
        private readonly ServiceBusProcessor _deadLetterProcessor;

        public EcomCardListener(
            IServiceProvider provider,
            ILogger<EcomCardListener> logger,
            ServiceBusClient client,
            IConfiguration configuration)
        {
            _provider = provider;
            _logger = logger;

            var maxSessions = configuration.GetValue<int>("ServiceBus:MaxSessions");
            var prefetchCount = configuration.GetValue<int>("ServiceBus:PrefetchCount");
            var maxConcurrentCallsPerSession = configuration.GetValue<int>("ServiceBus:MaxConcurrentCallsPerSession");

            _processor = client.CreateSessionProcessor
            (configuration["ServiceBus:EcomCards"], configuration["ServiceBus:ServiceSubscription"],
                new ServiceBusSessionProcessorOptions()
                {
                    PrefetchCount = prefetchCount,
                    MaxConcurrentCallsPerSession = maxConcurrentCallsPerSession,
                    MaxConcurrentSessions = maxSessions,
                    AutoCompleteMessages = false,
                });

            _processor.ProcessMessageAsync += Processor_ProcessMessageAsync;
            _processor.ProcessErrorAsync += Processor_ProcessErrorAsync;

            _deadLetterProcessor = client.CreateProcessor(configuration["ServiceBus:EcomCards"],
                configuration["ServiceBus:ServiceSubscription"] + "/$DeadLetterQueue", new ServiceBusProcessorOptions()
                {
                    PrefetchCount = 1,
                    AutoCompleteMessages = false,
                    MaxConcurrentCalls = 1,
                    SubQueue = SubQueue.DeadLetter,
                });

            _deadLetterProcessor.ProcessMessageAsync += DeadLetterProcessor_ProcessMessageAsync;
            _deadLetterProcessor.ProcessErrorAsync += DeadLetterProcessor_ProcessErrorAsync;
        }

        private async Task Processor_ProcessMessageAsync(ProcessSessionMessageEventArgs arg)
        {
            Task<bool> isHandledTask = HandelSubject(arg.Message.Subject, arg.Message);


            var isHandled = await isHandledTask;

            if (isHandled)
            {
                await arg.CompleteMessageAsync(arg.Message);
            }
        }


        private async Task DeadLetterProcessor_ProcessMessageAsync(ProcessMessageEventArgs arg)
        {
            Task<bool> isHandledTask = HandelSubject(arg.Message.Subject, arg.Message);

            var isHandled = await isHandledTask;

            if (isHandled)
                await arg.CompleteMessageAsync(arg.Message);
            else
                await arg.AbandonMessageAsync(arg.Message);
        }

        private Task Processor_ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            _logger.LogCritical(arg.Exception,
                "EcomCardListener => _processor => Processor_ProcessErrorAsync Message handler encountered an exception," +
                " Error Source:{ErrorSource}," +
                " Entity Path:{EntityPath}",
                arg.ErrorSource.ToString(),
                arg.EntityPath
            );

            return Task.CompletedTask;
        }

        private Task DeadLetterProcessor_ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            _logger.LogCritical(arg.Exception, "DeadLetter Message handler encountered an exception," +
                                               " Error Source:{ErrorSource}," +
                                               " Entity Path:{EntityPath}",
                arg.ErrorSource.ToString(),
                arg.EntityPath
            );

            return Task.CompletedTask;
        }

        private Task<bool> HandelSubject(string subject, ServiceBusReceivedMessage message)
        {
            message.ApplicationProperties.TryGetValue("Version", out var version);

            return (subject, version.ToString()) switch
            {

                //TODO: Assign Event type consts with event type data
                //e.g : 
                //(EventType.CardCreated, "1") => HandleAsync<CardCreatedData>(message),

                _ => Task.FromResult(true),
            };
        }


        private async Task<bool> HandleAsync<T>(ServiceBusReceivedMessage message)
        {
            bool isHandled;

            var eventType = new PropertyEnricher(name: "EventType", message.Subject);
            var sessionId = new PropertyEnricher(name: "SessionId", message.SessionId);
            var messageId = new PropertyEnricher(name: "MessageId", message.MessageId);

            using (LogContext.Push(eventType, sessionId, messageId))
            {
                _logger.LogInformation("Event handling started.");

                var json = Encoding.UTF8.GetString(message.Body);

                var body = JsonConvert.DeserializeObject<MessageBody<T>>(json);

                using var scope = _provider.CreateScope();

                var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler>();

                isHandled = await handler.HandleAsync(body);

                _logger.LogInformation("Event handling completed, Result: {Result}", isHandled);
            }

            return isHandled;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _processor.StartProcessingAsync(cancellationToken);
            _deadLetterProcessor.StartProcessingAsync(cancellationToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _processor.CloseAsync(cancellationToken);
            _deadLetterProcessor.CloseAsync(cancellationToken);

            return Task.CompletedTask;
        }
    }
}