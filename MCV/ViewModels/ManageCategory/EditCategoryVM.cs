using Microsoft.AspNetCore.Mvc.Rendering;

using System.ComponentModel.DataAnnotations;

namespace MCV.ViewModels.ManageCategory
{
    public class EditCategoryVM
    {
        public int Id { get; set; }

        [Required, MaxLength(150), MinLength(5)]
        public string Name { get; set; }

        public int? ParentCategoryId { get; set; }

        public IEnumerable<SelectListItem>? ParentCategories { get; set; }
    }
}
