using Microsoft.AspNetCore.Mvc;
using MirchasovStore.Models;
using MirchasovStore.Models.ViewModels;


namespace MirchasovStore.Controllers
{
    public class HomeController : BaseController
    {

        public int PageSize = 20;

        public IActionResult Index(string? category, int productPage = 1)
        {
            return View(new ProductsListViewModel {
                Products = Data.ExistingTovars
                    .Where(p => category == null || p.BrandName == category)
                    .OrderBy(p => p.Article)
                    .Skip((productPage - 1) * PageSize)
                    .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = productPage,
                    ItemsPerPage = PageSize,
                    //TotalItems = Data.ExistingTovars.Count()
                    TotalItems = category == null
                         ? Data.ExistingTovars.Count()
                        : Data.ExistingTovars.Where(e =>
                         e.BrandName == category).Count()
                },
                CurrentCategory = category

            });
        }
    }

}

