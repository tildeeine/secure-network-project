namespace MediTrack.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

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

public record Patient
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
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
        string TreatmentSummary
);
