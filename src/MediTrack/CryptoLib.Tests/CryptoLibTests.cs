using Xunit;
using System.Security.Cryptography;
using System.Text;

namespace CryptoLib.Tests;

public class CryptoLibTests
{
    [Fact]
    public void Test1()
    {
        byte[] inputData = File.ReadAllBytes("./input.json");
        var rsaSender = RSA.Create();
        var rsaReceiver = RSA.Create();

        var encryptedData = Crypto.Protect(inputData, rsaReceiver.ExportRSAPublicKeyPem(), rsaSender.ExportRSAPrivateKeyPem());
        Assert.NotNull(encryptedData);
        var decryptedData = Crypto.Unprotect(encryptedData, rsaReceiver.ExportRSAPrivateKeyPem());
        Assert.NotNull(decryptedData);

        Assert.True(Crypto.Check(decryptedData[..256], decryptedData[256..], rsaSender.ExportRSAPublicKeyPem()));
        Console.Write(Encoding.UTF8.GetChars(decryptedData[256..]));
    }
}
