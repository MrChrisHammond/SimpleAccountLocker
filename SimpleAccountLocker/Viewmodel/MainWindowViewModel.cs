using SimpleAccountLocker.Helpers;
using SimpleAccountLocker.Viewmodel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SimpleAccountLocker.Model
{
    public class MainWindowViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private int selectedIndex;
        public int lastAccountAdded = -1;
        private readonly IDialogService dialogService;
        private Settings settings;

        public BetterObservableCollection<AccountModel> _accountList;
        /// <summary>
        /// List of AccountModel in an ObservableCollection.
        /// </summary>
        public BetterObservableCollection<AccountModel> AccountsList
        {
            get
            {
                return _accountList;
            }
            set
            {
                _accountList = value;
                OnPropertyChange("AccountsList");

            }

        }
        private bool locked;
        public bool Locked
        {
            get
            {
                return locked;
            }
            set
            {
                locked = value;
                if (locked)
                    LockButtonText = "Unlock Account List";
                else
                    LockButtonText = "Lock Account List";
            }

        }
        private string lockButtonText;
        public string LockButtonText
        {
            get
            {
                return lockButtonText;
            }
            set
            {
                lockButtonText = value;
                OnPropertyChange("LockButtonText");
            }
        }
        private AccountModel _selectedAccount;
        /// <summary>
        /// Selected Account set through View XAML with two-way DataGrid setter.
        /// </summary>
        public AccountModel SelectedAccount
        {
            get
            {

                return _selectedAccount;
            }
            set
            {
                if (_selectedAccount != null)
                    _selectedAccount.OnPropertyChange("PasswordPlainText");
                _selectedAccount = value;
                if (_selectedAccount != null)
                    _selectedAccount.OnPropertyChange("PasswordPlainText");

            }
        }
        /// <summary>
        /// MainWindowViewModel constructor
        /// </summary>
        /// <param name="dialogService">Dialog service injected from App.xaml.cs</param>
        public MainWindowViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;
            InitialLoad();
        }
        /// <summary>
        /// Load settings and acconut list
        /// </summary>
        public async void InitialLoad()
        {
            await LoadSettings();
            await OpenAccountList();
        }

      
        #region ICommands
        /// <summary>
        /// Lock or unlock form acconut list relay command
        /// </summary>
        public ICommand LockUnlockFormCommand
        {
            get
            {
                return new RelayCommand(p => true, p => this.LockUnlockForm());
            }
        }
        /// <summary>
        /// Open acconut list file relay command
        /// </summary>
        public ICommand OpenSALFileCommand
        {
            get
            {
                return new RelayCommand(p => true, p => this.OpenSALFile());
            }
        }
        /// <summary>
        /// Save new simple account locker file relay command.
        /// </summary>
        public ICommand SaveNewSALFileCommand
        {
            get
            {
                return new RelayCommand(p => true, p => this.SetFileLocation());
            }
        }
        /// <summary>
        /// ICommand DeleteSelectedRowCommand sends RelayCommand to call DeleteSelectedRow();
        /// </summary>
        public ICommand DeleteSelectedRowCommand
        {
            get
            {
                return new RelayCommand(p => true, p => this.DeleteSelectedRow());
            }
        }
        /// <summary>
        /// Add a new account to list relay command. 
        /// This command will open NewAccountDialogViewModel with DialogService.
        /// </summary>
        public ICommand NewAccountCommand
        {
            get
            {
                return new RelayCommand(p => true, p => this.NewAccountDialogView());
            }
        }
        /// <summary>
        /// Command to open dialog for new simple account locker file creation.
        /// </summary>
        public ICommand NewSALFileCommand
        {
            get
            {
                return new RelayCommand(p => true, p => this.NewSALFile());
            }
        }
        /// <summary>
        /// Command to call method to save account list.
        /// </summary>
        public ICommand SaveSALFileCommand
        {
            get
            {
                return new RelayCommand(p => true, p => this.SaveAccountList());
            }
        }

        /// <summary>
        /// ICommand SaveAccountListCommand sends RelayCommand to call SaveAccountList().
        /// </summary>
        public ICommand SaveAccountListCommand
        {
            get
            {
                return new RelayCommand(p => true, p => this.SaveAccountList());
            }
        }
        #endregion
        /// <summary>
        /// Method to lock and unlock account list.
        /// </summary>
        public async void LockUnlockForm()
        {
            if (!Locked)
            {
                Locked = true;
                AccountsList.Clear();
            }
            else
            {
                Locked = false;
                await OpenAccountList();
            }
        }
        /// <summary>
        /// Open dialog for a new simple account locker file with DialogViewModel and DialogService.
        /// </summary>
        public void NewSALFile()
        {
            var viewModel = new DialogViewModel(true);
            IDialog result = dialogService.ShowDialog(viewModel);
            if (result != null)
            {
                if (result.ShowDialog().Value == true)
                {

                    // Accepted
                    // MessageBox.Show("Accepted " + (string)result.Content);
                    settings.saveFileLocation = (string)result.Content;
                    SaveSettings();
                    // await OpenAccountList();
                    AccountsList.Clear();
                    SaveAccountList();
                }
                else
                {
                    //  MessageBox.Show("False");
                }
            }
        }
        /// <summary>
        /// Method to open simple account locker file.
        /// Uses DialogViewModel and DialogService.
        /// </summary>
        public async void OpenSALFile()
        {
            var viewModel = new DialogViewModel(false);

            IDialog result = dialogService.ShowDialog(viewModel);
            if (result != null)
            {
                if (result.ShowDialog().Value == true)
                {
                    settings.saveFileLocation = (string)result.Content;
                    SaveSettings();
                    await OpenAccountList();
                }
                else
                {
                    //Future location of user notification about sal file open failure.
                }
            }
        }
        /// <summary>
        /// Opens NewAccountDialogViewModel with DialogService, following MVVM pattern.
        /// </summary>
        public void NewAccountDialogView()
        {
            var viewModel = new NewAccountDialogViewModel();
            IDialog result = dialogService.ShowDialog(viewModel);
            if (result != null)
            {
                if (result.ShowDialog().HasValue && result.Content != null && result.Content.GetType() == typeof(AccountModel))
                {

                    AccountsList.Add((AccountModel)result.Content);
                    //because acconutlist is subscribed to onchange event, listener will be notified and saved.
                }
                else
                {
                    //Future location of error notifier for new account dialog view
                }
            }
        }
        /// <summary>
        /// Deletes the currently selected row from ObservableCollection<AccountList>
        /// </summary>
        public void DeleteSelectedRow()
        {
            if (AccountsList.Count(s => s.isSelected) > 0)
            {
                List<AccountModel> selectedAccounts = AccountsList.Where(s => s.isSelected).ToList();
                foreach (AccountModel acct in selectedAccounts)
                {
                    AccountsList.Remove(acct);
                }
            }
        }
        /// <summary>
        /// Settings dialog which opens DialogViewModel while following MVVM design pattern.
        /// </summary>
        public void SetFileLocation()
        {
            var viewModel = new DialogViewModel(true);
            IDialog result = dialogService.ShowDialog(viewModel);
            if (result != null)
            {
                if (result.ShowDialog().HasValue)
                {

                    settings.saveFileLocation = (string)result.Content;
                    SaveSettings();
                    //   MessageBox.Show("Accepted " + settings.saveFileLocation);
                }
                else
                {
                    //MessageBox.Show("False");
                }
            }
        }
        #region OpenSaveMethods
        /// <summary>
        /// Asynchronous save of ObservableCollection<AccountList>.
        /// </summary>
        public async void SaveAccountList()
        {
            if (AccountsList == null)
                return;

            //remove all empty rows.
            for(int i = AccountsList.Count -1; i >= 0; i--)
            {
                if (AccountsList[i].IsEmpty)
                    AccountsList.RemoveAt(i);
            }
            KeyFile key;
            if (File.Exists(settings.keyLocation))
            {
                //load key from appdata.
                key = await CryptoTools.LoadStoredKeyFile(settings.keyLocation);
            }
            else
            {
                //create new keyfile if none created yet in appdata.
                key = await CryptoTools.CreateStoredKeyFile(settings.keyLocation);
            }
            //encrypt accountlist into byte array.
            byte[] listAsBytes = CryptoTools.EncryptData(key.key, key.IV, AccountsList);
            //save encrypted accountlist
            await FileManager.SaveFileAsync(settings.saveFileLocation, listAsBytes, false);
        }
        /// <summary>
        /// Opens and decrypts AccountList asynchronous from file and loads into ObservableCollection<AccountList>. 
        /// If null, will create new ObservableCollection<AccountModel>()
        /// </summary>
        public async Task<bool> OpenAccountList()
        {
            if (settings != null)
            {
                byte[] o = await FileManager.OpenFile(settings.saveFileLocation);
                KeyFile key = await CryptoTools.LoadStoredKeyFile(settings.keyLocation);
               // MessageBox.Show(Convert.ToBase64String(key.key));
                if (o != null && o.Length > 0 && key != null)
                {
                    object decryptedData = CryptoTools.DecryptData(key.key, key.IV, o);
                    if (decryptedData != null)
                    {
                        AccountsList = (BetterObservableCollection<AccountModel>)decryptedData;
                    }
                }
                else
                {
                    AccountsList = new BetterObservableCollection<AccountModel>();
                    SaveAccountList();
                }
                Locked = false;
            }
            AccountsList.CollectionChanged += AccountListChanged;
            return true;
        }
        /// <summary>
        /// Load app settings.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> LoadSettings()
        {
            byte[] o = await FileManager.OpenFile(Settings.SettingsFileURI);
            if (o == null || o.Length <= 0)
            {
                settings = new Settings();
                SaveSettings();
            }
            else
            {
                BinaryFormatter bf = new BinaryFormatter();
                settings = (Settings)bf.Deserialize(new MemoryStream(o));
                if (settings == null)
                {
                    settings = new Settings();
                    SaveSettings();
                }
            }

            o = await FileManager.OpenFile(settings.keyLocation);
            if (o == null || o.Length < 0)
            {
               await CryptoTools.CreateStoredKeyFile(settings.keyLocation); //new KeyFile();
            }
         /*   else
            {
                keyFile = await CryptoTools.LoadStoredKeyFile(settings.keyLocation);
              //  BinaryFormatter bf = new BinaryFormatter();
             //   keyFile = (KeyFile)bf.Deserialize(new MemoryStream(o));
              //  if(keyFile == null)
               // {
               //     keyFile = new KeyFile();
               // }
            }*/
            return true;
        }
        /// <summary>
        /// Save app settings
        /// </summary>
        public async void SaveSettings()
        {
            await FileManager.SaveFileAsync(Settings.SettingsFileURI, FileManager.Serialize(settings), true);
        }
        #endregion

        public void GenerateNewKey()
        {
            
        }
        /// <summary>
        /// Event subscriber to be notified if AccountList has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void AccountListChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if(AccountsList != null && !Locked)
            {
                SaveAccountList();
            }
        }



    }
}
