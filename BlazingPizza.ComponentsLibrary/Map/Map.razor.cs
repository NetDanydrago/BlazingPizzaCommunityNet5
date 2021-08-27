using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazingPizza.ComponentsLibrary.Map
{
    public partial class Map
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Parameter]
        public List<Marker> Markers { get; set; }

        public string ElementId { get; set; } = $"map--{Guid.NewGuid()}";

        protected async override Task OnAfterRenderAsync(bool FirstRender)
        {
            await JSRuntime.InvokeVoidAsync("deliveryMap.showOrUpdate", ElementId, Markers);
        }
    }
}
