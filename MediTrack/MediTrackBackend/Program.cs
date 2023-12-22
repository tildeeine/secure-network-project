using Microsoft.EntityFrameworkCore;
using MediTrack.Data;
using CryptoLib;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

var rsaServerPrivateKey = File.ReadAllText("../keys/meditrack-server.priv.pem");//@"-----BEGIN PRIVATE KEY-----

Dictionary<string, HashSet<int>> messageIds = new();

string AUTH_SERVER_URL = "https://192.168.0.10:5002";

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure DB                    Server=localhost;Port=1234;Database=My_Mysql_Database;Uid=root;Pwd=root;
var connectionString = "SSL Mode=Required;server=192.168.0.10;user=mysql;password=1234;database=meditrack";
// var connectionString = "server=192.168.1.30;user=mysql;password=1234;database=meditrack";
builder.Services.AddDbContext<MediTrackDb>(opt => opt.UseInMemoryDatabase("MediTrack"));
    // opt.UseMySql(
    //     connectionString,
    //     ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

HttpClientHandler clientHandler = new HttpClientHandler();
clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
{
    // Console.WriteLine(sender);
    // Console.WriteLine(cert);
    if (cert?.Subject == "CN=auth-server")
    {
        // Console.WriteLine($"It matches: {cert}");
        return true;
    }
    return false;
};
using HttpClient client = new HttpClient(clientHandler);

app.MapGet("/", () => {
		Console.WriteLine("Hellow worlds");
			});
app.MapGet("/patients/{patientNIC}", async (
    HttpRequest request,
    MediTrackDb db,
    string patientNIC,
    string doctorNIC,
    bool emergency,
    int id) =>
{
    var response = await client.GetAsync($"{AUTH_SERVER_URL}/{doctorNIC}");

    if (response.StatusCode != HttpStatusCode.OK)
    {
        Console.WriteLine("[Error] Auth Server Request Failed.");
        return Results.StatusCode(500);
    }

    PhycisianDTO? physician = await response.Content.ReadFromJsonAsync<PhycisianDTO>();
    if (physician is null)
    {
        Console.WriteLine("[Error] Auth Server PhysicianDTO failed.");
        return Results.StatusCode(500);
    }

    // Console.WriteLine($"Received {physician}");

    // Ensure authenticity of request using physician public key
    request.EnableBuffering();
    using var reader = new StreamReader(request.Body, Encoding.UTF8);
    reader.BaseStream.Seek(0, SeekOrigin.Begin);
    byte[] data = Convert.FromBase64String(await reader.ReadToEndAsync());

    if (!CryptoLib.Crypto.CheckWithFreshness(
        data,
        [.. Encoding.UTF8.GetBytes(doctorNIC), .. Encoding.UTF8.GetBytes(emergency.ToString()), .. Encoding.UTF8.GetBytes(id.ToString())],
        physician.PublicKey,
        id,
        messageIds.GetValueOrDefault(doctorNIC)))
    {
        Console.WriteLine("[Error]: Message Auth Failed");
        return Results.BadRequest();
    }

    // Message is fresh, so add its id to the dictionary.
    if (!messageIds.ContainsKey(doctorNIC))
        messageIds[doctorNIC] = new();
    messageIds[doctorNIC].Add(id);

    var patient = await db.Patients.Where(v => v.NIC == patientNIC).FirstOrDefaultAsync();

    if (patient is null)
    {
        Console.WriteLine($"[Error]: Failed to get patient with {patientNIC}");
        return Results.BadRequest();
    }

    if (!emergency)
        ProtectClassifiedRecords(patient, physician.Speciality);

    var patientDto = new PatientDTO
    (
        patient.Name,
        patient.NIC,
        Enum.GetName<SexType>(patient.Sex)!,
        patient.BloodType,
        patient.DateOfBirth,
        patient.KnownAllergies,
        patient.ConsultationRecords
    );

    string patientObj = JsonSerializer.Serialize(patientDto, new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    });


    var bytes = Crypto.Protect(JsonNode.Parse($"{{\"patient\": {patientObj}}}")!, physician.PublicKey, rsaServerPrivateKey);
    if (bytes is null)
    {
        Console.WriteLine("[Error]: Protection failed.");
        return Results.StatusCode(500);
    }

    // Console.WriteLine(Encoding.UTF8.GetString(bytes));
    return Results.Bytes(bytes);
});

