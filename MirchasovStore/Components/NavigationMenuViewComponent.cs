using Microsoft.AspNetCore.Mvc;
using MirchasovStore.Models;

namespace MirchasovStore.Components
{
    public class NavigationMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            ViewBag.SelectedCategory = RouteData?.Values["category"];
            return View(Data.Categories);
        }
    }
}
