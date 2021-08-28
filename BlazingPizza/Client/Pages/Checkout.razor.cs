using BlazingPizza.Client.Services;
using BlazingPizza.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.JSInterop;
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

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        bool IsSubmitting;

        protected override void OnInitialized()
        {
            //Preguntar al usuario si desea ser notificado con las actualizaciones de la orden
            _ = RequestNotificationSubscriptionAsync();
        }

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

        async Task RequestNotificationSubscriptionAsync()
        {
            var Subscription = await JSRuntime.InvokeAsync<NotificationSubscription>(
                "blazorPushNotifications.requestSubscription");
            if(Subscription != null)
            {
                try
                {
                    await OrdersClient.SubscribeToNotification(Subscription);
                }
                catch(AccessTokenNotAvailableException ex)
                {
                    ex.Redirect();
                }
            }
        }
    }
}
