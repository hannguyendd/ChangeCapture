using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using SearchService.Interfaces;
using Shared.Contracts.Products;

namespace SearchService.Services;

public class SearchProductService(ElasticsearchClient elasticsearchClient, ILogger<SearchProductService> logger)
    : ISearchProductService
{
    private readonly ElasticsearchClient _elasticsearchClient = elasticsearchClient;
    private readonly ILogger<SearchProductService> _logger = logger;
    private const string IndexName = "products";

    public async Task<IEnumerable<ProductResponse>> SearchProductsAsync(string searchTerm, int page = 1, int pageSize = 10)
    {
        try
        {
            var from = (page - 1) * pageSize;

            var searchRequest = new SearchRequest(IndexName)
            {
                Query = string.IsNullOrWhiteSpace(searchTerm)
                    ? new MatchAllQuery()
                    : new MultiMatchQuery
                    {
                        Query = searchTerm,
                        Fields = new[] { "name^2", "description" }, // Boost name field
                        Fuzziness = new Fuzziness("AUTO"),
                        Type = TextQueryType.BestFields
                    },
                From = from,
                Size = pageSize,
                Sort = new List<SortOptions>
                {
                    new SortOptions { Field = new FieldSort { Field = "_score", Order = SortOrder.Desc } },
                    new SortOptions { Field = new FieldSort { Field = "name.keyword", Order = SortOrder.Asc } }
                }
            };

            var response = await _elasticsearchClient.SearchAsync<ProductResponse>(searchRequest);

            if (!response.IsValidResponse)
            {
                _logger.LogError("Elasticsearch search failed: {Error}", response.DebugInformation);
                return Enumerable.Empty<ProductResponse>();
            }

            return response.Documents;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching products with term: {SearchTerm}", searchTerm);
            return Enumerable.Empty<ProductResponse>();
        }
    }

    public async Task<ProductResponse?> GetProductByIdAsync(int id)
    {
        try
        {
            var response = await _elasticsearchClient.GetAsync<ProductResponse>(id.ToString(), idx => idx.Index(IndexName));

            if (!response.IsValidResponse || !response.Found)
            {
                _logger.LogWarning("Product with ID {ProductId} not found in Elasticsearch", id);
                return null;
            }

            return response.Source;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting product by ID: {ProductId}", id);
            return null;
        }
    }

    public async Task<IEnumerable<ProductResponse>> GetAllProductsAsync(int page = 1, int pageSize = 10)
    {
        try
        {
            var from = (page - 1) * pageSize;

            var searchRequest = new SearchRequest(IndexName)
            {
                Query = new MatchAllQuery(),
                From = from,
                Size = pageSize,
                Sort = new List<SortOptions>
                {
                    new SortOptions { Field = new FieldSort { Field = "name.keyword", Order = SortOrder.Asc } }
                }
            };

            var response = await _elasticsearchClient.SearchAsync<ProductResponse>(searchRequest);

            if (!response.IsValidResponse)
            {
                _logger.LogError("Elasticsearch get all products failed: {Error}", response.DebugInformation);
                return Enumerable.Empty<ProductResponse>();
            }

            return response.Documents;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all products");
            return Enumerable.Empty<ProductResponse>();
        }
    }

    public async Task IndexProductAsync(ProductChangeEvent productEvent)
    {
        try
        {
            var productResponse = new ProductResponse(
                productEvent.Id,
                productEvent.Name,
                productEvent.Description,
                productEvent.Price
            );

            var response = await _elasticsearchClient.IndexAsync(productResponse, req => req
                .Index(IndexName)
                .Id(productEvent.Id.ToString())
                .Refresh(Refresh.WaitFor));

            if (!response.IsValidResponse)
            {
                _logger.LogError("Failed to index product {ProductId}: {Error}", productEvent.Id, response.DebugInformation);
            }
            else
            {
                _logger.LogInformation("Successfully indexed product {ProductId}: {ProductName}", productEvent.Id, productEvent.Name);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while indexing product: {ProductId}", productEvent.Id);
        }
    }

    public async Task DeleteProductAsync(int id)
    {
        try
        {
            var response = await _elasticsearchClient.DeleteAsync(IndexName, id.ToString());

            if (!response.IsValidResponse)
            {
                _logger.LogError("Failed to delete product {ProductId}: {Error}", id, response.DebugInformation);
            }
            else
            {
                _logger.LogInformation("Successfully deleted product {ProductId} from index", id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting product: {ProductId}", id);
        }
    }
}
