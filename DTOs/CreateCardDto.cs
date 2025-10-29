using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace BusinessCard.DTOs
{
    public class CreateCardDto
    {
        public CreateMode Mode { get; set; } = CreateMode.Manual;

        [StringLength(100)]
        public string? Name { get; set; } = string.Empty;

        [StringLength(10)]
        public string? Gender { get; set; } = string.Empty;

        public DateTime? DateOfBirth { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        public string? Phone { get; set; } = string.Empty;

        [StringLength(1048576)] 
        public string? Image { get; set; }

        [StringLength(500)]
        public string? Address { get; set; } = string.Empty;

        public IFormFile? File { get; set; }
    }
}


