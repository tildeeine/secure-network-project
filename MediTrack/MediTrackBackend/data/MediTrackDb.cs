namespace MediTrack.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class MediTrackDb : DbContext
{
    public MediTrackDb(DbContextOptions<MediTrackDb> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<Patient> Patients { get; set; } = null!;
    // public DbSet<Consultation> Consultations { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>().HasData(
            new Patient
            {
                Id = -1,
                Name = "Bob",
                NIC = "000000000",
                Sex = SexType.Male,
                DateOfBirth = "2001-07-21",
                BloodType = "O-",
                KnownAllergies = new List<string> { "Chocolate" },
                ConsultationRecords = new List<Consultation>() { },
                PublicKey = "test" // TODO: Generatee
            });

        modelBuilder.Entity<Patient>().OwnsMany(e => e.ConsultationRecords).HasData(
            new
            {
                PatientId = -1, // patient id
                Id = -2,
                Date = "2023-02-02",
                NIC = "000000000",
                MedicalSpeciality = "Orthopedic", // should be enum?
                DoctorName = "Bob",
                Practice = "Clinica", // should be enum?
                TreatmentSummary = "Pomade",
                PhysicianSignature = "e2LsTkrtPjE44NMyJzO9znj6unWOVPNZWvuG6P4eEGQbnP5ZxysBSsqUQNfDaySwXdsW6+iYXVxYQK/ZNbesqaGz8Hrtd1X+bHxSKLdQV1Vkw2Jpvr/MVPnBgVUbHL81KqdNk/g7wTwZ+8LWTf5sOdJGHIVX+QmWu41P3jbw6D1Q2yXzCWfqhHyUtHu8kgqZmUYijfwmF3s4A5zcYQxxqjXRc8XIjjuMT+iEfcIUulBuW5jPH5M8Oav2Lwqx+BQkNSXFzfKCWyfZGtMxMNNwTMxouOu/JSLX/OvjB07UVHVyMvEdcDmnvrQQl6I/ZkRbqzSXgnSejUviueAGmXguvw=="// base64

            },
            new
            {
                PatientId = -1, // patient id
                Id = -1,
                Date = "2023-02-02",
                NIC = "000000001",
                MedicalSpeciality = "Neuro", // should be enum?
                DoctorName = "Charlie",
                Practice = "Clinica", // should be enum?
                TreatmentSummary = "Pomade",
                PhysicianSignature = "LOxx3nCQU7oHRHeiPgQtqWxDiwoZbfOkhBvtyDuLxKx5PwQE/sm9xsaQwVCns55B43Sz0s7FYNx27feGFZThfDg0HJnYMxL7T56XP80GSWwZHeJWevE9QjJNmE6cIG+c11BjVytRnACbC+IUBgoa+NFzNk23w04lseZd/2GJGl9GON7qiUssYTvZY03zHiMHjp79GYS6BcVHCWBFBZMVNEOujVpWBPd4jOlFwlSxjtzSAlgqBolArAHDO1BdWUmUPHVsCKJ9FVRDno0qYn64OgL2kkIPupqZ9TQftM6hi80vZalJKK7xRyijc09lmZs/53DegXJ6wbntzJOIHmUPYQ=="// base64
            });
        base.OnModelCreating(modelBuilder);
    }
}

// public enum MedicalSpeciality
// {
//     Male,
//     Female,
//     Other
// }

public enum SexType
{
    Male,
    Female,
    Other
}

[Index(nameof(NIC), IsUnique = true)]
public record Patient
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    [RegularExpression(@"\d{9}", ErrorMessage = "NIC number should be a 9 digit number.")]
    public string NIC { get; init; } = null!;

    [Column(TypeName = "nvarchar(24)")]
    public SexType Sex { get; init; }

    public string DateOfBirth { get; init; } = null!;
    public string BloodType { get; init; } = null!;// should be enum?
    public List<string> KnownAllergies { get; init; } = new();
    public List<Consultation> ConsultationRecords { get; init; } = new();

    public string PublicKey { get; init; } = null!;

    public override string ToString()
    {
        return $"Name: {Name} NIC: {NIC} Sex: {Sex}, DateOfBirth: {DateOfBirth}, BloodType: {BloodType}," +
               $"{string.Join(' ', KnownAllergies.Select(v => v.ToString()))}\n" +
               $"Consultation Records: {string.Join('\n', ConsultationRecords.Select(v => v.ToString()))}";
    }
};

[Owned]
public record Consultation(
        string Date,
        string NIC,
        string MedicalSpeciality, // should be enum?
        string DoctorName,
        string Practice, // should be enum?
        string TreatmentSummary,
        string PhysicianSignature // base64
);
