using Bit.Http.Contracts;
using BlazorDualMode.Web.Implementations;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace BlazorDualMode.Web.Pages
{
    public partial class Index
    {
        [Inject]
        public BlazorDualModeAuthenticationStateProvider BlazorDualModeAuthenticationStateProvider { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public async Task Login()
        {
            Token token = await SecurityService.LoginWithCredentials(Username, Password, "BlazorDualModeResOwner", "secret");
            BlazorDualModeAuthenticationStateProvider.StateHasChanged();
        }

        public async Task Logout()
        {
            await TokenProvider.SetTokenAsync(null);
            BlazorDualModeAuthenticationStateProvider.StateHasChanged();
        }
    }
}
