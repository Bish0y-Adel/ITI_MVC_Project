using Entities.Models;
using Entities.Repositories;

using MCV.ViewModels.ManageOrders;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace MCV.Controllers.Admin
{
    public class ManageOrdersController : Controller
    {
        private readonly IUnitOfWork UnitOfWork;
        public ManageOrdersController(IUnitOfWork _unitOfWork)
        {
            UnitOfWork = _unitOfWork;
        }
        public IActionResult Index()
        {
            var orders = UnitOfWork.OrderRepo.Query()
                            .Include(o => o.User)
                            .Include(o => o.ShippingAddress)
                            .Include(o => o.OrderItems)
                                .ThenInclude(oi => oi.Product)
                            .ToList();

            var orderVM = orders.Select(o => new OrdersVM
            {
                OrderId = o.Id,
                OrderNumber = o.OrderNumber,
                TotalAmount = o.OrderItems.Sum(oi => oi.Quantity * oi.Product.Price),
                UserName = o.User.UserName!,
                OrderDate = o.OrderDate,
                ShippingAddress = $"{o.ShippingAddress.Street}, {o.ShippingAddress.City}, {o.ShippingAddress.Country}, {o.ShippingAddress.Zip}",
                StatusNumber = o.Status,
                OrderStatus = Enum.GetValues<OrderStatus>().Select(s => new SelectListItem
                {
                    Text = s.ToString(),
                    Value = ((int)s).ToString(),
                }).ToList()
            }).ToList();

            return View(orderVM);
        }

        public IActionResult Details(int id)
        {
            var order = UnitOfWork.OrderRepo.Query()
                            .Include(o => o.User)
                            .Include(o => o.ShippingAddress)
                            .Include(o => o.OrderItems)
                                .ThenInclude(oi => oi.Product)
                            .FirstOrDefault(o => o.Id == id);
            if (order == null)
                return NotFound();
            return View(order);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int orderId, OrderStatus statusNumber)
        {
            var order = UnitOfWork.OrderRepo.GetById(orderId);
            if (order == null)
                return NotFound();
            order.Status = statusNumber;


            UnitOfWork.OrderRepo.Update(order);
            UnitOfWork.SaveChanges();

            TempData["Success"] = $"Order status updated successfully";

            return RedirectToAction("Index");
        }

    }
}
