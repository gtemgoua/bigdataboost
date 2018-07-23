using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BigDataBoost.Data.HostedServices
{
    public class RealTimeUpdaterTask
    {

        public RealTimeUpdaterTask()
        {
            //Constructor’s parameters validations...      
        }

        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Update real time values
            BigDataBoostDbInitializer.GenerateRunTimeData(Model.TagStatus.Good);

            await Task.Delay(1000, stoppingToken);

        }
    }





    public class RealTimeUpdaterService : BackgroundService
    {
        private readonly RealTimeUpdaterTask _dbUpdater;
        public RealTimeUpdaterService(RealTimeUpdaterTask dbUpdater)
        {
            _dbUpdater = dbUpdater;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await _dbUpdater.ExecuteAsync(cancellationToken);

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}