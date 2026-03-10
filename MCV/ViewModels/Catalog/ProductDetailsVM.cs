using Entities.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCV.ViewModels.Catalog
{
    public class ProductDetailsVM
    {

        public int Id { get; set; }
        [Required, MaxLength(200)]
        public string Name { get; set; }

        [Required, MaxLength(100)]
        public string SKU { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public string CategoryName { get; set; }
        //=========================================

    }
}
