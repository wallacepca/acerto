namespace Acerto.Products.SDK
{
    public interface IAuthTokenStore
    {
        public Task<T?> GetTokenAsync<T>();
        public Task<T> SetTokenAsync<T>(T token);
    }
}
