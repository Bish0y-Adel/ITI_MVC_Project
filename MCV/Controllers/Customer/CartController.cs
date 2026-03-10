using Entities.Models;
using Entities.Repositories;

using MCV.ViewModels.Cart;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MCV.Controllers.Customer
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IUnitOfWork unitOfWork;

        public CartController(UserManager<User> _userManager, IUnitOfWork _unitOfWork)
        {
            userManager = _userManager;
            unitOfWork = _unitOfWork;
        }

        public IActionResult Index()
        {
            var cart = GetOrCreateCart();

            var vm = new CartVM
            {
                Items = cart.CartItems.Select(ci => new CartItemVM
                {
                    CartItemId = ci.Id,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    SKU = ci.Product.SKU,
                    UnitPrice = ci.UnitPrice,
                    Quantity = ci.Quantity,
                    StockQuantity = ci.Product.StockQuantity
                }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult Add(int productId, int quantity = 1)
        {
            var product = unitOfWork.ProductRepo.GetById(productId);
            if (product == null || !product.IsActive)
                return NotFound();

            if (quantity < 1)
                quantity = 1;

            var cart = GetOrCreateCart();

            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId && ci.IsDeleted == false);
            if (existingItem != null)
            {
                var newQty = existingItem.Quantity + quantity;
                if (newQty > product.StockQuantity)
                {
                    TempData["Error"] = $"Only {product.StockQuantity} items available in stock.";
                    return RedirectToAction("Index");
                }
                existingItem.Quantity = newQty;
                unitOfWork.CartItemRepo.Update(existingItem);
            }
            else
            {
                if (quantity > product.StockQuantity)
                {
                    TempData["Error"] = $"Only {product.StockQuantity} items available in stock.";
                    return RedirectToAction("Index");
                }

                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    UnitPrice = product.Price,
                    Quantity = quantity
                };
                unitOfWork.CartItemRepo.Add(cartItem);
            }

            unitOfWork.SaveChanges();
            TempData["Success"] = $"\"{product.Name}\" added to cart.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int cartItemId, int quantity)
        {
            var cartItem = unitOfWork.CartItemRepo.GetById(cartItemId, q => q.Include(ci => ci.Product));
            if (cartItem == null)
                return NotFound();

            if (quantity < 1)
            {
                unitOfWork.CartItemRepo.Delete(cartItemId);
                unitOfWork.SaveChanges();
                return RedirectToAction("Index");
            }

            if (quantity > cartItem.Product.StockQuantity)
            {
                TempData["Error"] = $"Only {cartItem.Product.StockQuantity} items available.";
                return RedirectToAction("Index");
            }

            cartItem.Quantity = quantity;
            unitOfWork.CartItemRepo.Update(cartItem);
            unitOfWork.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Remove(int cartItemId)
        {
            unitOfWork.CartItemRepo.Delete(cartItemId);
            unitOfWork.SaveChanges();
            TempData["Success"] = "Item removed from cart.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Clear()
        {
            var cart = GetOrCreateCart();
            foreach (var item in cart.CartItems.ToList())
            {
                unitOfWork.CartItemRepo.Delete(item.Id);
            }
            unitOfWork.SaveChanges();
            TempData["Success"] = "Cart cleared.";
            return RedirectToAction("Index");
        }

        // ── Helper: get or create the user's single cart ──
        public Cart GetOrCreateCart()
        {
            var userId = userManager.GetUserId(User);

            /*            var cart = unitOfWork.CartRepo
                .Query(c => c.CartItems)
                .Where(c => c.UserId == userId)
                .Select(c => new
                {
                    Cart = c,
                    CartItems = c.CartItems.Select(ci => new { ci, ci.Product }).ToList()
                })
                .AsEnumerable()
                .Select(x => x.Cart)
                .FirstOrDefault();*/

            //var cart = unitOfWork.CartRepo.Query(c => c.CartItems, c => c.CartItems.Select(ci => ci.Product)).Where(c => c.UserId == userId).AsEnumerable().FirstOrDefault();
            var cart = unitOfWork.CartRepo
                        .Query().Include(c => c.CartItems).ThenInclude(ci => ci.Product)
                        .FirstOrDefault(c => c.UserId == userId);

            if (cart != null)
                return cart;

            cart = new Cart { UserId = userId };
            unitOfWork.CartRepo.Add(cart);
            unitOfWork.SaveChanges();
            return cart;
        }
    }
}
