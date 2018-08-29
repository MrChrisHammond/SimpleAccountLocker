using SimpleAccountLocker.Helpers;
using SimpleAccountLocker.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimpleAccountLocker.Viewmodel
{
    /// <summary>
    /// ViewModel for new account dialog 
    /// </summary>
    public class NewAccountDialogViewModel : BaseViewModel, IDialogRequestClose
    {
        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;
        public NewAccountDialogViewModel()
        {
        }
        public string SiteName
        {
            get;
            set;
        }
        public string UserName
        {
            get;
            set;
        }
        private string password;
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
                OnPropertyChange("Password");

            }
        }
        private bool generatePasswordCheckBox;
        public bool GeneratePasswordCheckBox
        {
            get
            {
                return generatePasswordCheckBox;
            }
            set
            {
                generatePasswordCheckBox = value;
                GeneratePassword(int.Parse(PasswordLength), int.Parse(PasswordSpecialCharsCount));
            }
        }
        private int passwordLength = 14;
        public string PasswordLength
        {
            get
            {
                if (passwordLength < 0)
                    return "";
                else
                    return passwordLength.ToString();
            }
            set
            {
                if (value.Length <= 0)
                    passwordLength = -1;
                else
                {
                    int.TryParse(value, out passwordLength);
                    GeneratePassword(passwordLength, passwordSpecialCharsCount);
                }
            }
        }
        private int passwordSpecialCharsCount = 4;
        public string PasswordSpecialCharsCount
        {
            get
            {
                if (passwordSpecialCharsCount < 0)
                    return "";
                else
                return passwordSpecialCharsCount.ToString();
            }
            set
            {
                if (value.Length <= 0)
                    passwordSpecialCharsCount = -1;
                else
                {
                    int.TryParse(value, out passwordSpecialCharsCount);
                    GeneratePassword(passwordLength, passwordSpecialCharsCount);
                }
            }
        }
        private void GeneratePassword(int length, int specialCharCount)
        {
            if (length > 0)
            {
                if (specialCharCount < 0)
                    specialCharCount = 0;
                if (specialCharCount > length)
                    specialCharCount = length;
                Password = System.Web.Security.Membership.GeneratePassword(length, specialCharCount);
            }
        }
        public ICommand SaveAccountCommand
        {
            get
            {
                return new RelayCommand(p => true, p => this.SaveAccount());
            }
        }
        public void SaveAccount()
        {
            AccountModel account = new AccountModel(UserName, Password, SiteName);
            //by design no validation as we will allow users to save empty rows.
            CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true, account));

        }
        public ICommand CancelCommand
        {
            get
            {
                return new RelayCommand(p => true, p => CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(false, null)));
            }
        }
        
    }
}
