using Entities.Models;
using Entities.Repositories;

using MCV.ViewModels.ManageCategory;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MCV.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class ManageCategoryController : Controller
    {
        IUnitOfWork unitOfWork;
        public ManageCategoryController(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public IActionResult Index()
        {
            var categories = unitOfWork.CategoryRepo.GetAll();
            return View(categories);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var category = unitOfWork.CategoryRepo.GetById(id.Value, q => q.Include(c => c.ParentCategory));

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpGet]
        public IActionResult create()
        {
            var viewModel = new CreateCategoryVM
            {
                ParentCategories = unitOfWork.CategoryRepo.GetAll().Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
            };

            return View(viewModel);
        }


        [HttpPost]
        public IActionResult Create(CreateCategoryVM model)
        {
            if (ModelState.IsValid)
            {
                var category = new Category
                {
                    Name = model.Name,
                    ParentCategoryId = model.ParentCategoryId,
                    UpdatedAt = DateTime.Now,
                    CreatedAt = DateTime.Now
                };
                unitOfWork.CategoryRepo.Add(category);
                unitOfWork.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                model.ParentCategories = unitOfWork.CategoryRepo.GetAll().Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,    
                });
                return View(model);
            }
        }

        public IActionResult Edit(int? id)
        {
            if(id == null)
                return BadRequest();
            var category = unitOfWork.CategoryRepo.GetById(id.Value, q => q.Include(c => c.ParentCategory));
            if (category == null)
                return NotFound();

            var viewModel = new EditCategoryVM
            {
                Id = category.Id,
                Name = category.Name,
                ParentCategoryId = category.ParentCategoryId,
                ParentCategories = unitOfWork.CategoryRepo.GetAll().Where(c => c.Id != category.Id).Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    //Selected = (c.Id == category.ParentCategoryId)
                    
                })
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Edit(EditCategoryVM model)
        {
            if (ModelState.IsValid)
            {
                var category = unitOfWork.CategoryRepo.GetById(model.Id);
                if (category == null)
                    return NotFound();
                category.Name = model.Name;
                category.ParentCategoryId = model.ParentCategoryId;
                category.UpdatedAt = DateTime.Now;
                unitOfWork.CategoryRepo.Update(category);
                unitOfWork.SaveChanges();
                TempData["Success"] = "Category updated successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                model.ParentCategories = unitOfWork.CategoryRepo.GetAll().Where(c => c.Id != model.Id).Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                });
                TempData["Error"] = "Please correct the errors in the form.";
                return View(model);
            }
        }
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            if (unitOfWork.CategoryRepo.GetById(id.Value) == null)
                return NotFound();

            var hasSubCategories = unitOfWork.CategoryRepo.FindAll(c => c.ParentCategoryId == id).Count > 0;
            var hasProducts = unitOfWork.ProductRepo.FindAll(p => p.CategoryId == id).Count > 0;

            if (hasSubCategories || hasProducts)
            {
                TempData["Error"] = "Cannot delete this category because it has "
                    + (hasSubCategories ? "subcategories" : "")
                    + (hasSubCategories && hasProducts ? " and " : "")
                    + (hasProducts ? "products" : "")
                    + " linked to it.";
                return RedirectToAction("Index");
            }

            unitOfWork.CategoryRepo.Delete(id.Value);
            unitOfWork.SaveChanges();
            TempData["Success"] = "Category deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
