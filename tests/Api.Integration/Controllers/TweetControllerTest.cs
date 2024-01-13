using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TwitterApi.Database;
using TwitterApi.DTO;



namespace Api.Integration.TestController
{
    public class TweetControllerTest : IntegrationTest
    {

        [Fact]
        public async Task GetTweets_ReturnSuccess()
        {
            await AuthenticateAsync();

            var response = await _client.GetAsync("Tweet/GetTweets");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

        }
    }
}