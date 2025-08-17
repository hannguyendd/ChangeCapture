using Dumpify;
using MassTransit;
using Shared.Contracts.Products;

namespace SearchService.Services;

public class ProductChangesConsumer(ILogger<ProductChangesConsumer> logger) : IConsumer<ProductChangeEvent>
{
    private readonly ILogger<ProductChangesConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<ProductChangeEvent> context)
    {
        var @event = context.Message;
        @event?.Dump();
    }
}