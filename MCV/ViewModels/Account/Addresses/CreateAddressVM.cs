using System.ComponentModel.DataAnnotations;

namespace MCV.ViewModels.Account.Addresses
{
    public class CreateAddressVM
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
    }
}
