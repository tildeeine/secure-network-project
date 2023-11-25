const string TOOL_NAME = "CryptoLib";
const string HELP = @$"{TOOL_NAME} Commands: 
    help
    protect (input-file) (output-file) ...
    unprotect (input-file) (output-file) ...
    check (input-file)";

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
        if (args[0] == "protect")
            Console.WriteLine($"Protecting file {inputFile}");
        else
            Console.WriteLine($"Unprotecting file {inputFile}");
        break;
    case "check":
        if (args.Length < 2)
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
        Console.WriteLine($"Checking file {inputFile}");
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

