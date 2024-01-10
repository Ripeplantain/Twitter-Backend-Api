
namespace TwitterApi.DTO
{
    public class ResponseDTO
    {
        public string Message { get; set; } = string.Empty;
        public int Count { get; set; }
        public List<TweetDTO> Tweets { get; set; } = new List<TweetDTO>();
    }
}

