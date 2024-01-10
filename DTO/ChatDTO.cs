

using System.ComponentModel.DataAnnotations;

namespace TwitterApi.DTO
{
    public class ChatDTO
    {
        [Required]
        public string RecipientId {get; set;} = string.Empty;
        [Required]
        public string Message {get; set;} = string.Empty;
    }
}