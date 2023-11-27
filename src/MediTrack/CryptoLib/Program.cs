using CryptoLib;

const string TOOL_NAME = "CryptoLib";
const string HELP = @$"{TOOL_NAME} Commands: 
    help
    protect (input-file) (output-file) (encryptionKey) (authKey)
    unprotect (input-file) (output-file) (decryptionKey)
    check (input-file) (authKey)";

if (args.Length < 1)
{
    DisplayHelp();
    return;
}

byte[]? inputFile = null;
switch (args[0])
{
    case "protect":
    case "unprotect":

        int length = args.Length;
        if ((args[0] == "protect" && length != 5)
            || (args[0] == "unprotect" && length != 4))
        {
            DisplayHelp();
            return;
        }

        inputFile = ParseFile(args[1]);
        if (inputFile is null)
        {
            DisplayHelp();
            return;
        }
        byte[]? bytes = null;
        if (args[0] == "protect")
        {
            Console.WriteLine($"Protecting file {inputFile}");
            bytes = Crypto.Protect(inputFile, args[3], args[4]);
        }
        else
        {
            Console.WriteLine($"Unprotecting file {inputFile}");
            bytes = Crypto.Unprotect(inputFile, args[3]);
        }
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
        inputFile = ParseFile(args[1]);
        if (inputFile is null)
        {
            DisplayHelp();
            return;
        }
        Console.WriteLine(Crypto.Check(inputFile[..256], inputFile[256..], args[2]));
        break;
    case "help":
    default: DisplayHelp(); return;
}

byte[]? ParseFile(string inputFile)
{
    try
    {
        return File.ReadAllBytes(inputFile);
    }
    catch (FileNotFoundException)
    {
        Console.WriteLine($"Couldn't find file: \"{inputFile}\"");
    }
    return null;
}

void DisplayHelp()
{
    Console.WriteLine(HELP);
}
