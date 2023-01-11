namespace MirchasovStore.Models
{
    public class Category
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string ParentId { get; set; }

        public List<Category> Children { get; set; }

        public bool HasChildren { get; set; }
    }
}
