using Anis.Template.Application.Contracts.Services.BaseService;
using Anis.Template.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Anis.Template.Infra.Services.BaseService
{
    public class RebuildManager : IRebuildManager
    {
        private readonly IServiceProvider _provider;

        public RebuildManager(IServiceProvider provider)
        {
            _provider = provider;
        }


    }
}
