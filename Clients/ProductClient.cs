namespace Commerce.Inventory.Service.Clients;

public class ProductClient
{
    private readonly HttpClient _httpClient;

    public ProductClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyCollection<ProductDto>> GetProductListAsync()
    {
        var products = await _httpClient.GetFromJsonAsync<IReadOnlyCollection<ProductDto>>("/product");
        return products;
    }
}