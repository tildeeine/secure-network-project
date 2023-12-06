using System.Text;
using System.Text.Json.Nodes;
using CryptoLib;

const string TOOL_NAME = "Client";
const string HELP = @$"{TOOL_NAME} Commands: 
    help
    start [private-key-file-path public-key-file-path]";

const string HELP_COMMAND = @$"Api Commands: 
    help
    get [patientId]";

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
        while ((command = Console.ReadLine()) != "quit")
        {
            if (command is null)
            {
                DisplayCommandHelp();
                continue;
            }
            string[] command_args = command.Split(' ');
            switch (command)
            {
                case "get":
                    GetPatient(command_args[1..]);
                    break;
                default:
                    DisplayCommandHelp();
                    break;
            }
        }
        break;
    case "help":
    default: DisplayHelp(); return;
}

async void GetPatient(string[] strings)
{
    var response = await client.GetAsync($"{HOST}/patients");
    var signedData = await response.Content.ReadAsByteArrayAsync();
    var data = CryptoLib.Crypto.Unprotect(signedData, privateKey);

    if (data is null)
    {
        Console.WriteLine("[ERROR]: Unprotect was not successfull");
        return;
    }

    if (CryptoLib.Crypto.Check(signedData, data, serverPublicKey))
    {
        Console.WriteLine(data);
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
