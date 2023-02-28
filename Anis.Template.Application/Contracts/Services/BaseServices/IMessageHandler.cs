using Anis.Template.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anis.Template.Application.Contracts.Services.BaseServices
{
    public interface IMessageHandler
    {
        Task<bool> HandleAsync<T>(MessageBody<T> message);

    }
}
