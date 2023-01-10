using MirchasovStore;
using System.Text;

var Prov = CodePagesEncodingProvider.Instance;
Encoding.RegisterProvider(Prov);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();


Data.InitData(builder.Configuration);

//Data.ImportCSV();     //Обновление товаров

var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

app.UseStaticFiles();

//стр 170  Improving  the URLs
app.MapControllerRoute("pagination",
    "Products/Page{productPage}",
    new { Controller = "Home", action = "Index" });
//

app.MapDefaultControllerRoute();


app.Run();
