using Microsoft.AspNetCore.Mvc;
using MirchasovStore.Models;


namespace MirchasovStore.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            return View(Data.ExistingTovars);
        }
    }

}

