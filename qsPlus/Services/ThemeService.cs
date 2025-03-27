using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace qsPlus.Services
{
    public class ThemeService
    {
        private readonly IJSRuntime _jsRuntime;
        private bool _isDarkMode;
        
        public event Action? OnThemeChanged;

        public bool IsDarkMode => _isDarkMode;

        public ThemeService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        private async Task UpdateDocumentClassAsync()
        {
            // This adds the dark-mode class to the html element for more complete styling
            await _jsRuntime.InvokeVoidAsync("eval", @$"
                document.documentElement.classList.{(_isDarkMode ? "add" : "remove")}('dark-mode');
            ");
        }

        // Call this in your ToggleThemeAsync and SetThemeAsync methods:
        public async Task ToggleThemeAsync()
        {
            _isDarkMode = !_isDarkMode;
            await SaveThemePreferenceAsync();
            await UpdateDocumentClassAsync(); // Add this line
            NotifyThemeChanged();
        }

        // Also add to InitializeAsync
        public async Task InitializeAsync()
        {
            var storedTheme = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "themePreference");
            _isDarkMode = storedTheme == "dark";
            await UpdateDocumentClassAsync(); // Add this line
        }

        public async Task SetThemeAsync(bool isDarkMode)
        {
            if (_isDarkMode != isDarkMode)
            {
                _isDarkMode = isDarkMode;
                await SaveThemePreferenceAsync();
                NotifyThemeChanged();
            }
        }

        private async Task SaveThemePreferenceAsync()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "themePreference", _isDarkMode ? "dark" : "light");
        }

        private void NotifyThemeChanged() => OnThemeChanged?.Invoke();
    }
}