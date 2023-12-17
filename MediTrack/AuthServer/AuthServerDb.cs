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
                        Id = -2,
                        Name = "Bob",
                        NIC = "000000000",
                        Speciality = "Orthopedic",
                        PublicKey = @"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEArzlh7J0ffHNn+aq2dijR
LXs4MAGopAZhguwlvlWvWQ0uhPkONoz/Znda4RgAUmPSMbkGdnTT8rr8/7Imlwon
i0n42U0i+UVJg12TObu7pTnVj5xsdsSu865r1d3fdVOSMAkDD183PILI6xzLKjoe
CZtUTlg5oL5khL+cx9ofX3ofShMoYqQKpCK1bzstLQS53r05oFHG9YlbUHnGqGyH
7J+kJgkIqjRHVV5Aoon8Lcd9zn85DP258QBgDyJNWA48ZE6k9XvnOtW587/SWRxW
6DrJfwxvykuAFKS2t4mM/eeAxTiMo0nLLVBuAJ4QYkBqFP9/tB6dtBqRE/gYlINq
pwIDAQAB
-----END PUBLIC KEY-----"
                    },
                    new Physician
                    {
                        Id = -1,
                        Name = "Charlie",
                        NIC = "000000001",
                        Speciality = "Neuro",
                        PublicKey = @"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA3a3YwY3Dspq+soIF+wOj
kLoI2vxxmbDeYW1PnjFpd1bjUcDu4ZDw9DMcdl3KmAR2U2aesEv4PZmG+Lp/CxA1
IExoI8DW812Hj0WkR+H5TuHGvg7z/lYpTTK9sD+9GbZrlFYMU4C7Jyf+FASgA0t5
f6DPFGKK0NQzQB7Z9K4Kg+LrxXhEZU8zy9lLplHUBBS30FTwkjK3LbaX6KkrQyYF
CIa2EEi7D3Rv2Q6gqu+LvHZD/RYKZT5sN26Idy13vHsEvsQhklxYakT8RYiWjsai
YwdrAWHyaujddoxNPYqftaWKoW6L4lKCm9HMH1IJIdgJaYA9dzLzTPdfqjEbEdEr
aQIDAQAB
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
