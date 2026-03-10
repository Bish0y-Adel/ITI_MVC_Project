using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public class Order : BaseEntity
    {

        [Required, MaxLength(50)]
        public string OrderNumber { get; set; }

        [Required]
        public OrderStatus Status { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        //=========================================

        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }



        [Required]
        public int ShippingAddressId { get; set; }

        [ForeignKey(nameof(ShippingAddressId))]
        public Address ShippingAddress { get; set; }



        public ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
    }

    public enum OrderStatus
    {
        Pending = 1,
        Processing = 2,
        Shipped = 3,
        Delivered = 4,
        Cancelled = 5
    }
}
