using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TwitterApi.DTO;
using TwitterApi.Database;
using TwitterApi.Service;
using TwitterApi.Models;
using Microsoft.EntityFrameworkCore;


namespace TwitterApi.Controllers
{
    [Route("[controller]/[action]")]
    [Authorize]
    [ApiController]
    public class ChatController(IHubContext<ChatHub> hubContext, DataContext context, ILogger<ChatController> logger) : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext = hubContext;
        private readonly DataContext _context = context;
        private readonly ILogger<ChatController> _logger = logger;

        [HttpPost]
        public async Task<IActionResult> SendMessage(ChatDTO chatDTO)
        {
            if (User == null || User.Identity == null)
            {
                return Unauthorized();
            }

            var receipient = await _context.Users.FindAsync(chatDTO.RecipientId);
            if (receipient == null)
            {
                return StatusCode(404, "Receipient not found");
            }

            var senderId = User.Identity.Name;
            var recipientId = chatDTO.RecipientId;
            var message = chatDTO.Message;

            await _hubContext.Clients.User(recipientId).SendAsync("ReceiveMessage", senderId, message);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateChatroom()
        {
            try {
                var chatRoom = new ChatRoom
                {
                    Name = Guid.NewGuid().ToString()
                };

                await _context.ChatRooms.AddAsync(chatRoom);
                await _context.SaveChangesAsync();

                return StatusCode(201, "Chat room created successfully");
            } catch (Exception e)
            {
                _logger.LogError(e, "Error");
                return StatusCode(500, "Error creating chat room");
            }
        }

        [HttpGet("/{id}")]
        public async Task<IActionResult> GetMessages(int id)
        {
            try {
                var chatRoom = await _context.ChatRooms.FindAsync(id);

                if (chatRoom == null)
                {
                    return NotFound();
                }

                var messages = await _context.Messages
                    .Where(m => m.ChatroomId == id)
                    .ToListAsync(); 

                return Ok(new ResultDTO<Message> {
                    Status = "Chatroom created successfully",
                    Count = messages.Count,
                    Data = messages
                });

            } catch (Exception e) {
                _logger.LogError(e, "Error");
                return StatusCode(500, "Error fetching messages");
            }
        }
    }
}
