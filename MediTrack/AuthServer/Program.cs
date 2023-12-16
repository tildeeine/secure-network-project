using Microsoft.EntityFrameworkCore;
using AuthServer.Data;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AuthServerDb>(opt =>
    opt.UseInMemoryDatabase("AuthServer"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/{nic}", async (AuthServerDb db, string nic) =>
{
    var physician = await db.Physicians.Where(v => v.NIC == nic).FirstOrDefaultAsync();
    if (physician is null)
        return Results.BadRequest("Physician not found");

    var jsonObj = new JsonObject{
        {"publicKey", physician.PublicKey},
        {"speciality", physician.Speciality }
    };
    return Results.Ok(jsonObj);
});
//app.MapGet("/", () => "Hello World!");

app.Run();
