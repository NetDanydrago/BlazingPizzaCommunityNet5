using BlazingPizza.Server.Models;
using BlazingPizza.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebPush;
using System.Text.Json;

namespace BlazingPizza.Server.Controllers
{
    [Route("orders")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly PizzaStoreContext Context;
        public OrdersController(PizzaStoreContext context) =>
            this.Context = context;

        private string GetUserId()
        {
            return HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
        
        
        [HttpPost]
        public async Task<ActionResult<int>> PlaceOrder(Order order)
        {
            order.CreatedTime = DateTime.Now;
            // Establecer una ubicación de envío ficticia
            order.DeliveryLocation =
                new LatLong(19.043679206924864, -98.19811254438645);

            order.UserId = GetUserId();

            // Establecer el valor de Pizza.SpecialId y Topping.ToppingId
            // para que no se creen nuevos registros Special y Topping.
            foreach (var Pizza in order.Pizzas)
            {
                Pizza.SpecialId = Pizza.Special.Id;
                Pizza.Special = null;

                foreach (var topping in Pizza.Toppings)
                {
                    topping.ToppingId = topping.Topping.Id;
                    topping.Topping = null;
                }
            }

            Context.Orders.Attach(order);
            await Context.SaveChangesAsync();

            //en segundo plano, enviar notificaciones push de ser posible
            var Subscription = await Context.NotificationSubscriptions.Where(
                e => e.UserId == GetUserId()).SingleOrDefaultAsync();
            if(Subscription != null)
            {
                _ = TrackAndSendNotificationAsync(order, Subscription);
            }

            return order.OrderId;
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderWithStatus>>> GetOrders()
        {
            var Orders = await Context.Orders.Where(o => o.UserId == GetUserId())
                .Include(o => o.DeliveryLocation)
                .Include(o => o.Pizzas).ThenInclude(p => p.Special)
                .Include(o => o.Pizzas).ThenInclude(p => p.Toppings)
                .ThenInclude(t => t.Topping)
                .OrderByDescending(o=>o.CreatedTime)
                .ToListAsync();
            return Orders.Select(o=> OrderWithStatus.FromOrder(o)).ToList();
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderWithStatus(int orderId)
        {
            IActionResult result;

            var Order = await Context.Orders.Where(o => o.UserId == GetUserId())
                .Where(o => o.OrderId == orderId)
                .Include(o => o.DeliveryLocation)
                .Include(o => o.Pizzas).ThenInclude(p => p.Special)
                .Include(o => o.Pizzas).ThenInclude(p => p.Toppings)
                .ThenInclude(t => t.Topping)
                .SingleOrDefaultAsync();

            if (Order == null)
            {
                result = NotFound();
            }
            else
            {
                result = Ok(OrderWithStatus.FromOrder(Order));
            }

            return result;
        }

        private  async Task SendNotificationAsync(Order order,
            NotificationSubscription subscription,string message)
        {
            //En una aplicacion real puedes generar tus propias llaves 
            var PublicKey = "BLC8GOevpcpjQiLkO7JmVClQjycvTCYWm6Cq_a7wJZlstGTVZvwGFFHMYfXt6Njyvgx_GlXJeo5cSiZ1y4JOx1o";
            var PrivateKey = "OrubzSz3yWACscZXjFQrrtDwCKg-TGFuWhluQ2wLXDo";

            var PushSubscription =
                new PushSubscription(subscription.Url, subscription.P256dh, subscription.Auth);
            //Aqui puedes colocar tu propio correo de informacion
            var VapidDetails = new VapidDetails("mailto:someone@example.com", PublicKey, PrivateKey);
            var WebPushClient = new WebPushClient();
            try
            {
                var Payload = JsonSerializer.Serialize(new
                {
                    message,
                    url = $"myorders/{order.OrderId}"
                });
                await WebPushClient.SendNotificationAsync
                    (PushSubscription, Payload, VapidDetails);
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine($"error al enviar la notificacion push: {ex.Message}");
            }

        }

        private  async Task TrackAndSendNotificationAsync
            (Order order,NotificationSubscription subscription)
        {
            //En la un proyecto real otro proceso backend llevaria el seguimiento de la orden
            await Task.Delay(OrderWithStatus.PreparationDuration);
            await SendNotificationAsync(order, subscription, "!Tu orden ya esta en camino¡");
            await Task.Delay(OrderWithStatus.DeliveryDuration);
            await SendNotificationAsync(order, subscription, "!Tu orden ha sido entregada¡ !Disfrutala¡");
        }
    }
}
