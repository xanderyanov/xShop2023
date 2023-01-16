using MirchasovStore;
using MirchasovStore.Models;
using System.Text;

var Prov = CodePagesEncodingProvider.Instance;
Encoding.RegisterProvider(Prov);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();


Data.InitData(builder.Configuration);

//Data.ImportCSV();     //Обновление товаров

builder.Services.AddRazorPages();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

app.UseStaticFiles();
app.UseRouting();
app.UseSession();

//стр 170  Improving  the URLs
//app.MapControllerRoute("pagination",
//    "Products/Page{productPage}",
//    new { Controller = "Home", action = "Index" });
//

app.MapControllerRoute("catpage",
    "{category}/Page{productPage:int}",
    new { Controller = "Home", action = "Index" });
//app.MapControllerRoute("page", "Page{productPage:int}",
//    new { Controller = "Home", action = "Index", productPage = 1 });
app.MapControllerRoute("category", "{category}",
    new { Controller = "Home", action = "Index", productPage = 1 });
//app.MapControllerRoute("pagination",
//    "Products/Page{productPage}",
//    new { Controller = "Home", action = "Index", productPage = 1 });


app.MapDefaultControllerRoute();

app.MapRazorPages();

app.Run();
