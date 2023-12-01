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

app.MapGet("/patients", async (MediTrackDb db) =>
{
    var patients = await db.Patients.ToListAsync();
    return patients.Select(v =>
    {
        string obj = JsonSerializer.Serialize(v, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        Console.WriteLine(obj);
        JsonNode node = JsonNode.Parse($"{{\"patient\": {obj}}}");
        var bytes =  Crypto.Protect(node, rsaClient.ExportRSAPublicKeyPem(), rsaServer.ExportRSAPrivateKeyPem());
        Console.WriteLine(Encoding.UTF8.GetString(bytes));
        return bytes;
    });

});
// app.MapGet("/patients/{id}", async (MediTrackDb db, int id) => await db.Patients.FindAsync(id));
//.WithName("GetWeatherForecast")
//.WithOpenApi();

app.Run();
