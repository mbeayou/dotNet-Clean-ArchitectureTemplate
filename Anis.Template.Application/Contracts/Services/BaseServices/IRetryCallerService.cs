using System;
using System.Threading.Tasks;

namespace Anis.Template.Application.Contracts.Services.BaseService
{
    public interface IRetryCallerService
    {
        Task<T> CallAsync<T>(Func<Task<T>> operation, int retryCount = 5, int millisecondsDelay = 250);
    }
}
