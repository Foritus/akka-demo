using System.Threading.Tasks;
using AkkaWeb.UI.Actors.Messages;
using Microsoft.AspNetCore.SignalR;

namespace AkkaWeb.UI.Hubs
{
    public class BalanceHub : Hub
    {
        public async Task UpdateBalance(BalanceSnapshot snapshot)
        {
            await Clients.All.SendAsync("UpdateBalance", snapshot);
        }
    }
}
