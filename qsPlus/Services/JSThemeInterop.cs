using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace qsPlus.Services
{
    public static class JSThemeInterop
    {
        public static async ValueTask<bool> GetSystemPrefersDarkModeAsync(this IJSRuntime jsRuntime)
        {
            return await jsRuntime.InvokeAsync<bool>(
                "eval", 
                "window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches");
        }
    }
}