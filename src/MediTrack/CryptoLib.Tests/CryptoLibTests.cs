using Xunit;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using System.Text;

namespace CryptoLib.Tests;

public class CryptoLibTests
{
    // [Fact]
    [Theory]
    [InlineData("./input.json")]
    [InlineData("./backend-input.json")]
    public void Test1(string path)
    {
        string jsonString = File.ReadAllText(path);
        JsonNode? inputData = JsonNode.Parse(jsonString);
        Assert.NotNull(inputData);

        var rsaSender = RSA.Create();
        var rsaReceiver = RSA.Create();

        var encryptedData = Crypto.Protect(inputData, rsaReceiver.ExportRSAPublicKeyPem(), rsaSender.ExportRSAPrivateKeyPem());
        Assert.NotNull(encryptedData);

        var decryptedData = Crypto.Unprotect(encryptedData, rsaReceiver.ExportRSAPrivateKeyPem());
        Assert.NotNull(decryptedData);

        Assert.True(Crypto.Check(decryptedData[..256], decryptedData[256..], rsaSender.ExportRSAPublicKeyPem()));
        Console.WriteLine(JsonNode.Parse(decryptedData[256..]));
    }
}
