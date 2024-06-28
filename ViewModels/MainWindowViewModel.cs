using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using ReactiveUI;
using StockNote.IO.Models;
using StockNote.IO.Views;

namespace StockNote.IO.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private readonly HttpClient _httpClient;
        private readonly Window _currentWindow;

        private string _email;
        public string Email
        {
            get => _email;
            set => this.RaiseAndSetIfChanged(ref _email, value);
        }
        private string _password;
        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        public MainWindowViewModel(Window currentWindow)
        {
            _httpClient = new HttpClient();
            _currentWindow = currentWindow;
        }

        private async void ChangeView()
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                var onboardingWindow = new Onboarding();
                onboardingWindow.Show();
                _currentWindow.Close();
            });
        }

        public MainWindowViewModel()
        {
            
        }
        public async Task SendLoginRequest()
        {
            var requestBody = new
            {
                email = Email,
                password = Password,
                project = "testing"
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("http://localhost:3000/api/users/login", content);
                response.EnsureSuccessStatusCode();

                // Handle the successful response
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseContent);

                ChangeView();
            }
            catch (Exception ex)
            {
                // Handle the error
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
