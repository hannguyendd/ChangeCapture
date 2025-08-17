using Elastic.Clients.Elasticsearch;
using SearchService.Interfaces;
using Shared.Constants;
using Shared.Contracts.Products;

namespace SearchService.Services;

public class ElasticsearchIndexService(ElasticsearchClient elasticsearchClient, ILogger<ElasticsearchIndexService> logger) : IElasticsearchIndexService
{
    private readonly ElasticsearchClient _elasticsearchClient = elasticsearchClient;
    private readonly ILogger<ElasticsearchIndexService> _logger = logger;
    private const string IndexName = SearchIndex.Products;

    public async Task EnsureIndexExistsAsync()
    {
        try
        {
            var existsResponse = await _elasticsearchClient.Indices.ExistsAsync(IndexName);

            if (existsResponse.Exists)
            {
                _logger.LogInformation("Elasticsearch index '{IndexName}' already exists", IndexName);
                return;
            }

            _logger.LogInformation("Creating Elasticsearch index '{IndexName}'", IndexName);

            var createIndexResponse = await _elasticsearchClient.Indices.CreateAsync(IndexName, c => c
                .Mappings(m => m
                    .Properties<ProductIndex>(p => p
                        .IntegerNumber(n => n.Id)
                        .Text(t => t.Name, td => td
                            .Fields(f => f
                                .Keyword(k => k.Name.Suffix("keyword"))
                            )
                        )
                        .Text(t => t.Description)
                        .FloatNumber(n => n.Price)
                    )
                )
                .Settings(s => s
                    .NumberOfShards(1)
                    .NumberOfReplicas(0)
                    .Analysis(a => a
                        .Analyzers(an => an
                            .Standard("standard_analyzer")
                        )
                    )
                )
            );

            if (createIndexResponse.IsValidResponse)
            {
                _logger.LogInformation("Successfully created Elasticsearch index '{IndexName}'", IndexName);
            }
            else
            {
                _logger.LogError("Failed to create Elasticsearch index '{IndexName}': {Error}",
                    IndexName, createIndexResponse.DebugInformation);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while ensuring Elasticsearch index exists");
        }
    }
}


