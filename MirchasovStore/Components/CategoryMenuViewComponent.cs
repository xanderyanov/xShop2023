using Microsoft.AspNetCore.Mvc;
using MirchasovStore.Models;

namespace MirchasovStore.Components
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            ViewBag.SelectedCategory = RouteData?.Values["id"];

            foreach (var path in Data.Razdels) { 
                var punkt = path.Split('/').ToList();
            }









            return View(Data.Razdels);
        }
    }
}
