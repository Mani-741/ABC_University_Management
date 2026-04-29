using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversityAcademicManagementSystem.Models
{
	public class User
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int UserId { get; set; }

		[Required(ErrorMessage = "Email address is required")]
		[EmailAddress(ErrorMessage = "Invalid Email Address")]
		[MaxLength(120)]
		public string Email { get; set; } = string.Empty;

		[Required(ErrorMessage = "Password is required")]
		[StringLength(300, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
			ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
		[DataType(DataType.Password)]
		public string Password { get; set; } = string.Empty;

		[NotMapped]
		[Required(ErrorMessage = "Confirm Password is required")]
		[Compare("Password", ErrorMessage = "Passwords do not match!")]
		[DataType(DataType.Password)]
		public string? ConfirmPassword { get; set; }

		[Required(ErrorMessage = "Role is required")]
		public Role Role { get; set; } = Role.Student;

		[MaxLength(100)]
		public string? Department { get; set; } = string.Empty;
	}
}