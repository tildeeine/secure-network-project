using Microsoft.EntityFrameworkCore;
using MediTrack.Data;
using CryptoLib;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// TODO: automate this
var rsaServerPrivateKey = @"-----BEGIN PRIVATE KEY-----
MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCz/RLd9ErnyjvK
RRkjdyZ/46zDAqBsZvcL/JNRrZAZchiEkvPTfiOuvs/D8pWdoX/hpEIyD82kNA+U
QBLczyabcG1BKV01tPjEdWTbmmg78cDQhxFgKW6mext50P27RKI4ztY69CdV7BsL
sEGXagNNS1GsAMvrZar8D3ZFLAEytWj9iFhtK/Cp2w46KPjWtZmUFEvfUfW/YwcQ
gKoWJtOwhiBNFHjGemOwmxaoAmllatutoXFWmT/NDhXv9V2Dp28+FGBII1KnNYgt
HXaoOgErsWU3zaoYIqbcw16HI+QBJ+0xv/ZU29Z+qaKKQHDsfT5k/Iq3t3zYdhMm
iYLfSBu/AgMBAAECggEAKQHiUA5uGIRTfgCjOxjU/TNOs7l/+mK1K5Z2rBfKqnl7
y4y9n7RwBcq0hHTbEKhwfeTSLhy6ogw4dnLouUU7CxQg2EVH1souGXiTq5Lw56Nr
Zjc3xrx52NVYi7cJcCaxfRbm6VSiN6nM+atFLm25Zd1gtLsTusSVocsWC6l8dmRK
/bGAIm6EYPqXjE64JdgeYawnYcQJe5wEdZFoZiVyIwnGsyX5m+dwqOf2/v+dyNy/
QtiXqruKvCsbrHL5XnQ8t615v1UlGBuOJ5X6A+Ydm90OtniGL/KR87g87LdtRCCl
2r8PslkowBYnNwjUkRgAVyIEaqc9NQWxBn68Rozh0QKBgQDzLUXyxoEPq5YF7eec
b99ebpyVrrwg1U8X7zjgkjb/7LijmF4R4TeE2U48Jh6cheMQCaZCREtvLSq0DuKy
fJMX+2EVbegOkJiUyVylkvE59uEwbRXv4aL3ZMq2kaj+DTssIJWebS84/HfGyUM2
1aMIPqipATPmAJgwmt3D24NPjwKBgQC9eszqB0kLXfksj6orOpb2VrCxKLv5l++0
o3SwvW9jsnQW1yUpnOrN5Vb85psilMlUeQ+wY++llBqOurv4GHILMJeLaBLZFRFG
WgnPRseQpI+ybf5hCtxFGtqQb9IL4GXvYJ3Zl00ZV6jmN7ed22sP1kWOZOJaEYS2
kvqOkRVY0QKBgQCCvY2E6EqNRUBEcdL7XX5nQ+r14tsMgAKlKJ2Yx6PAVuIyOoIg
9MgnqbmRRyFgH++jOLzlldhErrDt267wLV/cHe/lWJDR+9W88MHZ1zXQZzZNFekc
bmByyALgw1FrPWvZ3q7yXVttNPekraJwgc3EagO90YK4nwsz3p9qZMOlawKBgFeY
4AcJmzFQHpQFTuxxR71W0pT0egKnxT/DmXzj9w0mQRkWGa4lR0As2IxOqEXSd0lA
NoqbiP0JfUWi+qd11bNqoYTndl85qCTYF1TmKfiqu2wIAeQnOzIPeh/wMjEfasDB
7/faROCEcCcOdyrZd6N4setkpGhzVOF7Da6/UBkhAoGAfPxYEMsJVTzqAT3J4M/T
PpsqM8NgVcNAA9TPAzKPq6vN2svbOhj1zEm+DO2zUgRNWwO1GW6x6v3iP1vwI8KL
5GFuTEC0kSbwon0NmRn6E1UkgfGbXUqMIHJVy/MvmjwiXzWMKYroce8HHU0HKWsg
cgwvI9KD55ah77I6u7ZG/Yo=
-----END PRIVATE KEY-----";

