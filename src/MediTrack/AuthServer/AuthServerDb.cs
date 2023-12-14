namespace AuthServer.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class AuthServerDb : DbContext
{
	public AuthServerDb(DbContextOptions<AuthServerDb> options){
	}

	public DbSet<Physician> Physicians {get;set; } = null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
	modelBuilder.Entity<Physician>().HasData(
                new Physician
                {
                    Id = 1,
                    Name = "Bob",
                    NIC = "000000000",
            	    PublicKey = "your_public_key_here"
                });

        base.OnModelCreating(modelBuilder);
    }
}

[Index(nameof(NIC), IsUnique = true)]
public record Physician
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    [RegularExpression(@"\d{9}", ErrorMessage = "NIC number should be a 9 digit number.")]
    public string NIC { get; init; } = null!;
    public string PublicKey { get; init; } = null!;
}
