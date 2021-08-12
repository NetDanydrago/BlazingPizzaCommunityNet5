using BlazingPizza.Client.Services;
using BlazingPizza.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazingPizza.Client.Pages
{
    public partial class Index
    {

        [Inject]
        public HttpClient HttpClient { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public OrderState OrderState { get; set; }

        List<PizzaSpecial> Specials;

        protected async override Task OnInitializedAsync()
        {
            Specials = await HttpClient
                .GetFromJsonAsync<List<PizzaSpecial>>("specials");
        }

    }
}