var rsaServerPublicKey = @"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAs/0S3fRK58o7ykUZI3cm
f+OswwKgbGb3C/yTUa2QGXIYhJLz034jrr7Pw/KVnaF/4aRCMg/NpDQPlEAS3M8m
m3BtQSldNbT4xHVk25poO/HA0IcRYClupnsbedD9u0SiOM7WOvQnVewbC7BBl2oD
TUtRrADL62Wq/A92RSwBMrVo/YhYbSvwqdsOOij41rWZlBRL31H1v2MHEICqFibT
sIYgTRR4xnpjsJsWqAJpZWrbraFxVpk/zQ4V7/Vdg6dvPhRgSCNSpzWILR12qDoB
K7FlN82qGCKm3MNehyPkASftMb/2VNvWfqmiikBw7H0+ZPyKt7d82HYTJomC30gb
vwIDAQAB
-----END PUBLIC KEY-----";


var rsaClientPublicKey = @"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAzfCcokPXokxohfB2foWl
0gcr2Jc6TnEH2M9MTHXhWrvv4toH+wC9Ti7WAMVUYRx8Z7bDUeU/IV+dtaWwrsXG
uupqLI89IKciGmAv9NCHuOrcpRte5QW+eJHp+W8kpESHZJFWqPXAKJoP3FTmtxwi
Hm++hvCXlYW3f0FZrsJ5hBLHK1lw+PoEpwUSuqCGfvwCeYmoyfYtvZgzTE5KIJvE
X+1o5RRuIN1nYd7uA2oF1Ts42qw0lXvKz6QXLXhaL7DpNbISMmaIK1RbwC6jGvC9
3U86Xzg7csSyvLiB5UpA6ZbNezmNqabRE1TyDy9/VuDx4oTXDa6jkFctW31vbGCS
GwIDAQAB
-----END PUBLIC KEY-----";

HashSet<int> messageIds = new();
string AUTH_SERVER_URL = "http://localhost:5110";

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add db TODO: use real db
builder.Services.AddDbContext<MediTrackDb>(opt =>
    opt.UseInMemoryDatabase("MediTrack"));

var app = builder.Build();

//var db = app.Services.GetService<MediTrackDb>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// TODO: REMOVE THIS and enable ssl
HttpClientHandler clientHandler = new HttpClientHandler();
clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
// Pass the handler to httpclient(from you are calling api)
using HttpClient client = new HttpClient(clientHandler);

app.MapGet("/patients/{nic}", async (HttpRequest request, MediTrackDb db, string nic) =>
{
    var response = await client.GetAsync($"{AUTH_SERVER_URL}/{nic}");

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

    Console.WriteLine($"Received {physician}");

    // Ensure authenticity of request using physician public key
    request.EnableBuffering();
    using var reader = new StreamReader(request.Body, Encoding.UTF8);
    reader.BaseStream.Seek(0, SeekOrigin.Begin);
    byte[] data = Convert.FromBase64String(await reader.ReadToEndAsync());

    if (!CryptoLib.Crypto.Check(data, Encoding.UTF8.GetBytes(nic), physician.PublicKey))
    {
        Console.WriteLine("[Error]: Message Auth Failed");
        return Results.BadRequest();
    }

    var patient = await db.Patients.Where(v => v.NIC == nic).FirstOrDefaultAsync();

    if (patient is null)
    {
        Console.WriteLine($"[Error]: Failed to get patient with {nic}");
        return Results.BadRequest();
    }

    Console.WriteLine($"Requested patient: {patient}");

    ProtectClassifiedRecords(patient, physician.Speciality);

    Console.WriteLine($"Requested patient [after]: {patient}");

    var patientDto = new PatientDTO
    (
        patient.Name,
        patient.NIC,
        patient.Sex.ToString(),
        patient.BloodType,
        patient.DateOfBirth,
        patient.KnownAllergies,
        patient.ConsultationRecords
    );

    string patientObj = JsonSerializer.Serialize(patient, new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    });


    var bytes = Crypto.Protect(JsonNode.Parse($"{{\"patient\": {patientObj}}}")!, rsaClientPublicKey, rsaServerPrivateKey);
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

    foreach (var (consultation, id) in patient.ConsultationRecords.Select((v, i) => (v, i)))
    {
        if (consultation.MedicalSpeciality != PhysicianSpeciality)
        {
            patient.ConsultationRecords[id] = consultation with
            {
                Date = Crypto.EncryptToBase64(consultation.DoctorName, aes),
                MedicalSpeciality = Crypto.EncryptToBase64(consultation.MedicalSpeciality, aes),
                DoctorName = Crypto.EncryptToBase64(consultation.DoctorName, aes),
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
