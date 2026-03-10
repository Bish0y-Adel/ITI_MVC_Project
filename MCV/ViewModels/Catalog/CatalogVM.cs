using Entities.Models;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace MCV.ViewModels.Catalog
{
    public class CatalogVM
    {
        // --- Results ---
        public ICollection<Product> Products { get; set; } = [];
        public IEnumerable<SelectListItem> Categories { get; set; } = [];

        // --- Filters (round-trip via query string) ---
        public int? CategoryId { get; set; }
        public string? Q { get; set; }
        public string Sort { get; set; } = "name";

        // --- Pagination ---
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 9;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;
    }
}
