using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using CryptoLib;
using System.Net;
using System.Net.Http.Json;
using System.Diagnostics;

record PhycisianDTO(
    string Speciality,
    string PublicKey
);

record ConsultationDTO(
    string Date,
    string NIC,
    string MedicalSpeciality,
    string DoctorName,
    string Practice,
    string TreatmentSummary

// HACK: Physician Signature ins't part of ConsultationDTO, to allow for easier serialization.
// string PhysicianSignature
);

enum Mode
{
    Doctor,
    Patient
}

class Program
{
    const string TOOL_NAME = "MediTrack Client";
    const string HELP = @$"{TOOL_NAME} Commands: 
    help
    start mode=(doctor|patient) civil-number(NIC) private-key-file public-key-file";

    const string HELP_COMMAND_DOCTOR = @$"Doctor Commands: 
    help
    get patient-civil-number(NIC) Attempt to get patient info
    emergency patient-civil-number(NIC) Emergency situation, so the physician is able to get all medical records
    change Changes to patient mode";

    const string HELP_COMMAND_PATIENT = @$"Patient Commands: 
    help
    get Get All patient info
    change Changes to doctor mode";

    //static string MEDITRACK_HOST = "https://localhost:5001";
	//static string MEDITRACK_HOST = "https://192.168.2.10:5001";
    static string MEDITRACK_HOST = "https://192.168.2.10";
    static string AUTH_SERVER_HOST = "https://localhost:5002";

    static string serverPublicKey = null!; 

    static int messageId = 0;

    private static async Task Main(string[] args)
    {
        serverPublicKey = File.ReadAllText("../keys/meditrack-server.pub.pem");

        HttpClientHandler clientHandler = new HttpClientHandler();
        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { 
            Console.WriteLine(sender);
            Console.WriteLine(cert);
            if (cert?.Subject == "CN=meditrack-server" || cert?.Subject == "CN=auth-server")
            {
                Console.WriteLine($"It matches: {cert}");
                return true;
            }
            return false; 
        };
        // Pass the handler to httpclient(from you are calling api)
        using HttpClient client = new HttpClient(clientHandler);

        if (args.Length < 1)
        {
            DisplayHelp();
            return;
        }

        switch (args[0])
        {
            case "start":
                int length = args.Length;
                if (args.Length != 5)
                {
                    DisplayHelp();
                    return;
                }

                Mode mode = Enum.Parse<Mode>(args[1], true); // Parse client mode
                string NIC = args[2];
                string privateKey = File.ReadAllText(args[3]);
                string publicKey = File.ReadAllText(args[4]);

                string? command;
                DisplayCommandHelp(mode);
                while ((command = Console.ReadLine()) != "quit")
                {
                    if (command is null)
                    {
                        DisplayCommandHelp(mode);
                        continue;
                    }

                    string[] command_args = command.Split(' ');
                    if (mode == Mode.Doctor)
                    {
                        switch (command_args[0])
                        {
                            case "get": // Doctor Command
                                if (command_args.Length != 2)
                                    break;
                                await GetPatient(command_args[1], NIC, privateKey, publicKey, client, false);
                                break;
                            case "emergency": // Doctor Command
                                if (command_args.Length != 2)
                                    break;
                                await GetPatient(command_args[1], NIC, privateKey, publicKey, client, true);
                                break;
                            case "change":
                                mode = Mode.Patient;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (command_args[0])
                        {
                            case "get": // Patient Command
                                await GetMyInfo(NIC, privateKey, publicKey, client);
                                break;
                            case "change":
                                mode = Mode.Doctor;
                                break;
                            default:
                                break;
                        }
                    }
                    DisplayCommandHelp(mode);
                }
                break;
            case "help":
            default: DisplayHelp(); return;
        }
    }

    private static async Task GetMyInfo(string nic, string privateKey, string publicKey, HttpClient client)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{MEDITRACK_HOST}/my-info/{nic}?id={messageId}"),
            // Sign your request content to prove authenticity
            Content = new StringContent(Convert.ToBase64String(Crypto.SignData(
            [
                .. Encoding.UTF8.GetBytes(nic),
                .. Encoding.UTF8.GetBytes(messageId.ToString())
            ],
                privateKey)))
        };

        var response = await client.SendAsync(request);
        messageId++;

        if (response.StatusCode != HttpStatusCode.OK)
        {
            Console.WriteLine("[Error] Bad Request.");
            return;
        }

        var encryptedData = await response.Content.ReadAsByteArrayAsync();
        var data = Crypto.Unprotect(encryptedData, privateKey);


        if (data is null)
        {
            Console.WriteLine("[ERROR]: Unprotect was not successfull");
            return;
        }

