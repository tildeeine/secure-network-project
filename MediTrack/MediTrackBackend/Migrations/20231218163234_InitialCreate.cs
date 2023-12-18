using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MediTrackBackend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NIC = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Sex = table.Column<string>(type: "nvarchar(24)", nullable: false),
                    DateOfBirth = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BloodType = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    KnownAllergies = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PublicKey = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Consultation",
                columns: table => new
                {
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NIC = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MedicalSpeciality = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DoctorName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Practice = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TreatmentSummary = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhysicianSignature = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consultation", x => new { x.PatientId, x.Id });
                    table.ForeignKey(
                        name: "FK_Consultation_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "Id", "BloodType", "DateOfBirth", "KnownAllergies", "NIC", "Name", "PublicKey", "Sex" },
                values: new object[] { -1, "O-", "2001-07-21", "[\"Chocolate\"]", "000000000", "Bob", "-----BEGIN PUBLIC KEY-----\nMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEArzlh7J0ffHNn+aq2dijR\nLXs4MAGopAZhguwlvlWvWQ0uhPkONoz/Znda4RgAUmPSMbkGdnTT8rr8/7Imlwon\ni0n42U0i+UVJg12TObu7pTnVj5xsdsSu865r1d3fdVOSMAkDD183PILI6xzLKjoe\nCZtUTlg5oL5khL+cx9ofX3ofShMoYqQKpCK1bzstLQS53r05oFHG9YlbUHnGqGyH\n7J+kJgkIqjRHVV5Aoon8Lcd9zn85DP258QBgDyJNWA48ZE6k9XvnOtW587/SWRxW\n6DrJfwxvykuAFKS2t4mM/eeAxTiMo0nLLVBuAJ4QYkBqFP9/tB6dtBqRE/gYlINq\npwIDAQAB\n-----END PUBLIC KEY-----", "Male" });

            migrationBuilder.InsertData(
                table: "Consultation",
                columns: new[] { "Id", "PatientId", "Date", "DoctorName", "MedicalSpeciality", "NIC", "PhysicianSignature", "Practice", "TreatmentSummary" },
                values: new object[,]
                {
                    { -2, -1, "2023-02-02", "Bob", "Orthopedic", "000000000", "e2LsTkrtPjE44NMyJzO9znj6unWOVPNZWvuG6P4eEGQbnP5ZxysBSsqUQNfDaySwXdsW6+iYXVxYQK/ZNbesqaGz8Hrtd1X+bHxSKLdQV1Vkw2Jpvr/MVPnBgVUbHL81KqdNk/g7wTwZ+8LWTf5sOdJGHIVX+QmWu41P3jbw6D1Q2yXzCWfqhHyUtHu8kgqZmUYijfwmF3s4A5zcYQxxqjXRc8XIjjuMT+iEfcIUulBuW5jPH5M8Oav2Lwqx+BQkNSXFzfKCWyfZGtMxMNNwTMxouOu/JSLX/OvjB07UVHVyMvEdcDmnvrQQl6I/ZkRbqzSXgnSejUviueAGmXguvw==", "Clinica", "Pomade" },
                    { -1, -1, "2023-02-02", "Charlie", "Neuro", "000000001", "LOxx3nCQU7oHRHeiPgQtqWxDiwoZbfOkhBvtyDuLxKx5PwQE/sm9xsaQwVCns55B43Sz0s7FYNx27feGFZThfDg0HJnYMxL7T56XP80GSWwZHeJWevE9QjJNmE6cIG+c11BjVytRnACbC+IUBgoa+NFzNk23w04lseZd/2GJGl9GON7qiUssYTvZY03zHiMHjp79GYS6BcVHCWBFBZMVNEOujVpWBPd4jOlFwlSxjtzSAlgqBolArAHDO1BdWUmUPHVsCKJ9FVRDno0qYn64OgL2kkIPupqZ9TQftM6hi80vZalJKK7xRyijc09lmZs/53DegXJ6wbntzJOIHmUPYQ==", "Clinica", "Pomade" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Patients_NIC",
                table: "Patients",
                column: "NIC",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Consultation");

            migrationBuilder.DropTable(
                name: "Patients");
        }
    }
}
