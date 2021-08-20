using BlazingPizza.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazingPizza.Client.Pages
{
    public partial class Checkout
    {
        [Inject]
        public OrderState OrderState { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public OrdersClient OrdersClient { get; set; }

        bool IsSubmitting;

        async Task PlaceOrder()
        {
            IsSubmitting = true;
            try
            {
                int NewOrderID = await OrdersClient.PlaceOrder(OrderState.Order);
                OrderState.ResetOrder();
                NavigationManager.NavigateTo($"myorders/{NewOrderID}");
            }
            catch (AccessTokenNotAvailableException ex) 
            {
                ex.Redirect();
            }
            finally
            {
                IsSubmitting = false;
            }                            
        }

    }
}
