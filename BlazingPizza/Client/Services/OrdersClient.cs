using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using BlazingPizza.Shared;
using System.Net.Http.Json;

namespace BlazingPizza.Client.Services
{
     

    public class OrdersClient
    {
        private readonly HttpClient HttpClient;

        public OrdersClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public async Task<List<OrderWithStatus>> GetOrders() =>
           await HttpClient.GetFromJsonAsync<List<OrderWithStatus>>("orders");

        public async Task<OrderWithStatus> GetOrder(int orderId) =>
            await HttpClient.GetFromJsonAsync<OrderWithStatus>($"orders/{orderId}");

        public async Task<int> PlaceOrder(Order order)
        {
            var Response = await HttpClient.PostAsJsonAsync("orders", order);
            Response.EnsureSuccessStatusCode();
            var OrderId = await Response.Content.ReadFromJsonAsync<int>();
            return OrderId;
        }

        public async Task SubscribeToNotification(NotificationSubscription subscription)
        {
            var Response = await HttpClient.PutAsJsonAsync("notifications/subscribe", subscription);
            Response.EnsureSuccessStatusCode();
        }

    }
}
