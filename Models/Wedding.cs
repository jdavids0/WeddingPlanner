#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WeddingPlanner.Models;

public class Wedding
{
    [Key]
    public int WeddingID {get; set;}
    [Required]
    public string WedderOne {get; set;}
    [Required]
    public string WedderTwo {get; set;}
    [Required]
    public DateTime Date {get; set;}
    [Required]
    public string Address {get; set;}
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt {get; set;}
    // foreign key for User
    public int CreatorID {get; set;}
    // one to many nav property for related User obj
    public User? Creator {get; set;}
    public List<Guest> Attendees {get; set;} = new List<Guest>();
    
}