app.MapGet("/my-info/{NIC}", async (HttpRequest request, MediTrackDb db, string NIC, int id) =>
{
    var patient = await db.Patients.Where(v => v.NIC == NIC).FirstOrDefaultAsync();

    if (patient is null)
    {
        Console.WriteLine($"[Error]: Failed to get patient with {NIC}");
        return Results.BadRequest();
    }

    // Ensure authenticity of request using patient public key
    request.EnableBuffering();
    using var reader = new StreamReader(request.Body, Encoding.UTF8);
    reader.BaseStream.Seek(0, SeekOrigin.Begin);
    byte[] data = Convert.FromBase64String(await reader.ReadToEndAsync());

    if (!CryptoLib.Crypto.CheckWithFreshness(
        data,
        [.. Encoding.UTF8.GetBytes(NIC), .. Encoding.UTF8.GetBytes(id.ToString())],
        patient.PublicKey,
        id,
        messageIds.GetValueOrDefault(NIC)))
    {
        Console.WriteLine("[Error]: Message Auth Failed");
        return Results.BadRequest();
    }

    // Message is fresh, so add its id to the dictionary.
    if (!messageIds.ContainsKey(NIC))
        messageIds[NIC] = new();
    messageIds[NIC].Add(id);

    var patientDto = new PatientDTO
    (
        patient.Name,
        patient.NIC,
        Enum.GetName<SexType>(patient.Sex)!,
        patient.BloodType,
        patient.DateOfBirth,
        patient.KnownAllergies,
        patient.ConsultationRecords
    );

    string patientObj = JsonSerializer.Serialize(patientDto, new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    });


    var bytes = Crypto.Protect(JsonNode.Parse($"{{\"patient\": {patientObj}}}")!, patient.PublicKey, rsaServerPrivateKey);
    if (bytes is null)
    {
        Console.WriteLine("[Error]: Protection failed.");
        return Results.StatusCode(500);
    }

    // Console.WriteLine(Encoding.UTF8.GetString(bytes));
    return Results.Bytes(bytes);
});

void ProtectClassifiedRecords(Patient patient, string PhysicianSpeciality)
{
    using Aes aes = Aes.Create();
    aes.GenerateIV();
    aes.GenerateKey();

    for (int i = 0; i < patient.ConsultationRecords.Count(); ++i)
    {
        var consultation = patient.ConsultationRecords[i];
        if (consultation.MedicalSpeciality != PhysicianSpeciality)
        {
            patient.ConsultationRecords[i] = consultation with
            {
                DoctorName = Crypto.EncryptToBase64(consultation.DoctorName, aes),
                NIC = Crypto.EncryptToBase64(consultation.NIC, aes),
                Date = Crypto.EncryptToBase64(consultation.Date, aes),
                MedicalSpeciality = Crypto.EncryptToBase64(consultation.MedicalSpeciality, aes),
                Practice = Crypto.EncryptToBase64(consultation.Practice, aes), // should be enum?
                TreatmentSummary = Crypto.EncryptToBase64(consultation.TreatmentSummary, aes),
                PhysicianSignature = Crypto.EncryptToBase64(consultation.PhysicianSignature, aes)
            };
        }
    }
}
// app.MapGet("/patients/{id}", async (MediTrackDb db, int id) => await db.Patients.FindAsync(id));
//.WithName("GetWeatherForecast")
//.WithOpenApi();

app.Run();
