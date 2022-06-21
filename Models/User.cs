#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WeddingPlanner.Models;

public class User
{
    [Key]
    public int UserID {get; set;}
    [Required]
    [MinLength(2, ErrorMessage="First name must be at least 2 characters")]
    public string FirstName {get; set;}[Required]
    [MinLength(2, ErrorMessage="Last name must be at least 2 characters")]
    public string LastName {get; set;}
    [EmailAddress]
    [Required]
    public string Email {get; set;}
    [DataType(DataType.Password)]
    [Required]
    [MinLength(8, ErrorMessage="Must be at least 8 characters")]
    public string Password {get; set;}
    [DataType(DataType.Password)]
    [NotMapped]
    [Compare("Password")]
    public string ConfirmPass {get; set;}
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt {get; set;}
    // one to many nav property for related Wedding objs
    public List<Wedding> RSVPs {get; set;} = new List<Wedding>();
    // many to many nav property
    public List<Guest> Planners {get; set;} = new List<Guest>();
}