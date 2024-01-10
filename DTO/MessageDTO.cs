

namespace TwitterApi.DTO
{
    public class ResultDTO<T>
    {
        public string Status {get; set;} = string.Empty;
        public int Count {get; set;}

        public List<T> Data {get; set;} = [];
    }
}