        // Check message authenticity
        if (Crypto.Check(data[..256], data[256..], serverPublicKey))
        {
            var node = JsonNode.Parse(data[256..]);
            Debug.Assert(node is not null, "Node is null");

            Console.WriteLine($"Received Data: {node}");

            await CheckConsultationRecordAuth(node, null, client);
        }
        else
        {
            Console.WriteLine("[Error]: Data was tampered with.");
            Console.WriteLine($" Data: {Encoding.UTF8.GetString(data[256..])}\n {Convert.ToBase64String(data[256..])}");
            // Console.WriteLine(Encoding.UTF8.GetString(encryptedData));
            // Console.WriteLine("[Error]: Data was tampered with.");
        }
    }

    private static async Task GetPatient(string patientNIC, string doctorNIC, string privateKey, string publicKey, HttpClient client, bool emergency)
    {

        // var doctorNIC = args[0];
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{MEDITRACK_HOST}/patients/{patientNIC}?doctorNIC={doctorNIC}&emergency={emergency}&id={messageId}"),
            // Sign your request content to prove authenticity
            Content = new StringContent(Convert.ToBase64String(Crypto.SignData(
            [
                .. Encoding.UTF8.GetBytes(doctorNIC),
                .. Encoding.UTF8.GetBytes(emergency.ToString()),
                .. Encoding.UTF8.GetBytes(messageId.ToString())
            ], privateKey)))
        };

        var response = await client.SendAsync(request);
        messageId++;

        if (response.StatusCode != HttpStatusCode.OK)
        {
            Console.WriteLine("[Error] Bad Request.");
            return;
        }

        var encryptedData = await response.Content.ReadAsByteArrayAsync();
        var data = Crypto.Unprotect(encryptedData, privateKey);


        if (data is null)
        {
            Console.WriteLine("[ERROR]: Unprotect was not successfull");
            return;
        }

        // Check message authenticity
        if (Crypto.Check(data[..256], data[256..], serverPublicKey))
        {
            var node = JsonNode.Parse(data[256..]);
            Debug.Assert(node is not null, "Node is null");

            string? speciality = null;
            Console.WriteLine($"Received Data: {node}");

            // If we are not in a emergency we only need to check for consultation
            // records with our speciality.
            if (!emergency)
            {
                PhycisianDTO? physician = (await GetPhysicianInfo(doctorNIC, client)); // Get our speciality
                Debug.Assert(physician is not null, $"[Error]: Auth server doesn't have doctor record with {doctorNIC}");
                speciality = physician.Speciality;
            }

            await CheckConsultationRecordAuth(node, speciality, client);
        }
        else
        {
            Console.WriteLine("[Error]: Data was tampered with.");
            Console.WriteLine($" Data: {Encoding.UTF8.GetString(data[256..])}\n {Convert.ToBase64String(data[256..])}");
            // Console.WriteLine(Encoding.UTF8.GetString(encryptedData));
            // Console.WriteLine("[Error]: Data was tampered with.");
        }
    }

    private static void DisplayCommandHelp(Mode mode)
    {
        if (mode == Mode.Doctor)
            Console.WriteLine(HELP_COMMAND_DOCTOR);
        else Console.WriteLine(HELP_COMMAND_PATIENT);
    }

    private static void DisplayHelp()
    {
        Console.WriteLine(HELP);
    }

    private static async Task CheckConsultationRecordAuth(JsonNode node, string? speciality, HttpClient client)
    {
        List<ConsultationDTO>? consultationRecords = JsonSerializer.Deserialize<List<ConsultationDTO>>(node["patient"]!["consultationRecords"], new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        if (consultationRecords is null)
        {
            Console.WriteLine("[Error]: Consultation Records Deserialization failed.");
            return;
        }

        for (int i = 0; i < consultationRecords.Count(); ++i)
        {
            ConsultationDTO consultation = consultationRecords[i];

            // Console.WriteLine($"Checking for {consultationRecords[i]}, {consultationHeader}");
            if (speciality is null || consultation.MedicalSpeciality == speciality)
            {
                // Console.WriteLine($"Checking {speciality} {consultation}");
                var physician = await GetPhysicianInfo(consultation.NIC, client);
                if (physician is null)
                {
                    Console.WriteLine($"[Error:] Skipping physician with NIC {consultation.NIC}");
                    continue;
                }

                var result = Crypto.Check(
                    // HACK: Signature ins't part of ConsultationDTO, to allow for easier serialization.
                    Convert.FromBase64String((node["patient"]!["consultationRecords"]![i]!["physicianSignature"]!).ToString()),
                    JsonSerializer.SerializeToUtf8Bytes(consultation, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    }),
                    physician.PublicKey);
                Console.WriteLine($"Checking auth for consultationRecord: {i}\nResult: {result}");
            }
        }

    }

    private static async Task<PhycisianDTO?> GetPhysicianInfo(string doctorNIC, HttpClient client)
    {
        var response = await client.GetAsync($"{AUTH_SERVER_HOST}/{doctorNIC}");

        if (response.StatusCode != HttpStatusCode.OK)
        {
            Console.WriteLine("[Error] Auth Server Request Failed.");
            return null;
        }
        return await response.Content.ReadFromJsonAsync<PhycisianDTO>();
    }

}

