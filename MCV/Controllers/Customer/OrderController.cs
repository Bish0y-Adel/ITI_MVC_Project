using Entities.Models;
using Entities.Repositories;

using MCV.ViewModels.Cart;
using MCV.ViewModels.Order;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MCV.Controllers.Customer
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork UnitOfWork;
        private readonly UserManager<User> UserManager;

        public OrderController(IUnitOfWork _unitOfWork, UserManager<User> _userManager)
        {
            UnitOfWork = _unitOfWork;
            UserManager = _userManager;
        }

        public IActionResult Index()
        {
            var userId = UserManager.GetUserId(User);

            var orderr= UnitOfWork.OrderRepo
                .Query()
                    .Include(s=>s.ShippingAddress)
                    .Include(o => o.OrderItems)
                        .ThenInclude(p => p.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orderr);
        }

        public IActionResult Checkout()
        {
            var userId = UserManager.GetUserId(User)!;
            var cart = UnitOfWork.CartRepo
                        .Query()
                            .Include(c => c.CartItems)
                                .ThenInclude(ci => ci.Product)
                        .FirstOrDefault(c => c.UserId == userId);

            

            if (cart == null || cart.CartItems.Count == 0)
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            var vm = BuildCheckoutVM(cart, userId);

            //Default Address is selected
            vm.ShippingAddressId = UnitOfWork.AddressRepo
                .FindAll(a => a.UserId == userId && a.IsDefault)
                .Select(a => a.Id).FirstOrDefault();

            return View(vm);
        }


        [HttpPost]
        public IActionResult PlaceOrder(CheckoutVM model)
        {
            var userId = UserManager.GetUserId(User)!;
            
            var cart = UnitOfWork.CartRepo
                        .Query()
                            .Include(c => c.CartItems)
                                .ThenInclude(ci => ci.Product)
                        .FirstOrDefault(c => c.UserId == userId);

            if (cart == null || cart.CartItems.Count == 0)
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            if (!ModelState.IsValid)
            {
                var vm = BuildCheckoutVM(cart, userId);
                vm.ShippingAddressId = model.ShippingAddressId;
                return View("Checkout", vm);
            }

            // Verify the address belongs to this user
            var address = UnitOfWork.AddressRepo.GetById(model.ShippingAddressId!.Value);
            if (address == null || address.UserId != userId)
            {
                ModelState.AddModelError("ShippingAddressId", "Invalid address selected.");
                var vm = BuildCheckoutVM(cart, userId);
                return View("Checkout", vm);
            }

            // Stock validation
            foreach (var item in cart.CartItems)
            {
                if (item.Quantity > item.Product.StockQuantity)
                {
                    TempData["Error"] = $"\"{item.Product.Name}\" only has {item.Product.StockQuantity} items in stock.";
                    return RedirectToAction("Checkout");
                }
            }

            // Create the order using a transaction to ensure data integrity
            using var transaction = UnitOfWork.BeginTransaction();
            try
            {
                var order = new Order
                {
                    UserId = userId,
                    ShippingAddressId = model.ShippingAddressId.Value,
                    OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}",
                    Status = OrderStatus.Pending,
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = cart.CartItems.Sum(ci => ci.UnitPrice * ci.Quantity)
                };

                UnitOfWork.OrderRepo.Add(order);
                UnitOfWork.SaveChanges();

                foreach (var cartItem in cart.CartItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = cartItem.ProductId,
                        UnitPrice = cartItem.UnitPrice,
                        Quantity = cartItem.Quantity,
                        LineTotal = cartItem.UnitPrice * cartItem.Quantity
                    };
                    UnitOfWork.OrderItemRepo.Add(orderItem);

                    var product = UnitOfWork.ProductRepo.GetById(cartItem.ProductId)!;
                    product.StockQuantity -= cartItem.Quantity;
                    UnitOfWork.ProductRepo.Update(product);
                }

                foreach (var item in cart.CartItems.ToList())
                {
                    UnitOfWork.CartItemRepo.Delete(item.Id);
                }

                UnitOfWork.SaveChanges();
                transaction.Commit();

                TempData["Success"] = $"Order #{order.OrderNumber} placed successfully!";
                return RedirectToAction("Details", new { id = order.Id });
            }
            catch
            {
                transaction.Rollback();
                TempData["Error"] = "Something went wrong while placing your order.";
                return RedirectToAction("Checkout");
            }

            
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
                return BadRequest();

            var userId = UserManager.GetUserId(User);

            var order = UnitOfWork.OrderRepo
                .Query().Include(o => o.ShippingAddress)
                              .Include(o => o.OrderItems)
                                  .ThenInclude(oi => oi.Product)
                .FirstOrDefault(o => o.Id == id && o.UserId == userId);

            if (order == null)
                return NotFound();

            return View(order);
        }

        private CheckoutVM BuildCheckoutVM(Cart cart, string userId)
        {
            return new CheckoutVM
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
                }).ToList(),

                Addresses = UnitOfWork.AddressRepo.FindAll(a => a.UserId == userId)
                    .Select(a => new SelectListItem
                    {
                        Value = a.Id.ToString(),
                        Text = $"{a.Street}, {a.City}, {a.Country}" + (a.IsDefault ? " (Default)" : "")
                    })
            };
        }


    }
}
