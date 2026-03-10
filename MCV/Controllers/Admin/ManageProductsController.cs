using Entities.Models;
using Entities.Repositories;

using MCV.ViewModels.ManageCategory;
using MCV.ViewModels.ManageProducts;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MCV.Controllers.Admin
{
    public class ManageProductsController : Controller
    {
        IUnitOfWork UnitOfWork;
        public ManageProductsController(IUnitOfWork _unitOfWork)
        {
            UnitOfWork = _unitOfWork;
        }
        public IActionResult Index()
        {
            return View(UnitOfWork.ProductRepo.GetAll(q => q.Include(p => p.Category)));
        }
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = UnitOfWork.ProductRepo.GetById(id.Value);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        public IActionResult Create()
        {
            var viewModel = new CreateProductVM
            {
                Categories = UnitOfWork.CategoryRepo.GetAll().Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
            };
            return View(viewModel);
        }


        [HttpPost]
        public IActionResult Create(CreateProductVM viewModel)
        {
            //enssure SKU is unique
            if (UnitOfWork.ProductRepo.GetAll().Any(p => p.SKU == viewModel.SKU))
            {
                ModelState.AddModelError("SKU", "SKU must be unique.");
            }

            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Name = viewModel.Name,
                    Price = viewModel.Price,
                    CategoryId = viewModel.CategoryId,
                    StockQuantity = viewModel.StockQuantity,
                    IsActive = viewModel.IsActive,
                    SKU = viewModel.SKU,
                };
                UnitOfWork.ProductRepo.Add(product);
                UnitOfWork.SaveChanges();
                TempData["Success"] = "Product created successfully!";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Please correct the errors in the form.";
            viewModel.Categories = UnitOfWork.CategoryRepo.GetAll().Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            });
            return View(viewModel);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = UnitOfWork.ProductRepo.GetById(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            //map product to view model
            var viewModel = new EditProductVM
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                CategoryId = product.CategoryId,
                StockQuantity = product.StockQuantity,
                IsActive = product.IsActive,
                SKU = product.SKU,
                Categories = UnitOfWork.CategoryRepo.GetAll().Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Edit(int? id, EditProductVM viewModel)
        {
            if (id == null)
            {
                return BadRequest();
            }
            if (UnitOfWork.ProductRepo.GetAll().Any(p => p.SKU == viewModel.SKU && p.Id != id.Value))
            {
                ModelState.AddModelError("SKU", "SKU must be unique.");
            }


            if (ModelState.IsValid)
            {
                var product = UnitOfWork.ProductRepo.GetById(id.Value);
                if (product == null)
                {
                    return NotFound();
                }
                else
                {
                    product.Name = viewModel.Name;
                    product.Price = viewModel.Price;
                    product.CategoryId = viewModel.CategoryId;
                    product.StockQuantity = viewModel.StockQuantity;
                    product.IsActive = viewModel.IsActive;
                    product.SKU = viewModel.SKU;
                    UnitOfWork.ProductRepo.Update(product);
                    UnitOfWork.SaveChanges();
                    TempData["Success"] = "Product updated successfully!";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["Error"] = "Please correct the errors in the form.";
                viewModel.Categories = UnitOfWork.CategoryRepo.GetAll().Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                });
                return View(viewModel);
            }
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var product = UnitOfWork.ProductRepo.GetById(id.Value);
            if (product == null)
            {
                return NotFound();
            }
            UnitOfWork.ProductRepo.Delete(id.Value);
            UnitOfWork.SaveChanges();
            TempData["Success"] = "Product deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}
