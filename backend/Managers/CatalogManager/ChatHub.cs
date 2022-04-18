using Microsoft.AspNetCore.SignalR;

namespace CatalogManager
{
    public class ChatHub :Hub
    {
        private readonly ILogger<ChatHub> _logger;



       


        public ChatHub(ILogger<ChatHub> logger)
        {
           
            _logger = logger;
        }
        public async Task BroadcastMessage(string name, string message)
        {
            try
            {
                await Clients.All.SendAsync("broadcastMessage", name, message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
            

        public async Task Echo(string name, string message)
        {
            try
            {
                await Clients.Client(Context.ConnectionId)
                        .SendAsync("echo", name, $"{message} (echo from server)");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
            
}
