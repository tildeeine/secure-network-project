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
    get NIC (patient civil number)";

using HttpClient client = new();

string HOST = "http://localhost:5171";
string privateKey = @"-----BEGIN PRIVATE KEY-----
MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDN8JyiQ9eiTGiF
8HZ+haXSByvYlzpOcQfYz0xMdeFau+/i2gf7AL1OLtYAxVRhHHxntsNR5T8hX521
pbCuxca66mosjz0gpyIaYC/00Ie46tylG17lBb54ken5bySkRIdkkVao9cAomg/c
VOa3HCIeb76G8JeVhbd/QVmuwnmEEscrWXD4+gSnBRK6oIZ+/AJ5iajJ9i29mDNM
Tkogm8Rf7WjlFG4g3Wdh3u4DagXVOzjarDSVe8rPpBcteFovsOk1shIyZogrVFvA
LqMa8L3dTzpfODtyxLK8uIHlSkDpls17OY2pptETVPIPL39W4PHihNcNrqOQVy1b
fW9sYJIbAgMBAAECggEAZRusyz3jsJsy9g+JHbUgJG3A6SXWSozT/W5JV4DIk3OR
4x6IrINAbhIwn1BCjSsfKQxh+ONEi24WUAh5JlWTrFFKr3Xj3RQxeiGfaeK3v+IP
UkCN5oNbcHazGPOoWb4LeySgF3QQU97Pyq0kWOJHHgpe0IFu6sorvR6omVSBtIVh
Y1GxJ4uja6wBEz/37qJbjh31PCLU4n2ibyhizUeW/wLbCxysfgtNybiW0nDQf9lc
PiQdX4Mcb00oaX8Rih2op/nGjcV1q+bBA5czMSAXEwIdGIkwfdxkdmpRdKpuWVDs
8S1HutREFo/9Zhm0AkBs6eK0pcGxMyd9tQ9QQ6HNAQKBgQD5p0Ev1uAnh+C/Huqd
CRIugPbTP11IFejW5TH2bD3Xj3ZmpJC09v2bbiavD2yQR9M3lOaGeHlN8Z2kbIIR
qzW9xuPm4SwCE5FXS8U6j6wQZFX4vmn2/S+/fRukxQ5cE1cuJSk2mVuK9PlDfXQ+
KHJj3pI/29gIOmy5EDdQqXgKMwKBgQDTLN6t5q1xv+0atg2o7kHWo/iFIody6l69
r3VLQfgNGJDFSqMbGWP3k24nMAPNAIjAi2cBNhV82RyJfMwPKNTk3SesNkXTevTx
iDSGZKuZFHYCZxbBqpA4u1ufpe54usHte0gGzNtizVzsY3cRmnlhwqVUzduEIOPT
7HWPho1AeQKBgDDsaObiGf1FMHLjsSBjBbAdT8FoGnSk7oMmWRssbRYQJCjLORxt
hpduB6CoyiKgILE0udRCSatPnQ/6v6aMwbRWBJVbLQ+fHA1aaOUoAJUZxItBbWyc
gz3oW4F3qG+8zonZeHEdroXVqf9i12PS80/E7y4afARoxqOhnOVuwHpnAoGAGuip
y2EMkuUQ8olmPjN2AkLMpTJcLiF9RxB3kspqMEkEEY/MLuTSXzbTH303zsSVqGtb
CcV5gXos77wOSJQ8ZJllt8UGqscNNUXU45cqYow/6Vh3huAUFpaRO0uqkonBsmA2
Ml+iSPnAMIMQJhcYBoQGC0NcCH8kaNnFtS9BCokCgYEAv+SfOrD238t7jWc780Um
PpZu2iSk3D2nlGJK/gzwb/98t2p/et6cf9/H/CJqRSAzvWzmQjL1+Q5VHDgB9Ogt
HqKX44HzZxwOIHZEPWGOarys77DZnFiwNKhGslgRgX36/wPUPDw/dM+tuUu3Neyl
JPNYiPCHfFdRRZZApLecZUw=
-----END PRIVATE KEY-----";

string publicKey = @"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAzfCcokPXokxohfB2foWl
0gcr2Jc6TnEH2M9MTHXhWrvv4toH+wC9Ti7WAMVUYRx8Z7bDUeU/IV+dtaWwrsXG
uupqLI89IKciGmAv9NCHuOrcpRte5QW+eJHp+W8kpESHZJFWqPXAKJoP3FTmtxwi
Hm++hvCXlYW3f0FZrsJ5hBLHK1lw+PoEpwUSuqCGfvwCeYmoyfYtvZgzTE5KIJvE
X+1o5RRuIN1nYd7uA2oF1Ts42qw0lXvKz6QXLXhaL7DpNbISMmaIK1RbwC6jGvC9
3U86Xzg7csSyvLiB5UpA6ZbNezmNqabRE1TyDy9/VuDx4oTXDa6jkFctW31vbGCS
GwIDAQAB
-----END PUBLIC KEY-----";

string serverPublicKey = @"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAs/0S3fRK58o7ykUZI3cm
f+OswwKgbGb3C/yTUa2QGXIYhJLz034jrr7Pw/KVnaF/4aRCMg/NpDQPlEAS3M8m
m3BtQSldNbT4xHVk25poO/HA0IcRYClupnsbedD9u0SiOM7WOvQnVewbC7BBl2oD
TUtRrADL62Wq/A92RSwBMrVo/YhYbSvwqdsOOij41rWZlBRL31H1v2MHEICqFibT
sIYgTRR4xnpjsJsWqAJpZWrbraFxVpk/zQ4V7/Vdg6dvPhRgSCNSpzWILR12qDoB
K7FlN82qGCKm3MNehyPkASftMb/2VNvWfqmiikBw7H0+ZPyKt7d82HYTJomC30gb
vwIDAQAB
-----END PUBLIC KEY-----";

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

        if (args.Length > 1)
        {
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
                    await GetPatient(command_args[1..]);
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

async Task GetPatient(string[] args)
{
    if (args.Length != 1)
    {
        Console.WriteLine("[Error] Command get should have one argument.\n");
        return;
    }

    var request = new HttpRequestMessage
    {
        Method = HttpMethod.Get,
        RequestUri = new Uri($"{HOST}/patients/{args[0]}"),
        Content = new StringContent(Convert.ToBase64String(Crypto.SignData(Encoding.UTF8.GetBytes(args[0]), privateKey)))
    };

    var response = await client.SendAsync(request);

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
        Console.WriteLine(node);
        // TODO: check consultation record authenticity.
    }
    else
    {
        Console.WriteLine(Encoding.UTF8.GetString(data));
        Console.WriteLine(Encoding.UTF8.GetString(encryptedData));
        Console.WriteLine("[Error]: Data was tampered with.");
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
