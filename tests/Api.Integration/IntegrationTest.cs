using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.EntityFrameworkCore;
using TwitterApi.Database;
using System.Net.Http.Headers;
using System.Net.Http.Json;



namespace Api.Integration
{
    public class IntegrationTest
    {
        protected readonly HttpClient _client;

        protected IntegrationTest()
        {
            var app = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll(typeof(DataContext));
                        services.AddDbContext<DataContext>(options =>
                        {
                            options.UseInMemoryDatabase("TestDb");
                        });
                    });
                });
            _client = app.CreateClient();
        }


        


        protected async Task AuthenticateAsync()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetJwtAsync());
        }

        private async Task<string> GetJwtAsync()
        {
            var resgisterResponse = await _client.PostAsJsonAsync("Account/Register", new
            {
                Username = "testUser3",
                Email = "test@test.com",
                FullName = "Test User",
                Bio = "Test Bio",
                Location = "Test Location",
                Password = "password24"
            });

            Console.WriteLine(resgisterResponse.StatusCode);
            Console.WriteLine(resgisterResponse.Content.ReadAsStringAsync().Result);

            resgisterResponse.EnsureSuccessStatusCode();

            var loginResponse = await _client.PostAsJsonAsync("Account/Login", new
            {
                Username = "testUser3",
                Password = "password24"
            });

            var loginResponseContent = await loginResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>();

            return loginResponseContent?["token"] ?? string.Empty;
        }
    }
}