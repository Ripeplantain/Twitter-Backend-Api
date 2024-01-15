using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TwitterApi.Database;

namespace Api.Integration
{
    public class IntegrationTest : WebApplicationFactory<Program>
    {
        protected readonly HttpClient _client;

        protected IntegrationTest()
        {
            _client = CreateClient();
        }

        protected async Task AuthenticateAsync()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetJwtAsync());
        }

        private async Task<string> GetJwtAsync()
        {
            var registerResponse = await _client.PostAsJsonAsync("/Account/Register", new
            {
                Email = "test@test.com",
                FullName = "Test Test",
                Username = "test30",
                Location = "Test Location",
                Bio = "Test Bio",
                Password = "password24"
            });
            
            registerResponse.EnsureSuccessStatusCode();

            var loginResponse = await _client.PostAsJsonAsync("/Account/Login", new
            {
                Username = "test30",
                Password = "password24"
            });

            loginResponse.EnsureSuccessStatusCode();

            var loginResponseContent = await loginResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            return loginResponseContent!["token"];
        }

        protected async Task RemoveUser()
        {
            var response = await _client.DeleteAsync("/Account/DeleteUser/test30");

            response.EnsureSuccessStatusCode();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<DataContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<DataContext>(options =>
                {
                    options.UseSqlite("Data Source=test.db");
                });

                using (var scope = services.BuildServiceProvider().CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<DataContext>();
                    db.Database.EnsureCreated();
                }
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            File.Delete("testDatabase.db");
        }
    }
}
