using SimpleAccountLocker.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAccountLocker.Model
{
    /// <summary>
    /// SimpleAccountLocker Settings Data Model Class.
    /// </summary>
    [Serializable]
    public class Settings
    {
        public string saveFileLocation;
        public string keyLocation;
        public readonly static string defaultSettingsFileName = "settings.dat";
        public readonly static string defaultKeyFileName = "kf.kfsal";
        public readonly static string defaultAccountListFileName = "accounts.sal";
        /// <summary>
        /// Settings constructor with parameters.
        /// </summary>
        /// <param name="saveFileLocation">File location to save the account list.</param>
        /// <param name="keyLocation">File location to save key file location.</param>
        public Settings(string saveFileLocation, string keyLocation)
        {
            this.saveFileLocation = saveFileLocation;
            this.keyLocation = keyLocation;
        }
        /// <summary>
        /// Settings constructor without parameters.
        /// Default settings will be used including save location.
        /// </summary>
        public Settings()
        {
            this.saveFileLocation = FileManager.GetLocalAccountAppFolder()  +  @"\" + defaultAccountListFileName;
            this.keyLocation = FileManager.GetLocalAccountAppFolder() + @"\" + defaultKeyFileName;
        }
        /// <summary>
        /// Returns the file location of this applications settings file.
        /// </summary>
        public static string SettingsFileURI
        {
            get {
                return FileManager.GetLocalAccountAppFolder() + @"\" + defaultSettingsFileName;
            }
        }
    }
}
