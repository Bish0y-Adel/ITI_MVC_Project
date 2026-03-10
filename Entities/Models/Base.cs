using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}