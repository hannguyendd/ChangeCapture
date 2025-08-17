using System.Net.Mime;
using MassTransit;
using Scalar.AspNetCore;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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

app.Run();

