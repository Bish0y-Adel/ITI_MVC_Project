using Entities.Repositories;

using MCV.ViewModels.Catalog;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MCV.Controllers.Customer
{
    public class CatalogController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public CatalogController(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        public IActionResult Index(int? categoryId, string? q, string sort = "name", int page = 1)
        {
            const int pageSize = 9;

            // Start with IQueryable — nothing hits the DB yet
            var query = unitOfWork.ProductRepo.Query().Include(p => p.Category)
                .Where(p => p.IsActive);

            // Filter by category
            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId);

            // Search by name or SKU
            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(term) || p.SKU.ToLower().Contains(term));
            }

            // Sort
            query = sort switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "newest" => query.OrderByDescending(p => p.CreatedAt),
                _ => query.OrderBy(p => p.Name)
            };

            // Count BEFORE paging (single SELECT COUNT query)
            var totalItems = query.Count();

            // Page (single SELECT … OFFSET/FETCH query)
            var products = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var vm = new CatalogVM
            {
                Products = products,
                CategoryId = categoryId,
                Q = q,
                Sort = sort,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                Categories = unitOfWork.CategoryRepo.GetAll()
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name,
                        Selected = c.Id == categoryId
                    })
            };

            return View(vm);
        }

        public IActionResult Details(int? id)
        {
            if (id==null)
                return NotFound();

            var product = unitOfWork.ProductRepo.GetById(id.Value, q => q.Include(p => p.Category));

            if (product == null || !product.IsActive)
                return NotFound();

            var vm = new ProductDetailsVM
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.SKU,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                CategoryName = product.Category?.Name
            };

            return View(vm);
        }
    }
}
