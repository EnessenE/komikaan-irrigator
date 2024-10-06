using System.Reflection;
using komikaan.Irrigator.Services;
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
                .ReadFrom.Configuration(builder.Configuration)
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
            builder.Services.AddHttpClient();


            AddSuppliers(builder.Services);

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            ConfigureHealthChecks(builder);

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

        private static void ConfigureHealthChecks(WebApplicationBuilder builder)
        {
            builder.Services.AddNpgsqlDataSource(builder.Configuration.GetConnectionString("gtfs")!);
            builder.Services.AddHealthChecks().AddNpgSql();
        }

        private static void AddSuppliers(IServiceCollection serviceCollection)
        {
            serviceCollection.AddHostedService<GTFSRealtimeRetriever>();
        }
    }
}
