using System.Collections.Generic;

namespace AkkaWeb.UI.Actors.Messages
{
    public class BalanceSheet
    {
        public Dictionary<int, double> Balances { get; set; }
    }
}
