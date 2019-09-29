using System;
using Akka.Actor;
using AkkaWeb.UI.Actors.API;
using AkkaWeb.UI.Actors.Messages;

namespace AkkaWeb.UI.Actors
{
    public class CustomerActor : ReceiveActor
    {
        private readonly int _id;
        private double _balance;

        public CustomerActor(int id, TypedActorRef<BankActor> bank)
        {
            _id = id;
            _balance = 0;

            // Initialise our balance
            Self.Tell(MakeSnapshot());

            Receive<BalanceSnapshot>(snapShot =>
            {
                _balance = snapShot.Balance;
                bank.Tell(MakeSnapshot());
            });

            Receive<BalanceUpdate>(update =>
            {
                _balance += update.BalanceChange;
                bank.Tell(MakeSnapshot());
            });

            Receive<BalanceQuery>(query =>
            {
                Sender.Tell(MakeSnapshot());
            });

            Receive<SpendRandom>(msg =>
            {
                var rand = new Random();
                var sign = rand.NextDouble() > 0.5 ? 1 : -1;

                Self.Tell(new BalanceUpdate
                {
                    BalanceChange = rand.NextDouble() * sign
                });
            });
        }

        private BalanceSnapshot MakeSnapshot()
        {
            return new BalanceSnapshot
            {
                Balance = _balance,
                CustomerId = _id
            };
        }

        public static string CreateName(int id)
        {
            return $"Customer{id}";
        }
    }
}
