using System.ComponentModel.DataAnnotations;

namespace WebApplication12.Models
{
	public class CreateRoleViewModel
	{
		[Required]
		[Display(Name = "Role")]
		public string RoleName { get; set; }
		public string Description { get; set; }
	}
}
