using Microsoft.AspNetCore.Mvc;
using MirchasovStore.Models;
using MirchasovStore.Models.ViewModels;


namespace MirchasovStore.Controllers
{
    public class HomeController : BaseController
    {

        public int PageSize = 20;

        public IActionResult Index(string category, int productPage = 1)
        {
            var cat = Data.menuTree.Find(category);

            var products = new List<Product>(1024);
            cat.GetAllProducts(products);



            return View(new ProductsListViewModel
            {
                CurrentCat = cat,
                Products = products
                    //.Where(p => category == null || p.BrandName == category)
                    .OrderBy(p => p.Article)
                    .Skip((productPage - 1) * PageSize)
                    .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = productPage,
                    ItemsPerPage = PageSize,
                    //TotalItems = Data.ExistingTovars.Count()
                    TotalItems = products.Count() //category == null
                         //? products.Count()
                        //: products
                        //.Where(e => e.BrandName == category)
                        //.Count()
                }
                //,
                //CurrentCategory = category
            });


            //return View(new Category
            //{
            //    Products = products
            //        .Where(p => category == null || p.BrandName == category)
            //        .OrderBy(p => p.Article)
            //        .Skip((productPage - 1) * PageSize)
            //        .Take(PageSize),
            //    PagingInfo = new PagingInfo
            //    {
            //        CurrentPage = productPage,
            //        ItemsPerPage = PageSize,
            //        //TotalItems = Data.ExistingTovars.Count()
            //        TotalItems = category == null
            //             ? Data.menuTree.Products.Count()
            //            : Data.menuTree.Products.Where(e =>
            //             e.BrandName == category).Count()
            //    },
            //    //CurrentCategory = category

            //});
        }
    }

}

