using Entities.Models;

using Microsoft.AspNetCore.Mvc.Rendering;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCV.ViewModels.ManageCategory
{
    public class CreateCategoryVM
    {
        [Required, MaxLength(150),MinLength(5)]
        public string Name { get; set; }

        public int? ParentCategoryId { get; set; }

        public IEnumerable<SelectListItem>? ParentCategories { get; set; } = new HashSet<SelectListItem>();

    }
}
