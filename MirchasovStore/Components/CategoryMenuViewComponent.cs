using Microsoft.AspNetCore.Mvc;
using MirchasovStore.Models;

namespace MirchasovStore.Components
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            ViewBag.SelectedCategory = RouteData?.Values["id"];

            //foreach (var path in Data.root.Children) { 
                
            //}
            return View(Data.menuTree);
        }
    }
}
