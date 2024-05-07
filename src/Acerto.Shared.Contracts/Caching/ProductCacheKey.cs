namespace Acerto.Shared.Contracts.Caching
{
    public static class ProductCacheKey
    {
        public static string GetKey(Guid id) => $"product-{id}";
    }
}
