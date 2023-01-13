using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MirchasovStore.BaseController;

namespace MirchasovStore.Models.ViewModels;

public class ProductsListViewModel
{
    public IEnumerable<Product> Products { get; set; }
    public Category Categories { get; set; }
    public PagingInfo PagingInfo { get; set; }
    public string CurrentCategory { get; set; }

    public ViewSettingsClass ViewSettings { get; set; }
}

