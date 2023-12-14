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
        // modelBuilder
        // .Entity<Patient>()
        // .Property(p => p.Sex)
        // .HasConversion<string>(v => v.ToString(), v => v.ToString());

        modelBuilder.Entity<Patient>().HasData(
                new Patient
                {
                    Id = -1,
                    Name = "Bob",
                    NIC = "000000000",
                    Sex = SexType.Male,
                    DateOfBirth = "2001-07-21",
                    BloodType = "O-",
                    KnownAllergies = new() { "Chocolate" },
                    ConsultationRecords = new List<Consultation>() { },
                });

        base.OnModelCreating(modelBuilder);
    }
}

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
}

public record Consultation(
        int Id,
        string Date,
        string MedicalSpeciality, // should be enum?
        string DoctorName,
        string Practice, // should be enum?
        string TreatmentSummary,
        byte[] PhysicianSignature
);
