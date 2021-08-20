using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using BlazingPizza.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazingPizza.Client.Services
{
    public class PizzaAuthenticationState : RemoteAuthenticationState
    {
        public Order Order { get; set; }
    }
}
