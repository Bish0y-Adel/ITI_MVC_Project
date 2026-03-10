using Entities.Models;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace MCV.ViewModels.ManageOrders
{
    public class OrdersVM
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public string UserName { get; set; }
        public DateTime OrderDate { get; set; }
        public string ShippingAddress { get; set; }
        public OrderStatus StatusNumber { get; set; }
        public IEnumerable<SelectListItem> OrderStatus { get; set; } = [];

    }
}
