using Microsoft.AspNetCore.Mvc;
using MirchasovStore.Models;
using MirchasovStore.Models.ViewModels;


namespace MirchasovStore.Controllers
{
    public class HomeController : BaseController
    {

        public int PageSize = 12;

        public IActionResult Index(int productPage = 1)
        {
            return View(new ProductsListViewModel {
                Products = Data.ExistingTovars
                    .OrderBy(p => p.Article)
                    .Skip((productPage - 1) * PageSize)
                    .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = productPage,
                    ItemsPerPage = PageSize,
                    TotalItems = Data.ExistingTovars.Count()
                }

            });
        }
    }

}

