using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using ShoppingCart.ShoppingCart;

namespace ShoppingCart
{
    public class ProductCatalogClient : IProductCatalogClient
    {
        private readonly HttpClient _client;
        private const string ProductCatalogBaseUrl = @"https://git.io/JeHiE";
        private const string GetProductPathTemplate = "?productIds=[{0}]";

        public ProductCatalogClient(HttpClient client)
        {
            client.BaseAddress =
                new Uri(ProductCatalogBaseUrl);
            client
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client = client;
        }

        public async Task<IEnumerable<ShoppingCartItem>>
            GetShoppingCartItems(int[] productCatalogIds)
        {
            using var response = await RequestProductFromProductCatalogue(productCatalogIds);
            return await ConvertToShoppingCartItems(response);
        }

        private async Task<HttpResponseMessage> RequestProductFromProductCatalogue(int[] productCatalogueIds)
        {
            var productsResource = string.Format(GetProductPathTemplate, string.Join(",", productCatalogueIds));
            return await _client.GetAsync(productsResource);
        }

        private static async Task<IEnumerable<ShoppingCartItem>> ConvertToShoppingCartItems(
            HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            var products = await
                               JsonSerializer.DeserializeAsync<List<ProductCatalogProduct>>(
                                   await response.Content.ReadAsStreamAsync(),
                                   new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                           ?? new List<ProductCatalogProduct>();
            return products
                .Select(p =>
                    new ShoppingCartItem(
                        p.ProductId,
                        p.ProductName,
                        p.ProductDescription,
                        p.Price
                    ));
        }

        private record ProductCatalogProduct(int ProductId, string ProductName, string ProductDescription, Money Price);
    }

    public interface IProductCatalogClient
    {
        Task<IEnumerable<ShoppingCartItem>> GetShoppingCartItems(int[] productIds);
    }
}