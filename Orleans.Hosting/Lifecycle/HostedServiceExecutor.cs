using Microsoft.Extensions.Logging;
using Orleans.Hosting.Internal;
using System;
using System.Collections.Generic;

namespace Orleans.Hosting
{
    public class HostedServiceExecutor
    {
        private readonly IEnumerable<IHostedService> _services;
        private readonly ILogger<HostedServiceExecutor> _logger;

        public HostedServiceExecutor(ILogger<HostedServiceExecutor> logger, IEnumerable<IHostedService> services)
        {
            _logger = logger;
            _services = services;
        }

        public void Start()
        {
            try
            {
                Console.WriteLine("Information about configured hosted services:");

                Execute(service =>
                {
                    Console.WriteLine(service.ToString());
                    service.Start();
                });
            }
            catch (Exception ex)
            {
                _logger.ApplicationError(LoggerEventIds.HostedServiceStartException, "An error occurred starting the application", ex);
            }
        }

        public void Stop()
        {
            try
            {
                Execute(service => service.Stop());
            }
            catch (Exception ex)
            {
                _logger.ApplicationError(LoggerEventIds.HostedServiceStopException, "An error occurred stopping the application", ex);
            }
        }

        private void Execute(Action<IHostedService> callback)
        {
            List<Exception> exceptions = null;

            foreach (var service in _services)
            {
                try
                {
                    callback(service);
                }
                catch (Exception ex)
                {
                    if (exceptions == null)
                    {
                        exceptions = new List<Exception>();
                    }

                    exceptions.Add(ex);
                }
            }

            // Throw an aggregate exception if there were any exceptions
            if (exceptions != null)
            {
                throw new AggregateException(exceptions);
            }
        }
    }
}