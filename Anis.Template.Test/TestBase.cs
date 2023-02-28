﻿using Anis.Template.Grpc;
using Anis.Template.Infrastructure.Persistence;
using Anis.Template.Test;
using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;
using Xunit.Abstractions;

[assembly: CollectionBehavior(DisableTestParallelization = TestBase.UseSqlDataBase)]
namespace Anis.Template.Test
{
    public abstract class TestBase
    {
        public const bool UseSqlDataBase = true;
        private GrpcChannel _channel;
        private TestWebApplicationFactory<Program> _factory;

        protected TestBase(ITestOutputHelper output)
        {
            Output = output;
        }

        public ITestOutputHelper Output { get; }

        public GrpcChannel Channel
        {
            get
            {
                if (_channel != null)
                    return _channel;

                Initialize();
                return _channel ?? throw new Exception("return _channel");
            }

            private set => _channel = value;
        }
        protected TestWebApplicationFactory<Program> Factory
        {
            get
            {
                if (_factory != null)
                    return _factory;

                Initialize();
                return _factory ?? throw new Exception("return _factory");
            }

            private set => _factory = value;
        }

        public void Initialize(
            Action<IServiceCollection> configureTopic = null,
            Action<IServiceCollection> configureOther = null
        )
        {
            if (configureTopic == null)
                configureTopic = services =>
                {
                    // Add Mock objects here .
                };

            void Configure(IServiceCollection services)
            {
                configureTopic?.Invoke(services);
                configureOther?.Invoke(services);
            };

            var factory = new TestWebApplicationFactory<Program>(Output, Configure);

            var client = factory.CreateClient();

            Channel = GrpcChannel.ForAddress(client.BaseAddress ?? throw new Exception(), new GrpcChannelOptions()
            {
                HttpClient = client,
            });

            Factory = factory;

            ResetDb();
        }

        private void ResetDb()
        {
#pragma warning disable CS8520 // The given expression always matches the provided constant.
            if (UseSqlDataBase is false)
                return;

            using var scope = Factory.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Database.Migrate();

            context.Cards.RemoveRange(context.Cards);

            context.SaveChanges();
#pragma warning restore CS8520 // The given expression always matches the provided constant.
        }

    }

}
