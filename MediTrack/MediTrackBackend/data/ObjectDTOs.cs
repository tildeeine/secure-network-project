namespace MediTrack.Data;

public record PhycisianDTO(
    string Speciality,
    string PublicKey
);

public record PatientDTO(
    string Name,
    string NIC,
    string Sex,
    string DateOfBirth,
    string BloodType,
    List<string> KnownAllergies,
    List<Consultation> ConsultationRecords
);

// public record ConsultationDTO(
//     string Date,
//     string MedicalSpeciality,
//     string DoctorName,
//     string Practice,
//     string TreatmentSummary,
//     byte[] PhysicianSignature
// );
