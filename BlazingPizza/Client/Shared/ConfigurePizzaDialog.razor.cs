using BlazingPizza.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazingPizza.Client.Shared
{
    public partial class ConfigurePizzaDialog
    {
        [Inject]
        public HttpClient HttpClient { get; set; }

        [Parameter]
        public Pizza Pizza { get; set; }

        [Parameter]
        public EventCallback OnCancel { get; set; }

        [Parameter]
        public EventCallback OnConfirm { get; set; }

        IEnumerable<Topping> Toppings;


        protected async override Task OnInitializedAsync()
        {
            Toppings = await HttpClient.GetFromJsonAsync<IEnumerable<Topping>>("toppings");
        }


        void AddTopping(Topping topping)
        {
            if (Pizza.Toppings.Find(pt=>pt.Topping == topping) == null)
            {
                Pizza.Toppings.Add(new PizzaTopping { Topping = topping });
            }
        }


        void ToppingSelected(ChangeEventArgs e)
        {
            if (int.TryParse((string)e.Value, out var index) && index >= 0)
            {
                AddTopping(Toppings.ElementAt(index));
            }
        }

        void RemoveTopping(Topping topping)
        {
            Pizza.Toppings.RemoveAll(pt => pt.Topping == topping);
        }

    }
}
