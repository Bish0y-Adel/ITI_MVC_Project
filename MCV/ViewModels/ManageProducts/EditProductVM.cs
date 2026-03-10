using Microsoft.AspNetCore.Mvc.Rendering;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCV.ViewModels.ManageProducts
{
    public class EditProductVM
    {
        public int Id { get; set; }
        [Required, MaxLength(200)]
        public string Name { get; set; }

        [Required, MaxLength(100)]

        public string SKU { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Required]
        public decimal Price { get; set; }
        [Required]

        public int StockQuantity { get; set; }
        [Required]
        public bool IsActive { get; set; } = true;
        //===========Navigatiion Properties===========
        [Required]
        public int? CategoryId { get; set; }

        public IEnumerable<SelectListItem>? Categories { get; set; } = new HashSet<SelectListItem>();
    }
}
