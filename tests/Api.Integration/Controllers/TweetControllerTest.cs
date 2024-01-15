using System.Net;
using System.Text;
using Newtonsoft.Json;
using TwitterApi.DTO;
using TwitterApi.Models;

namespace Api.Integration
{
    public class TweetControllerTest : IntegrationTest
    {

        private static async Task<int> GetCreatedTweetId(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var tweetId = JsonConvert.DeserializeObject<Tweet>(responseContent)?.Id;
            return tweetId ?? 0;
        }


        [Fact]
        public async  Task GetTweetsReturnsOk()
        {
            try {
                await AuthenticateAsync();
                var response = await _client.GetAsync("/Tweet/GetTweets");

                response.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            } finally {
                await RemoveUser();
                Console.WriteLine("Get Tweet Test Done");
            }
        }

        [Fact]
        public async Task CreateTweetReturnsOk()
        {
            try
            {
                await AuthenticateAsync();

                var input = new TweetDTO { Content = "Test Tweet" };
                var json = JsonConvert.SerializeObject(input);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync("/Tweet/CreateTweet", content);

                Console.WriteLine(response.StatusCode);
                Console.WriteLine(await response.Content.ReadAsStringAsync());

                response.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
            finally
            {
                await RemoveUser();
                Console.WriteLine("Create Tweet Test Done");
            }
        }

        [Fact]
        public async Task GetTweetReturnsOk()
        {
            try
            {
                await AuthenticateAsync();
                
                var newTweet = new TweetDTO { Content = "Test Tweet" };
                var json = JsonConvert.SerializeObject(newTweet);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var createTweetResponse = await _client.PostAsync("/Tweet/CreateTweet", content);

                var createdTweetId = await GetCreatedTweetId(createTweetResponse);

                var response = await _client.GetAsync($"/Tweet/GetTweet/{createdTweetId}");

                response.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
            finally
            {
                await RemoveUser();
                Console.WriteLine("Get Tweet Test Done");
            }
        }

        [Fact]
        public async Task DeleteTweetReturnsOk()
        {
            try
            {
                await AuthenticateAsync();

                var newTweet = new TweetDTO { Content = "Test Tweet" };
                var json = JsonConvert.SerializeObject(newTweet);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var createTweetResponse = await _client.PostAsync("/Tweet/CreateTweet", content);

                var createdTweetId = await GetCreatedTweetId(createTweetResponse);

                var response = await _client.DeleteAsync($"/Tweet/DeleteTweet/{createdTweetId}");

                response.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
            finally
            {
                await RemoveUser();
                Console.WriteLine("Delete Tweet Test Done");
            }
        }

        [Fact]
        public async Task UpdateTweetReturnsOk()
        {
            try
            {
                await AuthenticateAsync();

                var newTweet = new TweetDTO { Content = "Test Tweet" };
                var json = JsonConvert.SerializeObject(newTweet);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var createTweetResponse = await _client.PostAsync("/Tweet/CreateTweet", content);

                var createdTweetId = await GetCreatedTweetId(createTweetResponse);

                var updateTweet = new TweetDTO { Content = "Updated Test Tweet" };
                var updateJson = JsonConvert.SerializeObject(updateTweet);
                var updateContent = new StringContent(updateJson, Encoding.UTF8, "application/json");
                var response = await _client.PutAsync($"/Tweet/UpdateTweet/{createdTweetId}", updateContent);

                response.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
            finally
            {
                await RemoveUser();
                Console.WriteLine("Update Tweet Test Done");
            }
        }

        [Fact]
        public async Task GetUserTweetsReturnOk()
        {
            try
            {
                await AuthenticateAsync();

                var newTweet = new TweetDTO { Content = "Test Tweet" };
                var json = JsonConvert.SerializeObject(newTweet);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await _client.PostAsync("/Tweet/CreateTweet", content);

                var response = await _client.GetAsync($"/Tweet/GetUserTweets/test30/tweets");

                response.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<ResponseDTO<List<Tweet>>>(responseContent);
                Assert.Equal(1, responseObject?.Count);
            }
            finally
            {
                await RemoveUser();
                Console.WriteLine("Get User Tweets Test Done");
            }
        }
    }
}

