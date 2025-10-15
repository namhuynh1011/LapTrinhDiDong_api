using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LapTrinhDiDong_api.Models
{
  public enum Role
  {
    Admin,
    Patient
  }
  public class User
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; }

    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }
    [Required]
    [MaxLength(255)]
    public string Password { get; set; }
    [MaxLength(255)]
    public string? Address { get; set; }

    public DateTime? BirthDay { get; set; }

    [MaxLength(10)]
    public string? Gender { get; set; }

    [Required]
    [Column(TypeName ="varchar(20)")]
    public Role Role { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
  }
}