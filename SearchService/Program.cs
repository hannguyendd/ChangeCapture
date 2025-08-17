using System.Net.Mime;
using Elastic.Clients.Elasticsearch;
using MassTransit;
using Scalar.AspNetCore;
using SearchService.Interfaces;
using SearchService.Services;
using Shared.Constants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// Configure Elasticsearch
builder.Services.AddSingleton<ElasticsearchClient>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var elasticsearchUrl = configuration.GetConnectionString("Elasticsearch") ?? "http://localhost:9200";
    var defaultIndex = configuration["ElasticsearchSettings:DefaultIndex"] ?? SearchIndex.Products;

    var settings = new ElasticsearchClientSettings(new Uri(elasticsearchUrl))
        .DefaultIndex(defaultIndex);

    return new ElasticsearchClient(settings);
});

// Register services
builder.Services.AddScoped<ISearchProductService, SearchProductService>();
builder.Services.AddScoped<IElasticsearchIndexService, ElasticsearchIndexService>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ProductChangesConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMQ"));

        cfg.ReceiveEndpoint("product-changes", endpointCfg =>
        {
            endpointCfg.ConfigureConsumeTopology = false;

            endpointCfg.DefaultContentType = new ContentType("application/json");
            endpointCfg.UseRawJsonDeserializer();

            endpointCfg.Consumer<ProductChangesConsumer>(context);
        });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// Map controllers
app.MapControllers();

// Initialize Elasticsearch index
using (var scope = app.Services.CreateScope())
{
    var indexService = scope.ServiceProvider.GetRequiredService<IElasticsearchIndexService>();
    await indexService.EnsureIndexExistsAsync();
}

app.Run();

