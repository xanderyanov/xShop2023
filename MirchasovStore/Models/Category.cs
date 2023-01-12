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

        public List<Product> Products { get; set; } = new();
    }
}
