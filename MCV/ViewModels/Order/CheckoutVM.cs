using MCV.ViewModels.Cart;

using Microsoft.AspNetCore.Mvc.Rendering;

using System.ComponentModel.DataAnnotations;

namespace MCV.ViewModels.Order
{
    public class CheckoutVM
    {
        public ICollection<CartItemVM> Items { get; set; } = [];
        public decimal Total => Items.Sum(i => i.LineTotal);
        public int ItemCount => Items.Sum(i => i.Quantity);



        [Required(ErrorMessage = "Please select a shipping address.")]
        [Display(Name = "Shipping Address")]
        public int? ShippingAddressId { get; set; }


        public IEnumerable<SelectListItem> Addresses { get; set; } = [];
    }
}
