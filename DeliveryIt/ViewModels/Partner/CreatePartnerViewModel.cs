using System.ComponentModel.DataAnnotations;

namespace DeliverIt.ViewModels.Partner
{
    public class CreatePartnerViewModel
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(100, ErrorMessage = "Invalid Length", MinimumLength = 1)]
        public string Name { get; set; }

    }
}
