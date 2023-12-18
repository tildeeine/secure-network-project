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

    static string MEDITRACK_HOST = "http://localhost:5171";
    static string AUTH_SERVER_HOST = "http://localhost:5110";

    //     static string privateKey = @"-----BEGIN PRIVATE KEY-----
    // MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDN8JyiQ9eiTGiF
    // 8HZ+haXSByvYlzpOcQfYz0xMdeFau+/i2gf7AL1OLtYAxVRhHHxntsNR5T8hX521
    // pbCuxca66mosjz0gpyIaYC/00Ie46tylG17lBb54ken5bySkRIdkkVao9cAomg/c
    // VOa3HCIeb76G8JeVhbd/QVmuwnmEEscrWXD4+gSnBRK6oIZ+/AJ5iajJ9i29mDNM
    // Tkogm8Rf7WjlFG4g3Wdh3u4DagXVOzjarDSVe8rPpBcteFovsOk1shIyZogrVFvA
    // LqMa8L3dTzpfODtyxLK8uIHlSkDpls17OY2pptETVPIPL39W4PHihNcNrqOQVy1b
    // fW9sYJIbAgMBAAECggEAZRusyz3jsJsy9g+JHbUgJG3A6SXWSozT/W5JV4DIk3OR
    // 4x6IrINAbhIwn1BCjSsfKQxh+ONEi24WUAh5JlWTrFFKr3Xj3RQxeiGfaeK3v+IP
    // UkCN5oNbcHazGPOoWb4LeySgF3QQU97Pyq0kWOJHHgpe0IFu6sorvR6omVSBtIVh
    // Y1GxJ4uja6wBEz/37qJbjh31PCLU4n2ibyhizUeW/wLbCxysfgtNybiW0nDQf9lc
    // PiQdX4Mcb00oaX8Rih2op/nGjcV1q+bBA5czMSAXEwIdGIkwfdxkdmpRdKpuWVDs
    // 8S1HutREFo/9Zhm0AkBs6eK0pcGxMyd9tQ9QQ6HNAQKBgQD5p0Ev1uAnh+C/Huqd
    // CRIugPbTP11IFejW5TH2bD3Xj3ZmpJC09v2bbiavD2yQR9M3lOaGeHlN8Z2kbIIR
    // qzW9xuPm4SwCE5FXS8U6j6wQZFX4vmn2/S+/fRukxQ5cE1cuJSk2mVuK9PlDfXQ+
    // KHJj3pI/29gIOmy5EDdQqXgKMwKBgQDTLN6t5q1xv+0atg2o7kHWo/iFIody6l69
    // r3VLQfgNGJDFSqMbGWP3k24nMAPNAIjAi2cBNhV82RyJfMwPKNTk3SesNkXTevTx
    // iDSGZKuZFHYCZxbBqpA4u1ufpe54usHte0gGzNtizVzsY3cRmnlhwqVUzduEIOPT
    // 7HWPho1AeQKBgDDsaObiGf1FMHLjsSBjBbAdT8FoGnSk7oMmWRssbRYQJCjLORxt
    // hpduB6CoyiKgILE0udRCSatPnQ/6v6aMwbRWBJVbLQ+fHA1aaOUoAJUZxItBbWyc
    // gz3oW4F3qG+8zonZeHEdroXVqf9i12PS80/E7y4afARoxqOhnOVuwHpnAoGAGuip
    // y2EMkuUQ8olmPjN2AkLMpTJcLiF9RxB3kspqMEkEEY/MLuTSXzbTH303zsSVqGtb
    // CcV5gXos77wOSJQ8ZJllt8UGqscNNUXU45cqYow/6Vh3huAUFpaRO0uqkonBsmA2
    // Ml+iSPnAMIMQJhcYBoQGC0NcCH8kaNnFtS9BCokCgYEAv+SfOrD238t7jWc780Um
    // PpZu2iSk3D2nlGJK/gzwb/98t2p/et6cf9/H/CJqRSAzvWzmQjL1+Q5VHDgB9Ogt
    // HqKX44HzZxwOIHZEPWGOarys77DZnFiwNKhGslgRgX36/wPUPDw/dM+tuUu3Neyl
    // JPNYiPCHfFdRRZZApLecZUw=
    // -----END PRIVATE KEY-----";
    //
    //     static string publicKey = @"-----BEGIN PUBLIC KEY-----
    // MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAzfCcokPXokxohfB2foWl
    // 0gcr2Jc6TnEH2M9MTHXhWrvv4toH+wC9Ti7WAMVUYRx8Z7bDUeU/IV+dtaWwrsXG
    // uupqLI89IKciGmAv9NCHuOrcpRte5QW+eJHp+W8kpESHZJFWqPXAKJoP3FTmtxwi
    // Hm++hvCXlYW3f0FZrsJ5hBLHK1lw+PoEpwUSuqCGfvwCeYmoyfYtvZgzTE5KIJvE
    // X+1o5RRuIN1nYd7uA2oF1Ts42qw0lXvKz6QXLXhaL7DpNbISMmaIK1RbwC6jGvC9
    // 3U86Xzg7csSyvLiB5UpA6ZbNezmNqabRE1TyDy9/VuDx4oTXDa6jkFctW31vbGCS
    // GwIDAQAB
    // -----END PUBLIC KEY-----";
    //
    static string serverPublicKey = @"-----BEGIN PUBLIC KEY-----
    MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAs/0S3fRK58o7ykUZI3cm
    f+OswwKgbGb3C/yTUa2QGXIYhJLz034jrr7Pw/KVnaF/4aRCMg/NpDQPlEAS3M8m
    m3BtQSldNbT4xHVk25poO/HA0IcRYClupnsbedD9u0SiOM7WOvQnVewbC7BBl2oD
    TUtRrADL62Wq/A92RSwBMrVo/YhYbSvwqdsOOij41rWZlBRL31H1v2MHEICqFibT
    sIYgTRR4xnpjsJsWqAJpZWrbraFxVpk/zQ4V7/Vdg6dvPhRgSCNSpzWILR12qDoB
    K7FlN82qGCKm3MNehyPkASftMb/2VNvWfqmiikBw7H0+ZPyKt7d82HYTJomC30gb
    vwIDAQAB
    -----END PUBLIC KEY-----";

    static int messageId = 0;

    private static async Task Main(string[] args)
    {
        // TODO: REMOVE THIS ENABLE SSL
        HttpClientHandler clientHandler = new HttpClientHandler();
        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
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
                                await GetPatient(command_args[1], NIC, privateKey, publicKey, client, false);
                                break;
                            case "emergency": // Doctor Command
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

