//*****************************************************************************
//
//  By The Dummy Programmer
//  https://www.thedummyprogrammer.com
//
//  Part of this code was taken from:
//  https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.rsacryptoserviceprovider?view=netframework-4.7.2
//*****************************************************************************

using System;
using System.Text;
using System.Security.Cryptography;

namespace TDP.BaseServices.Infrastructure.Security
{
    public class AsymmetricCryptography
    {
        private AsymmetricCryptography()
        {            
        }

        public static string CreateKey()
        {
            using (var Rsa = new RSACryptoServiceProvider())
            {
                try
                {
                    return Rsa.ToXmlString(true);
                }
                finally
                {
                    Rsa.PersistKeyInCsp = false;
                }
            }
        }

        public static string SignData(string message, RSAParameters privateKey)
        {
            //// The array to store the signed message in bytes
            byte[] SignedBytes;
            using (var Rsa = new RSACryptoServiceProvider())
            {
                //// Write the message to a byte array using UTF8 as the encoding.
                var Encoder = new UTF8Encoding();
                byte[] OriginalData = Encoder.GetBytes(message);

                try
                {
                    //// Import the private key used for signing the message
                    Rsa.ImportParameters(privateKey);

                    //// Sign the data, using SHA512 as the hashing algorithm 
                    SignedBytes = Rsa.SignData(OriginalData, CryptoConfig.MapNameToOID("SHA512"));
                }
                catch (CryptographicException)
                {
                    return null;
                }
                finally
                {
                    //// Set the keycontainer to be cleared when rsa is garbage collected.
                    Rsa.PersistKeyInCsp = false;
                }
            }
            //// Convert the a base64 string before returning
            return Convert.ToBase64String(SignedBytes);
        }

        public static bool VerifyData(string originalMessage, string signedMessage, RSAParameters publicKey)
        {
            bool Success = false;
            using (var Rsa = new RSACryptoServiceProvider())
            {
                byte[] BytesToVerify = Convert.FromBase64String(originalMessage);
                byte[] SignedBytes = Convert.FromBase64String(signedMessage);
                try
                {
                    Rsa.ImportParameters(publicKey);

                    SHA512Managed Hash = new SHA512Managed();

                    byte[] HashedData = Hash.ComputeHash(SignedBytes);

                    Success = Rsa.VerifyData(BytesToVerify, CryptoConfig.MapNameToOID("SHA512"), SignedBytes);
                }
                catch (CryptographicException e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    Rsa.PersistKeyInCsp = false;
                }
            }
            return Success;
        }

        public static string SignData(string message, string xmlKey)
        {
            //// The array to store the signed message in bytes
            byte[] SignedBytes;
            using (var Rsa = new RSACryptoServiceProvider())
            {
                //// Write the message to a byte array using UTF8 as the encoding.
                var Encoder = new UTF8Encoding();
                byte[] OriginalData = Encoder.GetBytes(message);

                try
                {
                    //// Import the private key used for signing the message
                    Rsa.FromXmlString(xmlKey);

                    //// Sign the data, using SHA512 as the hashing algorithm 
                    SignedBytes = Rsa.SignData(OriginalData, CryptoConfig.MapNameToOID("SHA512"));
                }
                catch (CryptographicException)
                {
                    return null;
                }
                finally
                {
                    //// Set the keycontainer to be cleared when rsa is garbage collected.
                    Rsa.PersistKeyInCsp = false;
                }
            }
            //// Convert the a base64 string before returning
            return Convert.ToBase64String(SignedBytes);
        }

        public static bool VerifyData(string originalMessage, string signedMessage, string xmlKey)
        {
            bool Success = false;
            using (var Rsa = new RSACryptoServiceProvider())
            {
                byte[] BytesToVerify = Convert.FromBase64String(originalMessage);
                byte[] SignedBytes = Convert.FromBase64String(signedMessage);
                try
                {
                    Rsa.FromXmlString(xmlKey);

                    SHA512Managed Hash = new SHA512Managed();

                    byte[] HashedData = Hash.ComputeHash(SignedBytes);

                    Success = Rsa.VerifyData(BytesToVerify, CryptoConfig.MapNameToOID("SHA512"), SignedBytes);
                }
                catch (CryptographicException e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    Rsa.PersistKeyInCsp = false;
                }
            }
            return Success;
        }
    }
}
