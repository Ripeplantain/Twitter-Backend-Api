

namespace TwitterApi.DTO
{
    public class ResponseDTO<T>
    {
        public string Message { get; set; } = string.Empty;
        public int Count { get; set; }
        public T Data { get; set; } = default(T)!;
    }
}