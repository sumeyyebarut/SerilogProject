using Serilog;
using Serilog.Core;
using Serilog.Sinks.Elasticsearch;


        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Host.ConfigureLogging(a => a.ClearProviders()).UseSerilog();
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        string env = Environment.GetEnvironmentVariable("ASPNET_ENVIRONMENT");
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .AddEnvironmentVariables()//appsetting dosyasýndan configurasyon ayarlarýný alýyoruz
            .Build();
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Error()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Environment", env)
            .ReadFrom.Configuration(config)
            .WriteTo.Elasticsearch(ConfigurationElasticSink(config, env))
            .CreateLogger();

         static ElasticsearchSinkOptions ConfigurationElasticSink(IConfigurationRoot config, string env)
        {
            return new ElasticsearchSinkOptions(new Uri(config["ElasticSearchConfiguration:Uri"]))
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"sumeyye-serilog-project",
            };
        };

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
 