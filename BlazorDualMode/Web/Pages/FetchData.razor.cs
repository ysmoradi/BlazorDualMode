using BlazorDualMode.Shared;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace BlazorDualMode.Web.Pages
{
    public partial class FetchData
    {
        WeatherForecast[] forecasts;

        protected override async Task OnInitializedAsync()
        {
            forecasts = await Http.GetJsonAsync<WeatherForecast[]>("WeatherForecast");
        }
    }
}
