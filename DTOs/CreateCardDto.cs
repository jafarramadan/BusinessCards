using System.ComponentModel.DataAnnotations;

namespace BusinessCards.DTOs
{
    public class CreateCardDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string Gender { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [StringLength(1048576)] // Max 1MB base64 string
        public string? Image { get; set; }

        [Required]
        [StringLength(500)]
        public string Address { get; set; } = string.Empty;
    }
}
