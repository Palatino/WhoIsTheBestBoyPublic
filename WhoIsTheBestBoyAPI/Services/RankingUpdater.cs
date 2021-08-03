using AutoMapper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WhoIsTheBestBoyAPI.Data;
using WhoIsTheBestBoyAPI.Data.Repositories;
using WhoIsTheBestBoyAPI.Data.Repositories.IRepositories;

namespace WhoIsTheBestBoyAPI.Services
{
    /// <summary>
    /// This class will check the ranking every 24 hours and modify the properties of the top ten dogs
    /// </summary>
    public class RankingUpdater : IHostedService, IDisposable
    {
        private Timer _timer;
        private IServiceProvider _serviceProvider;


        public RankingUpdater(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromHours(24));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {

            using (var scope = _serviceProvider.CreateScope())
            {
                BestBoyDbContext _context = scope.ServiceProvider.GetService<BestBoyDbContext>();

                if (_context.Database.GetPendingMigrations().Count() > 0)
                {
                    return;
                }

                var ranking = _context.Dogs.Where(d => d.Approved).OrderByDescending(d => d.Score).Take(10).ToList();

                if (ranking.Count == 0)
                {
                    return;
                }
                ranking[0].DaysAtNumberOne++;


                for (int i = 0; i < ranking.Count; i++)
                {
                    ranking[i].DaysAtTopTen++;
                }

                var numberOfChanges = _context.SaveChanges();

            }

        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
