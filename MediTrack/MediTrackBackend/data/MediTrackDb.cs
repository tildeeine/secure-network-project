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

        //   modelBuilder.Entity<Patient>().OwnsMany(m => m.OwnedTypes, ownedType =>
        // {
        //     ownedType.Property<int>("Id");
        //     ownedType.HasKey("Id");
        // });
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
                // new Consultation(
                //         0,
                //         "2023-02-02",
                //         "Mental",
                //         "Joao",
                //         "Clinica",
                //         "Pomade",
                //         "test" // TODO: GENERATE
                //         ),
                // new Consultation(
                //         1,
                //         "2023-02-02",
                //         "Orthopedic",
                //         "Manel",
                //         "Clinica",
                //         "Pomade",
                //         "test" // TODO: GENERATE
                // ,
                PublicKey = "test" // TODO: Generatee
            });

        modelBuilder.Entity<Patient>().OwnsMany(e => e.ConsultationRecords).HasData(
            new
            {
                PatientId = -1, // patient id
                Id = -1,
                Date = "2023-02-02",
                MedicalSpeciality = "Orthopedic", // should be enum?
                DoctorName = "Manel",
                Practice = "Clinica", // should be enum?
                TreatmentSummary = "Pomade",
                PhysicianSignature = "teste"// base64

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
}

[Owned]
public record Consultation(
        string Date,
        string MedicalSpeciality, // should be enum?
        string DoctorName,
        string Practice, // should be enum?
        string TreatmentSummary,
        string PhysicianSignature // base64
);
