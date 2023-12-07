using System.Text;
using System.Text.Json.Nodes;
using CryptoLib;
using System.Net;

const string TOOL_NAME = "Client";
const string HELP = @$"{TOOL_NAME} Commands: 
    help
    start [private-key-file-path public-key-file-path]";

const string HELP_COMMAND = @$"Api Commands: 
    help
    get [NIC (patient civil number)]";

using HttpClient client = new();

string HOST = "http://localhost:5171";
string privateKey = "teste1";
string publicKey = "teste2";
string serverPublicKey = "teste2";

if (args.Length < 1)
{
    DisplayHelp();
    return;
}

switch (args[0])
{
    case "start":
        int length = args.Length;
        if (length != 1 && length != 3)
        {
            DisplayHelp();
            return;
        }

        if (args.Length > 1){
            privateKey = ParseFile(args[1]) ?? privateKey;
            publicKey = ParseFile(args[2]) ?? publicKey;
        }

        string? command;
        DisplayCommandHelp();
        while ((command = Console.ReadLine()) != "quit")
        {
            if (command is null)
            {
                DisplayCommandHelp();
                continue;
            }
            string[] command_args = command.Split(' ');
            switch (command_args[0])
            {
                case "get":
                    GetPatient(command_args[1..]);
                    break;
                default:
                    DisplayCommandHelp();
                    break;
            }
            DisplayCommandHelp();
        }
        break;
    case "help":
    default: DisplayHelp(); return;
}

async void GetPatient(string[] args)
{
    if (args.Length != 1){
        Console.WriteLine("[Error] Command get should have one argument.\n");
        return;
    }

    var response = await client.GetAsync($"{HOST}/patients/{args[0]}");

    if (response.StatusCode != HttpStatusCode.OK)
    {
        Console.WriteLine("[Error] Bad Request.");
        return;
    }

    var signedData = await response.Content.ReadAsByteArrayAsync();
    var data = CryptoLib.Crypto.Unprotect(signedData, privateKey);

    if (data is null)
    {
        Console.WriteLine("[ERROR]: Unprotect was not successfull");
        return;
    }

    // Check message authenticity
    if (CryptoLib.Crypto.Check(signedData, data, serverPublicKey))
    {
        Console.WriteLine(data);
        // TODO: check consultation record authenticity.
    }
    else
    {
        Console.WriteLine("Data was tampered with.");
    }
}

string? ParseFile(string inputFile)
{
    try
    {
        return File.ReadAllText(inputFile);
    }
    catch (FileNotFoundException)
    {
        Console.WriteLine($"Couldn't find file: \"{inputFile}\"");
    }
    return null;
}

void DisplayCommandHelp()
{
    Console.WriteLine(HELP_COMMAND);
}

void DisplayHelp()
{
    Console.WriteLine(HELP);
}
