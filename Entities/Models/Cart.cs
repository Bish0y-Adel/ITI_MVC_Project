using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Models
{
    public class Cart : BaseEntity
    {

        //===============Foreign Keies==================
        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        public ICollection<CartItem> CartItems { get; set; } = new HashSet<CartItem>();
    }
}
