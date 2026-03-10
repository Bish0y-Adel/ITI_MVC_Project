using Microsoft.AspNetCore.Identity;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public class User : IdentityUser
    {
        [Required]
        [MaxLength(150)]
        public string FullName { get; set; }


        public ICollection<Address> Addresses { get; set; } = new HashSet<Address>();
        public ICollection<Order> Orders { get; set; } = new HashSet<Order>();
        public Cart? Cart { get; set; }

    }
}
