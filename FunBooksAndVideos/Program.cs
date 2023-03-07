using FunBooksAndVideos.Data;
using FunBooksAndVideos.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;

namespace FunBooksAndVideos
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            //Debugger.Launch();

#if DEBUG
            Console.WriteLine("!!!!! DEBUG !!!!!");
#endif

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            ConfigurationManager configuration = builder.Configuration;
            string connectionString = configuration.GetConnectionString("FunDbConnection");

            //ignoring dotnet ef
            if (args.Length > 0 && args[0] != "--applicationName")
            {
                using ILoggerFactory loggerFactory = LoggerFactory.Create(x => x.AddNLog());
                DbErrors dbCheckResult = DbHelper.CheckDb(connectionString, loggerFactory);

                if (dbCheckResult > 0)
                {
                    return (byte)dbCheckResult;
                }
            }

            builder.Logging.ClearProviders();
            builder.Logging.SetMinimumLevel(LogLevel.Trace);
            builder.Logging.AddNLog();

            //builder.Services.AddSingleton("FunDbConnection");
            builder.Services.AddScoped<IPurchaseOrderProcessor, PurchaseOrderProcessor>();

            builder.Services.AddDbContext<IFunDbContext, FunDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            WebApplication app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();

            return 0;
        }
    }
}