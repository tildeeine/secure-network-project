using Microsoft.EntityFrameworkCore;
using MediTrack.Data;
using CryptoLib;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// TODO: automate this
var rsaServer = RSA.Create();
var rsaClient = RSA.Create();
HashSet<int> messageIds = new();

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

app.MapGet("/patients/{nic}", async (MediTrackDb db, string nic) =>
{
    var patient = await db.Patients.Where(v => v.NIC == nic).FirstOrDefaultAsync();

    if (patient is null)
        return Results.BadRequest();

    string patientObj = JsonSerializer.Serialize(patient, new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    });

    Console.WriteLine(patientObj);
    JsonNode node = JsonNode.Parse($"{{\"patient\": {patientObj}}}")!;

    var bytes = Crypto.Protect(node, rsaClient.ExportRSAPublicKeyPem(), rsaServer.ExportRSAPrivateKeyPem());
    if (bytes is null)
    {
        Console.WriteLine("[Error]: Protection failed.");
        return Results.StatusCode(500);
    }

    // Console.WriteLine(Encoding.UTF8.GetString(bytes));
    return Results.Bytes(bytes);
});
// app.MapGet("/patients/{id}", async (MediTrackDb db, int id) => await db.Patients.FindAsync(id));
//.WithName("GetWeatherForecast")
//.WithOpenApi();

app.Run();
