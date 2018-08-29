using SimpleAccountLocker.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAccountLocker.Model
{
    [Serializable]
    public class AccountModel : INotifyPropertyChanged
    {
        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        public AccountModel(string accountName, string passwordPlainText, string accountURL)
        {
            keyFile = new KeyFile();
            this.AccountName = accountName;
            this.PasswordPlainText = passwordPlainText;
            this.AccountURL = accountURL;
            DateCreated = DateTime.UtcNow;
        }
        public AccountModel()
        {
            DateCreated = DateTime.UtcNow;
            LastModified = DateTime.UtcNow;
        }
        private string _accountName = "";
     //   private string _passwordPlainText = "";
        private string _accountURL = "";
        private double _unixDateCreated = 0;
        private double _unixLastModified = 0;
        public KeyFile keyFile;
        public byte[] encryptedPassword { get; set; }
        public byte[] encryptedUserName { get; set; }
        public bool isSelected
        {
            get;
            set;
        }

        public string AccountName
        {
            get
            {
                return _accountName;
            }
            set
            {
                _accountName = value;
                OnPropertyChange("AccountName");
            }
        }
        public string PasswordPlainText
        {
            get
            {
                if (isSelected)
                {
                    if (encryptedPassword != null)
                    {
                        string plainText = (string)CryptoTools.DecryptData(keyFile.key, keyFile.IV, encryptedPassword);
                        if (plainText != null)
                            return plainText;
                        else return "";
                    }
                    else return "";
                }
                else
                {
                    return "*********";
                }
            }
            set
            {
                //_passwordPlainText = value;
                byte[] data = CryptoTools.EncryptData(keyFile.key, keyFile.IV, value);
                if (data != null)
                    encryptedPassword = data;
                OnPropertyChange("PasswordPlainText");
            }
        }

        public string AccountURL
        {
            get
            {
                return _accountURL;
            }
            set
            {
                _accountURL = value;
                OnPropertyChange("AccountURL");
            }
        }
        public DateTime DateCreated
        {
            get
            {
                return Helpers.UnixTime.UnixTimestampToDateTime(_unixDateCreated);
            }
            set
            {
                _unixDateCreated = Helpers.UnixTime.DateTimeToUnixTimestamp(value);
            }
        }
        public DateTime LastModified
        {
            get
            {
                return Helpers.UnixTime.UnixTimestampToDateTime(_unixLastModified);
            }
            set
            {
                _unixLastModified = Helpers.UnixTime.DateTimeToUnixTimestamp(value);
                OnPropertyChange("LastModified");
            }
        }
        public bool IsEmpty {
            get
            {
                if (AccountName.Length <= 0 && PasswordPlainText.Length <= 0 && AccountURL.Length <= 0)
                    return true;
                else return false;
            }
        }
        /// <summary>
        /// Notify handler that a property has changed.
        /// </summary>
        /// <param name="name">Name of the property that has changed.</param>
        public void OnPropertyChange(string name)
        {
            if(!name.Equals("LastModified"))
                LastModified = DateTime.UtcNow;
            PropertyChangedEventHandler handler = PropertyChanged;
            if(handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
