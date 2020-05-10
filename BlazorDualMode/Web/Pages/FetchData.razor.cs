using BlazorDualMode.Shared;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorDualMode.Web.Pages
{
    public partial class FetchData
    {
        WeatherForecast[] forecasts;

        protected override async Task OnInitializedAsync()
        {
            forecasts = await Http.GetFromJsonAsync<WeatherForecast[]>("WeatherForecast");
        }
    }
}
