namespace MCV.ViewModels.Cart
{
    public class CartVM
    {
        public ICollection<CartItemVM> Items { get; set; } = [];
        public decimal Total => Items.Sum(i => i.LineTotal);
        public int ItemCount => Items.Sum(i => i.Quantity);
    }

}
