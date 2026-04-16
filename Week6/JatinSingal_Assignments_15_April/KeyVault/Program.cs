using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using System.Diagnostics.Metrics;
using System.Reflection.Metadata;
using System.Text;

namespace KeyVault
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string tenantId = "3e983587-74bf-4c91-8917-18caf9ef9019";
 string clientId = "7b6e8f0e-383b-4541-9b86-039b43db481f";
            clientSecret = "your-real-secret"


            var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);

            string vaultUrl = "https://newkeyvault121.vault.azure.net/";
            string keyName = "VAULT-KEY-1409";


            var keyClient = new KeyClient(new Uri(vaultUrl), credential);

   KeyVaultKey key;

            key = await keyClient.GetKeyAsync(keyName);

            string originalText = "Sensitive order data for CloudXeus Technology Services";
            byte[] plaintextBytes = Encoding.UTF8.GetBytes(originalText);

            var cryptoClient = new CryptographyClient(key.Id, credential);

            EncryptResult encryptResult = await cryptoClient.EncryptAsync(
                EncryptionAlgorithm.RsaOaep,
                plaintextBytes);

            Console.WriteLine("Encrypted text (Base64):");
            Console.WriteLine(Convert.ToBase64String(encryptResult.Ciphertext));

            DecryptResult decryptResult = await cryptoClient.DecryptAsync(
                EncryptionAlgorithm.RsaOaep,
                encryptResult.Ciphertext);

            string decryptedText = Encoding.UTF8.GetString(decryptResult.Plaintext);

            Console.WriteLine("\nDecrypted text:");
            Console.WriteLine(decryptedText);

            Console.ReadLine();
        }
    }
}
