using Microsoft.AspNetCore.Identity;
namespace nails_trail.Models
{
    public class ApplicationUser : IdentityUser

	{
		public string? FullName { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
 
    }
}
