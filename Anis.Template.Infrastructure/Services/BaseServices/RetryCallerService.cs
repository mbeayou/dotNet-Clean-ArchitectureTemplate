using Anis.Template.Application.Contracts.Repositories;
using Anis.Template.Application.Contracts.Services.BaseService;
using Anis.Template.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Anis.Template.Infra.Services.BaseService
{
    public class RetryCallerService : IRetryCallerService
    {
        private readonly ILogger<RetryCallerService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public RetryCallerService(ILogger<RetryCallerService> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<T> CallAsync<T>(Func<Task<T>> operation, int retryCount = 5, int millisecondsDelay = 250)
        {
            var count = retryCount + 1;

            while (true)
            {
                count--;

                try
                {
                    return await operation();
                }
                catch (DbUpdateException e)
                {
                    _logger.LogWarning(e, "Call failed with {attempts} left", count);

                    if (count == 0)
                        throw new AppException(ExceptionStatusCode.Aborted, e.Message);

                }

                await Task.Delay(millisecondsDelay);
            }
        }

    }
}
