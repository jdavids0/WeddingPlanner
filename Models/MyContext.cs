#pragma warning disable CS8618

using Microsoft.EntityFrameworkCore;
namespace WeddingPlanner.Models;
// the MyContext class representing a session with our MySQL database, allowing us to query for or save data
public class MyContext : DbContext 
{ 
    public MyContext(DbContextOptions options) : base(options) { }
    public DbSet<User> Users {get; set;}
    public DbSet<Wedding> Weddings {get; set;}
    public DbSet<Guest> Guests {get; set;}


}