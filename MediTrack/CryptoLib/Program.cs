using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using CryptoLib;

const string TOOL_NAME = "CryptoLib";
const string HELP = @$"{TOOL_NAME} Usage: 
    help Displays help message
    protect (input-file) (output-file) (encryptionKey-file) (authKey-file) Protects the given file.
    unprotect (input-file) (output-file) (decryptionKey-file) Unprotects the given file.
    sign (input-file) (authKey-file) Signs data and outputs the signed data in base64 format.
    check (input-file) (authKey-file) Checks the integrity of the given file.";
//?Is this enough for help, or should we maybe be more specific? Insetad of authKey for protect, (sender's private key) or (your private key), etc. 

if (args.Length < 1)
{
    DisplayHelp();
    return;
}

byte[]? bytes = null;
switch (args[0])
{
    case "protect":
        if (args.Length != 5)
        {
            DisplayHelp();
            return;
        }
        Console.WriteLine($"Protecting file {args[1]}");
        bytes = Crypto.Protect(JsonNode.Parse(File.ReadAllBytes(args[1]))!, File.ReadAllText(args[3]), File.ReadAllText(args[4]));
        if (bytes is null)
            return;
        File.WriteAllBytes(args[2], bytes);
        break;
    case "unprotect":

        if (args.Length != 4)
        {
            DisplayHelp();
            return;
        }
        Console.WriteLine($"Unprotecting file {args[1]}");
        bytes = Crypto.Unprotect(File.ReadAllBytes(args[1]), File.ReadAllText(args[3]));

        if (bytes is null)
            return;
        File.WriteAllBytes(args[2], bytes);
        break;
    case "check":
        if (args.Length < 3)
        {
            DisplayHelp();
            return;
        }
        byte[] inputFile = File.ReadAllBytes(args[1]);
        Console.WriteLine(Crypto.Check(inputFile[..256], inputFile[256..], File.ReadAllText(args[2])));
        break;
    case "sign":
        if (args.Length != 3)
        {
            DisplayHelp();
            return;
        }
        JsonNode? node = JsonNode.Parse(File.ReadAllText(args[1]));
        Debug.Assert(node is not null);

        var signedData = Crypto.SignData(JsonSerializer.SerializeToUtf8Bytes(node), File.ReadAllText(args[2]));
        Console.WriteLine(Convert.ToBase64String(signedData));
        break;
    case "help":
    default: DisplayHelp(); return;
}

void DisplayHelp()
{
    Console.WriteLine(HELP);
}
