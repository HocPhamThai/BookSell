using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookEcomWeb.Models
{
    public class ApplicationUser : IdentityUser
    {
        public required string Name { get; set; }

        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public int? CompanyID { get; set; }
        [ForeignKey("CompanyID")]
        [ValidateNever]
        public Company? Company { get; set; }
        [NotMapped]
        public string? Role { get; set; } 
    }
}
