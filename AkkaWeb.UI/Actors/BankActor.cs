using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using AkkaWeb.UI.Actors.Messages;
using AkkaWeb.UI.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace AkkaWeb.UI.Actors
{
    public class BankActor : ReceiveActor
    {
        private readonly Dictionary<int, double> _balances;

        public BankActor(IHubContext<BalanceHub> balanceHub)
        {
            _balances = new Dictionary<int, double>();

            Receive<BalanceQuery>(query =>
            {
                Sender.Tell(new BalanceSheet
                {
                    // Take a hard-copy of the balance sheet as in-process messages are passed by reference
                    Balances = _balances.ToDictionary(x => x.Key, x => x.Value)
                });
            });

            Receive<BalanceSnapshot>(snapshot =>
            {
                _balances[snapshot.CustomerId] = snapshot.Balance;
                Task.Run(() => balanceHub.Clients.All.SendAsync("UpdateBalance", snapshot));
            });
        }
    }
}
