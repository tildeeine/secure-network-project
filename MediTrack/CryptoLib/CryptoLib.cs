namespace CryptoLib;

using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

public class Crypto
{

    /// <summary>
    /// Computes the <paramref name="data"/> Digital Signature using <paramref name="authKey"/>, 
    /// in order to provide authenticity.
    /// Encrypts all the json <paramref name="data"/> values with a <c>symmetric key</c> providing confidentiality.
    ///
    /// </summary>
    /// <param name="data"><c>data</c> Json data that you want to encrypt.</param>
    /// <param name="encryptionKey"><c>encryptionKey</c> is the public key of the receiver in Pem format. So only the receiver can read. 
    /// the encrypted data</param>
    /// <param name="authKey"><c>authKey</c> is the sender's private key in Pem format. Used to provide authenticity.</param>
    /// <returns> 
    /// byte[]? with the encrypted <paramref name="data"/> and a header that 
    /// contains the <c>symmetric key</c> and <c>initialization vector</c> used to encrypt the <paramref name="data"/>. 
    /// The <c>header</c> was encrypted with the <paramref name="encryptionKey"/> so the sender is the only one (apart from us), with access to the shared key.
    /// </returns>
    static public byte[]? Protect(JsonNode data, string encryptionKey, string authKey)
    {
        try
        {
            // rsa.ImportFromPem(authKey);
            // byte[] hash = rsa.SignData(JsonSerializer.SerializeToUtf8Bytes(data), SHA256.Create());
            // Console.WriteLine($"Data before: {data} {Convert.ToBase64String(JsonSerializer.SerializeToUtf8Bytes(data))}");
            byte[] hash = Crypto.SignData(JsonSerializer.SerializeToUtf8Bytes(data), authKey);

            using RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            using Aes aes = Aes.Create();

            rsa.ImportFromPem(encryptionKey);
            aes.GenerateIV();
            aes.GenerateKey();
            byte[] symmetric = rsa.Encrypt([.. aes.Key, .. aes.IV], false);

            JsonNode patient = data["patient"]!;
            JsonArray allergies = patient["knownAllergies"]!.AsArray();
            JsonArray consultationRecords = patient["consultationRecords"]!.AsArray();

            int i = 0;
            foreach (var consultation in consultationRecords.ToList())
            {
                consultationRecords[i] = new JsonObject
                {
                    ["date"] = EncryptToBase64(consultation!["date"]!.ToString(), aes),
                    ["medicalSpeciality"] = EncryptToBase64(consultation!["medicalSpeciality"]!.ToString(), aes),
                    ["doctorName"] = EncryptToBase64(consultation!["doctorName"]!.ToString(), aes),
                    ["practice"] = EncryptToBase64(consultation!["practice"]!.ToString(), aes),
                    ["treatmentSummary"] = EncryptToBase64(consultation!["treatmentSummary"]!.ToString(), aes),
                };
                i++;
            }
            i = 0;
            foreach (var allergie in allergies.ToList())
            {
                allergies[i] = EncryptToBase64(allergie!.ToString(), aes);
                i++;
            }

            patient["name"] = EncryptToBase64(patient!["name"]!.ToString(), aes);
            patient["sex"] = EncryptToBase64(patient!["sex"]!.ToString(), aes);
            patient["dateOfBirth"] = EncryptToBase64(patient!["dateOfBirth"]!.ToString(), aes);
            patient["bloodType"] = EncryptToBase64(patient!["bloodType"]!.ToString(), aes);

            // Console.WriteLine($"Hash: {Convert.ToBase64String(hash)}");
            // Console.WriteLine($"Data: {data}");
            return [.. symmetric, .. hash, .. JsonSerializer.SerializeToUtf8Bytes(data)];
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
            // Skip header bytes
            data = data[256..];

            const int HEADER_SIZE = 32 + 16; // Aes.Key size + Aes.IV size
            Debug.Assert(
                header.Length == HEADER_SIZE,
                $"ERROR in Unprotect: header size is {header.Length} and should be {HEADER_SIZE}");

            aes.Key = header[..32];
            aes.IV = header[32..];


            JsonNode objectData = JsonNode.Parse(data[256..])!; // Skip hash bytes
            JsonNode patient = objectData["patient"]!;

            patient["name"] = DecryptFromBase64(patient!["name"]!.ToString(), aes);
            patient["sex"] = DecryptFromBase64(patient!["sex"]!.ToString(), aes);
            patient["dateOfBirth"] = DecryptFromBase64(patient!["dateOfBirth"]!.ToString(), aes);
            patient["bloodType"] = DecryptFromBase64(patient!["bloodType"]!.ToString(), aes);

            int i = 0;
            JsonArray consultationRecords = patient["consultationRecords"]!.AsArray();
            foreach (var consultation in consultationRecords.ToList())
            {
                consultationRecords[i] = new JsonObject
                {
                    ["date"] = DecryptFromBase64(consultation!["date"]!.ToString(), aes),
                    ["medicalSpeciality"] = DecryptFromBase64(consultation!["medicalSpeciality"]!.ToString(), aes),
                    ["doctorName"] = DecryptFromBase64(consultation!["doctorName"]!.ToString(), aes),
                    ["practice"] = DecryptFromBase64(consultation!["practice"]!.ToString(), aes),
                    ["treatmentSummary"] = DecryptFromBase64(consultation!["treatmentSummary"]!.ToString(), aes),
                };
                i++;
            }
            i = 0;
            JsonArray allergies = patient["knownAllergies"]!.AsArray();
            foreach (var allergie in allergies.ToList())
            {
                allergies[i] = DecryptFromBase64(allergie!.ToString(), aes);
                i++;
            }
            // Console.WriteLine($"Decrypted data: {objectData} {Convert.ToBase64String(JsonSerializer.SerializeToUtf8Bytes(objectData))}");
            // Console.WriteLine($"Hash: {Convert.ToBase64String(data[..256])}");
            return [.. data[..256], .. JsonSerializer.SerializeToUtf8Bytes(objectData)];
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
            // Console.WriteLine($"check hash: {Convert.ToBase64String(signedData)}");
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

    /// <summary>
    /// Provides authenticity to the data.
    /// </summary>
    /// <param name="data"><c>data</c> you want to provide authenticity.</param>
    /// <param name="key"><c>key</c> is the senders private key in Pem format.</param>
    /// <returns> <c>hashed data</c>.</returns>
    public static byte[] SignData(byte[] data, string key)
    {

        using RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        rsa.ImportFromPem(key);
        return rsa.SignData(data, SHA256.Create());
    }

    private static string EncryptToBase64(string value, Aes aes) => Convert.ToBase64String(aes.EncryptCbc(Encoding.UTF8.GetBytes(value), aes.IV));
    private static string DecryptFromBase64(string value, Aes aes) => Encoding.UTF8.GetString(aes.DecryptCbc(Convert.FromBase64String(value), aes.IV));
}
