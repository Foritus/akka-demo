using System.Diagnostics;
using System.Threading.Tasks;
using Akka.Actor;
using AkkaWeb.UI.Actors;
using AkkaWeb.UI.Actors.API;
using AkkaWeb.UI.Actors.Messages;
using AkkaWeb.UI.Models;
using Microsoft.AspNetCore.Mvc;

namespace AkkaWeb.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ActorSystem _actorSystem;

        private readonly TypedActorRef<BankActor> _bank;

        public HomeController(ActorSystem actorSystem, TypedActorRef<BankActor> bank)
        {
            this._actorSystem = actorSystem;
            this._bank = bank;
        }

        public async Task<IActionResult> Index()
        {
            // Query all customer balances
            BalanceSheet balanceSheet = await _bank.Ask<BalanceSheet>(new BalanceQuery());

            return View(new IndexModel
            {
                BalanceSheet = balanceSheet
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
