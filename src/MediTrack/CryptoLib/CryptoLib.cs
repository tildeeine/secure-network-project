namespace CryptoLib;

using System.Diagnostics;
using System.Security.Cryptography;

public class Crypto
{

    /// <summary>
    /// Computes the <paramref name="data"/> Digital Signature using <paramref name="authKey"/>, 
    /// in order to provide authenticity.
    /// Encrypts the <c>digital signature</c> and original <paramref name="data"/> with a <c>symmetric key</c> providing confidentiality.
    ///
    /// </summary>
    /// <param name="data"><c>data</c> you want to encrypt.</param>
    /// <param name="encryptionKey"><c>encryptionKey</c> is the public key of the receiver in Pem format. So only the receiver can read. 
    /// the encrypted data</param>
    /// <param name="authKey"><c>authKey</c> is the sender's private key in Pem format. Used to provide authenticity.</param>
    /// <returns> 
    /// byte[]? with the <paramref name="data"/> and a header that 
    /// contains the <c>symmetric key</c> and <c>initialization vector</c> used to encrypt the <paramref name="data"/>. 
    /// The <c>header</c> was encrypted with the <paramref name="encryptionKey"/> so the sender is the only one (apart from us), with access to the shared key.
    /// </returns>
    static public byte[]? Protect(byte[] data, string encryptionKey, string authKey)
    {
        try
        {
            using RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            using Aes aes = Aes.Create();
            rsa.ImportFromPem(authKey);
            byte[] hash = rsa.SignData(data, SHA256.Create());

            rsa.ImportFromPem(encryptionKey);
            aes.GenerateIV();
            aes.GenerateKey();
            byte[] symmetric = rsa.Encrypt([.. aes.Key, .. aes.IV], false);
            return [.. symmetric, .. aes.EncryptCbc(hash.Concat(data).ToArray(), aes.IV)];
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error during encryption {e.Message}");
            return null;
        }
    }

    /// <summary>
    /// Decrypts the <c>header</c> of the given <paramref name="data"/> with the provided <paramref name="decryptionKey"/>
    /// in order to get a shared <c>symmetric key</c>. With the <c>symmetric key</c> decrypt the remaining <paramref name="data"/>.
    /// </summary>
    /// <param name="data"><c>data</c> you want to decrypt.</param>
    /// <param name="decryptionKey"><c>decryptionKey</c> is the private key of the receiver in Pem format.</param> 
    /// <returns> byte[]? with the <c>decrypted data</c> </returns>
    static public byte[]? Unprotect(byte[] data, string decryptionKey)
    {
        try
        {
            using RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            using Aes aes = Aes.Create();
            rsa.ImportFromPem(decryptionKey);

            // header is [symmetricKey, iv]
            byte[] header = rsa.Decrypt(data[..256], false);

            const int HEADER_SIZE = 32 + 16; // Aes.Key size + Aes.IV size
            Debug.Assert(
                header.Length == HEADER_SIZE,
                $"ERROR in Unprotect: header size is {header.Length} and should be {HEADER_SIZE}");

            aes.Key = header[..32];
            byte[] iv = header[32..];

            return aes.DecryptCbc(data[256..], iv);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }

    /// <summary>
    /// Verifies the integrity of the given <paramref name="data"/> by hashing it and comparing it
    /// with the deciphered <paramref name="signedData"/>.
    /// </summary>
    /// <param name="signedData"><c>signedData</c> is the <paramref name="data"/> signed with the senders private key.</param>
    /// <param name="data"><c>data</c> you want to check for integrity.</param>
    /// <param name="key"><c>key</c> is the senders public key in Pem format.</param>
    /// <returns> <c>true</c> if the <paramref name="data"/> wasn't tampered with, <c>false</c> otherwise.</returns>
    static public bool Check(byte[] signedData, byte[] data, string key)
    {
        // TODO: Check for freshness too.
        try
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportFromPem(key);
                return rsa.VerifyData(data, SHA256.Create(), signedData);

            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }
}
