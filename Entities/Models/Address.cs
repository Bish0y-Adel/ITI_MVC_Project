using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public class Address : BaseEntity
    {

        [Required, MaxLength(100)]
        public string Country { get; set; }

        [Required, MaxLength(100)]
        public string City { get; set; }

        [Required, MaxLength(250)]
        public string Street { get; set; }

        [MaxLength(20)]
        public string? Zip { get; set; }

        public bool IsDefault { get; set; }



        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }



        public ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    }
}