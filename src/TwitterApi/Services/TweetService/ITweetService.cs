using TwitterApi.DTO;



namespace TwitterApi.Services.TweetService
{
    public interface ITweetService
    {
        Task<List<TweetDTO>> GetTweetAsync(int pageSize, int pageIndex, CancellationToken cancellationToken);
        Task<List<TweetDTO>?> GetUserTweetAsync(string username);
        Task<TweetDTO?> GetTweetByIdAsync(int id);
        Task<TweetDTO> CreateTweetAsync(string username, TweetDTO input);
        Task<TweetDTO?> UpdateTweetAsync(int id, TweetDTO input);
        Task<bool> DeleteTweetAsync(int id);
    }
}