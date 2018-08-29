using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleAccountLocker.Helpers
{
    /// <summary>
    /// A static class intended for helping to access/create/write/update files to disk location.
    /// </summary>
    public static class FileManager
    {
        /// <summary>
        /// Save object as file to disk at specified location.
        /// </summary>
        /// <param name="saveLocation">Location and name of file on local disk to save.</param>
        /// <param name="o">object to save as file on local disk.</param>
        public static void SaveFile(string saveLocation, object o)
        {
            try
            {
                FileStream fs = new FileStream(saveLocation, FileMode.Create);
                try
                {

                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, o);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
                finally
                {
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
        /// <summary>
        /// Asyncronous save byte data to disk at savePath location.
        /// </summary>
        /// <param name="savePath">File path, including file name, to save to disk.</param>
        /// <param name="data">byte array data to save to disk.</param>
        public static async Task<bool> SaveFileAsync(string savePath, byte[] data, bool createDirectory)
        {
            try
            {
                if (createDirectory)
                {
                    FileInfo file = new FileInfo(savePath);
                    file.Directory.Create();
                }
                // System.Threading.Thread.Sleep(10000);
            //    await Task.Delay(1000);
                using (FileStream sourceStream = new FileStream(savePath,
                    FileMode.Create, FileAccess.Write, FileShare.None,
                    bufferSize: 4096, useAsync: true))
                {
                    await sourceStream.WriteAsync(data, 0, data.Length);
                    return true;
                };
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debug.WriteLine(ex);
          //      MessageBox.Show(ex.ToString());
                return false;
              //  return null;
            }
        }
        /// <summary>
        /// Open file at location into memory.
        /// </summary>
        /// <param name="saveLocation">Location and name of file you want to open.</param>
        /// <returns></returns>
        public static async Task<byte[]> OpenFile(string saveLocation)
        {
            //  try
            // {
            // System.Threading.Thread.Sleep(10000);
            //   await Task.Delay(1000);
            if (!File.Exists(saveLocation))
                return null;

                byte[] result;
                using (FileStream fileStream = File.Open(saveLocation, FileMode.Open))
                {
                    result = new byte[fileStream.Length];
                    await fileStream.ReadAsync(result, 0, (int)fileStream.Length);
                }
                return result;
         //   }
          //  catch(Exception ex)
          //  {
          //      System.Diagnostics.Debug.WriteLine(ex);
            //    return null;
          //  }
        }
        /// <summary>
        /// Deserialize byte array into typeof T object.
        /// </summary>
        /// <typeparam name="T">object type of data being deserialized.</typeparam>
        /// <param name="param">byte array to be deserialized.</param>
        /// <returns></returns>
        public static T Deserialize<T>(byte[] param)
        {
            using (MemoryStream ms = new MemoryStream(param))
            {
                IFormatter br = new BinaryFormatter();
                return (T)br.Deserialize(ms);
            }
        }
        /// <summary>
        /// Serialize object to byte array.
        /// </summary>
        /// <param name="o">object to be serialized.</param>
        /// <returns></returns>
        public static byte[] Serialize(object o)
        {
            //FileStream fs = new FileStream(saveLocation, FileMode.Create);
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, o);
                ms.Close();
                return ms.ToArray();
            }
        }
        /// <summary>
        /// Get this application appdata folder from Windows local folder in %appdata%.
        /// </summary>
        /// <returns>Returns this applications local %appdata% folder.</returns>
        public static string GetLocalAccountAppFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\" + Application.ProductName;
        }
    }
}
