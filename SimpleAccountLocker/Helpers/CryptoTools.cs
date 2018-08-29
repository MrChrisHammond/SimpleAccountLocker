using SimpleAccountLocker.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleAccountLocker.Helpers
{
    /// <summary>
    /// A relatively simple class with methods to encrypt and decrypt data.
    /// </summary>
    public class CryptoTools
    {
        /// <summary>
        /// Encrypt byte array.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="IV"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] EncryptData(byte[] key, byte[] IV, object data)
        {
            byte[] encryptedData;
            RijndaelManaged rj = new RijndaelManaged();

            rj.Mode = CipherMode.CBC;
         //   rj.BlockSize = 256;

            ICryptoTransform t = rj.CreateEncryptor(key, IV);
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, t, CryptoStreamMode.Write))
                {
                    bf.Serialize(cs, data);
                    cs.FlushFinalBlock();
                    cs.Close();
                    encryptedData = ms.ToArray();
                }
            }
            return encryptedData;
        }
        /// <summary>
        /// Decrypt byte array
        /// </summary>
        /// <param name="key"></param>
        /// <param name="IV"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static object DecryptData(byte[] key, byte[] IV, byte[] data)
        {
            object decryptedData;
            RijndaelManaged rj = new RijndaelManaged();
            rj.Mode = CipherMode.CBC;
          //  rj.BlockSize = 256;
            ICryptoTransform t = rj.CreateDecryptor(key, IV);
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (CryptoStream cs = new CryptoStream(ms, t, CryptoStreamMode.Read))
                {
                    decryptedData = bf.Deserialize(cs);
                    cs.Close();
                }
            }
            return decryptedData;
        }
        /// <summary>
        /// Creates and saves stored keyfile in appdata based on current filename.
        /// </summary>
        /// <returns>New keyfile saved to disk.</returns>
        public static async Task<KeyFile> CreateStoredKeyFile(string saveLocation)
        {
            KeyFile key = new KeyFile(true);
            await FileManager.SaveFileAsync(saveLocation, FileManager.Serialize(key), true);
            return key;
        }
        /// <summary>
        /// Load a stored keyfile from disk at given location.
        /// </summary>
        /// <param name="saveLocation">File location on disk.</param>
        /// <returns>Keyfile from disk, or if not found, null.</returns>
        public static async Task<KeyFile> LoadStoredKeyFile(string saveLocation)
        {
            byte[] data = await FileManager.OpenFile(saveLocation);
            KeyFile key = null;
            if (data != null)
                key = FileManager.Deserialize<KeyFile>(data);

            return key;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string SHA256Hash(string input)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] returnData = new SHA256Managed().ComputeHash(data);
            return Encoding.ASCII.GetString(returnData);
        }

    }
}
