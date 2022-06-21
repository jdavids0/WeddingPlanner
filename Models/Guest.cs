#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WeddingPlanner.Models;

public class Guest
{
    [Key]
    public int GuestID {get; set;}
    public int UserID {get; set;}
    public int WeddingID {get; set;}
    public User? User {get; set;}
    public Wedding? Wedding {get; set;}
}