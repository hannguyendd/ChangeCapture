using Dumpify;
using MassTransit;
using SearchService.Interfaces;
using Shared.Contracts.Products;

namespace SearchService.Services;

public class ProductChangesConsumer(ILogger<ProductChangesConsumer> logger, ISearchProductService searchService) : IConsumer<ProductChangeEvent>
{
    private readonly ILogger<ProductChangesConsumer> _logger = logger;
    private readonly ISearchProductService _searchService = searchService;

    public async Task Consume(ConsumeContext<ProductChangeEvent> context)
    {
        var @event = context.Message;

        if (@event == null)
        {
            _logger.LogWarning("Received null product change event");
            return;
        }

        _logger.LogInformation("Received product change event for Product ID: {ProductId}", @event.Id);
        @event.Dump();

        try
        {
            // Index the product change event in Elasticsearch
            await _searchService.IndexProductAsync(@event);

            _logger.LogInformation("Successfully processed product change event for Product ID: {ProductId}", @event.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process product change event for Product ID: {ProductId}", @event.Id);
            throw; // Re-throw to trigger MassTransit retry logic if configured
        }
    }
}