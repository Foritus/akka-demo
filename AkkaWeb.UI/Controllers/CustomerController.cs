using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using AkkaWeb.UI.Actors;
using AkkaWeb.UI.Actors.API;
using AkkaWeb.UI.Actors.Messages;
using Microsoft.AspNetCore.Mvc;

namespace AkkaWeb.UI.Controllers
{
    public class CustomerController : Controller
    {
        private static int _idCounter;

        private readonly ActorSystem _actorSystem;
        private readonly TypedActorRef<BankActor> _bank;

        public CustomerController(ActorSystem actorSystem, TypedActorRef<BankActor> bank)
        {
            _actorSystem = actorSystem;
            _bank = bank;
        }

        public async Task<IActionResult> Create()
        {
            var id = Interlocked.Increment(ref _idCounter);
            var customer = await GetOrCreateCustomer(id);
            return GoHome();
        }

        public async Task<IActionResult> AddRandom(int customerId)
        {
            var customer = await GetOrCreateCustomer(customerId);
            var rand = new Random();

            customer.Tell(new BalanceUpdate
            {
                BalanceChange = 50 * rand.NextDouble()
            });

            return GoHome();
        }

        private async Task<IActorRef> GetOrCreateCustomer(int id)
        {
            string name = CustomerActor.CreateName(id);
            try
            {
                return await _actorSystem.ActorSelection($"akka://AkkaWeb/user/" + CustomerActor.CreateName(id))
                                         .ResolveOne(TimeSpan.FromMilliseconds(250));
            }
            catch (ActorNotFoundException)
            {
                return _actorSystem.ActorOf(Props.Create(() => new CustomerActor(id, _bank)), name);
            }
        }

        private IActionResult GoHome()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
