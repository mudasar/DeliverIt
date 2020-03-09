using System.ComponentModel.DataAnnotations;

namespace DeliverIt.ViewModels.User
{
    public class CreateUserViewModel
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(100, ErrorMessage = "Invalid Length", MinimumLength = 1)]
        public string FirstName { get; set; }
        [Required(AllowEmptyStrings = false)]
        [StringLength(100, ErrorMessage = "Invalid Length", MinimumLength = 1)]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required(AllowEmptyStrings = false)]
        [StringLength(100, ErrorMessage = "Invalid Length", MinimumLength = 5)]
        public string Phone { get; set; }
        [Required(AllowEmptyStrings = false)]
        [StringLength(100, ErrorMessage = "Invalid Length", MinimumLength = 5)]
        public string Address { get; set; }
    }
}
