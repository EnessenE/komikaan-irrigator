using System.Reflection;
using komikaan.Irrigator.Contexts;
using komikaan.Irrigator.Factories;
using komikaan.Irrigator.Helpers;
using komikaan.Irrigator.Interfaces;
using komikaan.Irrigator.Managers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;

namespace komikaan.Irrigator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddEnvironmentVariables();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            builder.Host.UseSerilog();
            Log.Logger.Information("Starting {app} {version} - {env}",
                Assembly.GetExecutingAssembly().GetName().Name,
                Assembly.GetExecutingAssembly().GetName().Version,
                builder.Environment.EnvironmentName);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            AddSuppliers(builder.Services);

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            builder.Services.AddHostedService<HarvestingManager>();
            builder.Services.AddSingleton<GardenerContext>();
            builder.Services.AddSingleton<IDataContext, PostgresContext>();
            builder.Services.AddDbContext<GTFSContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("HarvestingTarget"), o => o.UseNetTopologySuite());
                options.UseSnakeCaseNamingConvention();
                options.ReplaceService<ISqlGenerationHelper, NpgsqlSqlGenerationLowercasingHelper>();
            }, optionsLifetime: ServiceLifetime.Singleton, contextLifetime: ServiceLifetime.Singleton); 
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseSerilogRequestLogging();

            app.MapControllers();

            app.Run();
        }

        private static void AddSuppliers(IServiceCollection serviceCollection)
        {
            var factory = new SupplierFactory(serviceCollection);
            factory.AddSuppliers();
        }
    }
}
