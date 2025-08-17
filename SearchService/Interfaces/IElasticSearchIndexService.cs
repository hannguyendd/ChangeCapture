namespace SearchService.Interfaces;

public interface IElasticsearchIndexService
{
    Task EnsureIndexExistsAsync();
}
