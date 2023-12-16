namespace AuthServer.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class AuthServerDb : DbContext
{
    public AuthServerDb(DbContextOptions<AuthServerDb> options) 
       : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<Physician> Physicians { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Physician>().HasData(
                    new Physician
                    {
                        Id = 1,
                        Name = "Bob",
                        NIC = "000000000",
                        Speciality = "Orthopedic",
                        PublicKey = @"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAzfCcokPXokxohfB2foWl
0gcr2Jc6TnEH2M9MTHXhWrvv4toH+wC9Ti7WAMVUYRx8Z7bDUeU/IV+dtaWwrsXG
uupqLI89IKciGmAv9NCHuOrcpRte5QW+eJHp+W8kpESHZJFWqPXAKJoP3FTmtxwi
Hm++hvCXlYW3f0FZrsJ5hBLHK1lw+PoEpwUSuqCGfvwCeYmoyfYtvZgzTE5KIJvE
X+1o5RRuIN1nYd7uA2oF1Ts42qw0lXvKz6QXLXhaL7DpNbISMmaIK1RbwC6jGvC9
3U86Xzg7csSyvLiB5UpA6ZbNezmNqabRE1TyDy9/VuDx4oTXDa6jkFctW31vbGCS
GwIDAQAB
-----END PUBLIC KEY-----",
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
    public string Speciality { get; init; } = null!;
    public string PublicKey { get; init; } = null!;
}
