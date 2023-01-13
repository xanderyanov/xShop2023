using MirchasovStore.Models.ViewModels;
using MongoDB.Bson;

namespace MirchasovStore.Models
{
    public class Category
    {
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public Category Parent { get; set; }
        public ObjectId ParentId { get; set; }

        public List<Category> Children { get; set; } = new();

        public bool HasChildren => Children.Count > 0;

        public string CurrentCategory { get; set; }

        public List<Product> Products { get; set; } = new();

        internal Category Find(string category)
        {
            if (category == null) return this;

            if (Name == category) return this;
            foreach (var child in Children)
            {
                var r = child.Find(category);
                if (r != null) return r;
            }
            return null;
        }

        public void GetAllProducts(List<Product> products)
        {
            products.AddRange(Products);
            foreach (var child in Children) child.GetAllProducts(products);
        }
    }
}
