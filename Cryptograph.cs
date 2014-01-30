/*
 * Author:			Vex Tatarevic
 * Date Created:	2012-03-28
 * Copyright:       VEXIT Pty Ltd - www.vexit.com
 *
 * Description:     This is a cut-off version of Security Cryptograph from VEX IT security framework. 
 *                  This version features only the AES Managed Encryption/Decryption which is a current VEX IT security standard.
 * 
 */


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Configuration;

namespace vEX.Security
{
    public class Cryptograph
    {
        // AES KEYS
        //private static string KEY = "5rRj490s6DZ1eCcpJBFmNDrOQqUKT8kc+q7W0qsDYQo=";// 256 bits - Default Key - Convert.FromBase64String(KEY)
        //private static string IV = "K9ALmdBO1Eb7vIlE62m57A=="; // 128 bits - Default IV - convert to byte[] like this  Convert.FromBase64String(IV)

        // AES Encryption - default encryption key and iv stored inside web.config or app.config
        private static string _KEY = ConfigurationManager.AppSettings["AppSecurity_Key"]; // 256 bits Key - Convert.FromBase64String(KEY)
        private static string _IV = ConfigurationManager.AppSettings["AppSecurity_IV"]; // 128 bits IV - convert to byte[] like this  Convert.FromBase64String(IV)

        /// <summary>
        ///  Generate a new key using AES Managed algorhythm
        /// </summary>
        public static string NewKey
        {
            get
            {
                string key = "";
                using (AesManaged aesCrypto = new AesManaged())
                {
                    key = Convert.ToBase64String(aesCrypto.Key);
                }
                return key;
            }
        }

        /// <summary>
        ///  Generate a new Iitialization Vector using AES Managed algorhythm
        /// </summary>
        public static string NewIV
        {
            get
            {
                string iv = "";
                using (AesManaged aesCrypto = new AesManaged())
                {
                    iv = Convert.ToBase64String(aesCrypto.IV);
                }
                return iv;
            }
        }

        #region [ AES Managed ]
        // http://msdn.microsoft.com/en-us/library/system.security.cryptography.aesmanaged.aspx
        // Calls the Windows Crypto API, which uses RSAENH.DLL, and has been validated by NIST (National Institute of Standards and Technology) Cryptographic Module Validation Program (CMVP). 
        // If the Windows security policy setting for Federal Information Processing Standards (FIPS)-compliant algorithms is enabled, using this algorithm throws a CryptographicException.

        // AES encrypt
        private static string Encrypt_AESManaged(string plainText, string key, string iv) { return Encrypt_AESManaged(plainText, Convert.FromBase64String(key), Convert.FromBase64String(iv)); }
        private static string Encrypt_AESManaged(string plainText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("Key");
            byte[] encrypted;

            // Create an AesManaged object with the specified key and IV.
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            // convert back to a string
            return Convert.ToBase64String(encrypted);
        }
        // AES decrypt
        private static string Decrypt_AESManaged(string plainText, string key, string iv) { return Decrypt_AESManaged(plainText, Convert.FromBase64String(key), Convert.FromBase64String(iv)); }
        private static string Decrypt_AESManaged(string cypherText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (cypherText == null || cypherText.Length <= 0)
                throw new ArgumentNullException("cypherText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("Key");

            // Declare the string used to hold the decrypted text.
            string plaintext = null;
            byte[] cypherTextBytes = Convert.FromBase64String(cypherText); // convert string to byte array

            // Create an AesManaged object with the specified key and IV.
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cypherTextBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }
        #endregion

        public static string Encrypt(string value, string key = null, string iv = null) { return Cryptograph.Encrypt_AESManaged(value, (key == null ? _KEY : key), (iv == null ? _IV : iv)); } //return Cryptograph.Encrypt_TripleDES(value, KEY_192, IV_192); }
        public static string Decrypt(string value, string key = null, string iv = null) { return Cryptograph.Decrypt_AESManaged(value, (key == null ? _KEY : key), (iv == null ? _IV : iv)); } //return Cryptograph.Decrypt_TripleDES(value, KEY_192, IV_192); }






    }
}
