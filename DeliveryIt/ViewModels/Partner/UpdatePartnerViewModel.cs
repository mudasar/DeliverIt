using System.ComponentModel.DataAnnotations;

namespace DeliverIt.ViewModels.Partner
{
    public class UpdatePartnerViewModel
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(100, ErrorMessage = "Invalid Length", MinimumLength = 1)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(50, ErrorMessage = "Invalid Length", MinimumLength = 5)]
        public string Password { get; set; }

        [Required()]
        public int Id { get; set; }
    }
}