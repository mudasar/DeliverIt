using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverIt.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string LastName { get; set; }
        [NotMapped]
        public string Name => $"{FirstName} {LastName}";
        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Address { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 1)]
        public string Phone { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string Password { get; set; }
    }
}
