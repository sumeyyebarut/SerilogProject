using Serilog;
using Serilog.Filters;
using Serilog.Sinks.Elasticsearch;
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Host.ConfigureLogging(a => a.ClearProviders()).UseSerilog();
        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        string env = Environment.GetEnvironmentVariable("ASPNET_ENVIRONMENT");
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .AddEnvironmentVariables()//appsetting dosyas�ndan configurasyon ayarlar�n� al�yoruz
            .Build();
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
                .Filter.ByExcluding(Matching.FromSource("Microsoft"))//microsoft ve system loglar�n� d�sarda b�rak�yoruz
                .Filter.ByExcluding(Matching.FromSource("System"))
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Environment", env)
            .ReadFrom.Configuration(config)
            .WriteTo.Elasticsearch(ConfigurationElasticSink(config, env)) //elasticsearch sinkine yazacg�m� belirttim
             //.WriteTo.Console(theme: SystemConsoleTheme.Literate) --konsola yazmak istedi�imizde de bu �ekilde kullanabiliyoruz.
            .CreateLogger();
         static ElasticsearchSinkOptions ConfigurationElasticSink(IConfigurationRoot config, string env)
        {
            return new ElasticsearchSinkOptions(new Uri(config["ElasticSearchConfiguration:Uri"]))
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"sbarut-serilog"
            };
        };
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
 