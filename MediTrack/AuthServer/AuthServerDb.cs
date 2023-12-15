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
                    Speciality = "Orthopedic",
            	    PublicKey = @"-----BEGIN RSA PRIVATE KEY-----
MIIEowIBAAKCAQEAw4qQmWBgpIojKUABTgzq9vkn/rUSJFB4zImFf5WgcJu1/GK/
iVzIkDyoUySKYVBzFa7YB/2643asDCkMCiFM1ZNNTp4VJm6t08lSXR+13szUBW/N
y1Mp2JT09NGvs2aXWNqaK+8pDCQhG5STGUcXcurNjPd1AtQFNowOGwdYNXrnarR1
/KbY4UyEAwpHNb67Vb7WPHWZR+ELW+oXIlS6YtqvMjQZXryzy8k/3rWgxLblF7H0
ignwD2e+0EvGMcX01LLsDabK92V8vfsH/8Tf5y0YcuH+2y7xHFvOHEXK2mBU4JKW
VcxqDM6/d8UMaMJ4n0uNxOgEN6vbV6jwKPOiXQIDAQABAoIBACJmScZltVK053ce
Bqy9qoc8sUvGfHTNgFbROZ5GixVMW1FVaOcoHrN7LEyGIN76SWiu29vaB+r0qVMx
VGgi0BLBiWfTeRqL/YIEyHvP9I2HwKPdNZm3nWS33bkE+3Em0ujDejxQ5Ep5v6ZL
tq4LeuVRR8xtCxVuiaU9Mc0Q93RexkQzfuKU40tN2nohsSBqRFKt6z8tKkESG3QW
bfgbExrSUq4QN3LyPqpHrsCkb4dBaPFVU27adw7aXSvXrhJd6LPwvuD7K5A3gGBQ
U8VdCSj8su99WnlWYg3GbbrbWSOQfhMERu8KiL3lR6R236FSs4Xp/CV8cmfphMIz
TRU4o6kCgYEA468JyX6tqd0csf3PyZ/X+X8IKKNeSmek1sjgoxAZ2EJRtJDCqcEq
W0jHDtMLR/xrIQeiMEP8TyWQFfgKJl7KP7xFdF4G2ejBfFqdTiE0OER6+q60jBoJ
CEbamE0qDQ1bDpr2to77xt2Khp3pyY1yYAxkPe/rEyz+iEe5yMxyr2kCgYEA29wt
4K0eS6Z0BqrY6FpeRFzmhOUukZm2KCs75oi5CGu+Mob0n/TXe3G7Oy4iMPRfgEz2
V6EN/cmQhCF5HASQ6fCOIlrls4gE/mkXIBPZSllL9gywz20DZStnHrLyJxW+HKTe
eMTYVgl6zLRi31UNXTXqvEuoSL8jrmJRFHcEMNUCgYEAr3wpvvO0Rkf15foKPQzG
GbafQzMnaZGYqzMIcKXoRZZYAyPP5abMTq85PWPMPh9/MmX/y9OalCYPd99Fc217
1MP3hEk0Xp/XAVGWDLHq3bNqVexxm8o1S/vQX7KZxpWUqR0nKm3qAMygCEGN/5rV
nNHwMQzJ0m3LgJ5Hw0xQ2IECgYA2XD5LqwIumj1d2KhA6vcb2Ax3F2NoRKUxUV9C
JNP7LQid3ZhmZFdTAk/U1hdGG2XxhxXxnLSk502AHfaf4tEhxoSeIfNhbGQvszQw
jjfPljyd3Y1p+/426YBZqs0MtOeIlYscOS90NmvSaVd4+MKRJV+gpuQ4+GmOGGkL
slGq8QKBgGwRRf74pEABKTMgFsflzFPVW1ScmkwXJtJgIzdH7Htq1CqeA2zPHVy7
OzP8MqJi3G+3QLOET+WcUqyQ7cJqhBAQr1SXvLqPKvXeZZ6B1ZNbr4vQmhaLPJ3r
EM4DbpQ6dOUDpnjRX0sNEsQrzupvgne//qJORLLYxwnzAJGupimn
-----END RSA PRIVATE KEY-----",
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
    public string Speciality {get; init; } = null!;
    public string PublicKey { get; init; } = null!;
}
