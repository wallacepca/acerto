using System.Diagnostics;
using System.Net.Http.Json;
using Acerto.Auth.SDK;
using Acerto.Orders.SDK;
using Acerto.Products.SDK;
using Acerto.Shared.Contracts;
using NetDevPack.Identity.Jwt.Model;
using NetDevPack.Identity.Model;

namespace Acerto.Testing.App
{
    internal class TestConsole
    {
        private readonly IAuthApiClient _authApiClient;
        private readonly IProductsApiClient _productsApiClient;
        private readonly IOrdersApiClient _ordersApiClient;
        private readonly IAuthTokenStore _authTokenStore;

        public TestConsole(
            IAuthApiClient authApiClient,
            IProductsApiClient productsApiClient,
            IOrdersApiClient ordersApiClient,
            IAuthTokenStore authTokenStore)
        {
            _authApiClient = authApiClient;
            _productsApiClient = productsApiClient;
            _ordersApiClient = ordersApiClient;
            _authTokenStore = authTokenStore;
        }

        public async Task RunAsync()
        {
            var sw = Stopwatch.StartNew();

            var user = await GetOrCreateAcertoUserAsync();

            var createdProducts = new HashSet<Guid>();

            foreach (var item in Enumerable.Range(10, 60))
            {
                var productId = Guid.NewGuid();
                var stamp = Path.GetFileNameWithoutExtension(Path.GetTempFileName());
                var productRequest = new ProductRequest(productId, $"Galaxy S{item} Ultra {stamp}", "Samsung", 5_000 + (item * 10))
                {
                    Description = LoremIpsumGenerator.Generate(2000),
                    Color = stamp,
                };

                var productResponse = await _productsApiClient.CreateProductAsync(productRequest);
                Console.WriteLine($"Produto {productResponse.Name} criado com sucesso!.");
                createdProducts.Add(productId);
            }

            Console.WriteLine($"{createdProducts.Count} criados!");

            foreach (var id in createdProducts)
            {
                var productResponse = await _productsApiClient.GetProductAsync(id);
                Console.WriteLine($"Produto {productResponse.Name} consultado com sucesso!.");
            }

            var allProducts = await _productsApiClient.GetAllProductsAsync();
            Console.WriteLine($"{allProducts.Count()} produtos existentes na base de dados!");

            var ordersItemsChunks = createdProducts.Chunk(3);

            var ordersCreated = new HashSet<Guid>();
            foreach (var chunk in ordersItemsChunks)
            {
                var orderId = Guid.NewGuid();
                var order = new OrderRequest(orderId, chunk.Select(x => new OrderItemRequest(Guid.Empty, x, 5)));
                var orderResponse = await _ordersApiClient.CreateOrderAsync(order);
                Console.WriteLine($"Pedido {orderId} criado com sucesso!");

                ordersCreated.Add(orderId);
            }

            var allOrders = await _ordersApiClient.GetAllOrdersAsync();
            Console.WriteLine($"Um total de {allOrders.Count()} na base de dados.");

            var completedOrders = new HashSet<Guid>();

            await Task.Delay(5000);
            Console.WriteLine($"Aguardando processamento de pedidos....");

            var finishedOrdersCount = allOrders.Count(x => x.OrderStatusChanges.LastOrDefault()?.Status == OrderStatus.Completed || x.OrderStatusChanges.LastOrDefault()?.Status == OrderStatus.Canceled);
            while (finishedOrdersCount < ordersCreated.Count)
            {
                foreach (var finishedOrder in allOrders)
                {
                    var latestStaus = finishedOrder.OrderStatusChanges.LastOrDefault();
                    if (latestStaus != null && (latestStaus.Status == OrderStatus.Completed || latestStaus.Status == OrderStatus.Canceled) && !completedOrders.Contains(finishedOrder.Id))
                    {
                        completedOrders.Add(finishedOrder.Id);
                        Console.WriteLine($"Pedido {finishedOrder.Id} concluido com o status {latestStaus.Status}:{latestStaus.StatusReason} ({completedOrders.Count}/{allOrders.Count()})");
                    }
                }

                await Task.Delay(1000);

                allOrders = await _ordersApiClient.GetAllOrdersAsync();
                finishedOrdersCount = allOrders.Count(x => x.OrderStatusChanges.LastOrDefault()?.Status == OrderStatus.Completed || x.OrderStatusChanges.LastOrDefault()?.Status == OrderStatus.Canceled);
            }

            Console.WriteLine($"Processamento de pedidos encerrado...");

            sw.Stop();
            Console.WriteLine($"Teste concluído em {sw.Elapsed}");
        }

        private async Task<UserResponse?> GetOrCreateAcertoUserAsync()
        {
            var loginResponse = await _authApiClient.SendLoginAsync(new LoginUser { Email = ApiCredentials.Email, Password = ApiCredentials.Password });

            UserResponse? user;

            if (!loginResponse.IsSuccessStatusCode)
            {
                var message = await loginResponse.Content.ReadAsStringAsync();

                Console.WriteLine($"Usuário {ApiCredentials.Email} não existe na base de dados e será registrado: {message}");

                user = await _authApiClient.RegisterUserAsync(new RegisterUser
                {
                    Email = ApiCredentials.Email,
                    Password = ApiCredentials.Password,
                    ConfirmPassword = ApiCredentials.Password
                });

                Console.WriteLine($"Usuário {ApiCredentials.Email} criado com sucesso.");
            }
            else
            {
                Console.WriteLine($"Usuário {ApiCredentials.Email} já existe na base de dados.");
                user = await loginResponse.Content.ReadFromJsonAsync<UserResponse>();
            }

            if (user != null)
            {
                Console.WriteLine($"\n\nAccessToken: {user?.AccessToken}\n\n");
                await _authTokenStore.SetTokenAsync(user);
            }

            return user;
        }
    }
}
