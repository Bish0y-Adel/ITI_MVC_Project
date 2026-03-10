using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public class Category : BaseEntity
    {
        [Required, MaxLength(150)]
        public string Name { get; set; }

        public int? ParentCategoryId { get; set; }

        [ForeignKey(nameof(ParentCategoryId))]
        public Category? ParentCategory { get; set; }

        public ICollection<Category> SubCategories { get; set; } = new HashSet<Category>();
        public ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}