using Bit.Http.Contracts;
using BlazorDualMode.Shared;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorDualMode.Web.Pages
{
    public partial class FetchData
    {
        public List<WeatherForecast> Forecasts { get; set; }

        protected override async Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            Forecasts = await HttpClient.Weather().GetWeatherForecasts(oDataContext: new ODataContext { Query = "$top=3" });

            var context = new ODataContext { Query = "$top=3&$count=true" };
            var forecasts = await HttpClient.Weather().GetWeatherForecasts(context);
            var totalCount = context.TotalCount;

            int result = await HttpClient.Math().Sum(1, 2);
        }
    }
}
