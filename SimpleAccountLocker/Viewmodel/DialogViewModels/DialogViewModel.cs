using Microsoft.Win32;
using SimpleAccountLocker.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace SimpleAccountLocker.Viewmodel
{
    /// <summary>
    /// A simple dialog viewmodel.
    /// Currently only functionality includes opening file or creating new file.
    /// </summary>
    public class DialogViewModel : BaseViewModel, IDialogRequestClose
    {
        protected bool _createNewFile;
        /// <summary>
        /// Constructor. Must specify whether creating new file otherwise Windows UI OpenFileDialog will be used.
        /// Uses Windows default UI SaveFileDialog and OpenFileDialog for file browse.
        /// </summary>
        /// <param name="createNewFile">Specify whether to use Windows SaveFileDialog OpenFileDialog.</param>
        public DialogViewModel(bool createNewFile)
        {
            _createNewFile = createNewFile;
           // SelectFileLocationCommand = new RelayCommand(p => true, p => this.FileLocationURI());
        }
        public DialogViewModel()
        {

        }
        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;
        private string _fileLocationURI;
        public string FileLocationURI {
            get
            {
                return _fileLocationURI;
            }
            set
            {
                _fileLocationURI = value;
                OnPropertyChange("FileLocationURI");
               
            }
        }
        public ICommand SaveCommand {
            get
            {
                return new RelayCommand(p => true, p => CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true, FileLocationURI)));
            }
        }
        public ICommand CancelCommand {
            get
            {
                return new RelayCommand(p => true, p => CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(false, FileLocationURI)));
            }
        }
        public ICommand SelectFileLocationCommand {
            get
            {
                return new RelayCommand(p => true, p => this.CreateOrOpenFile(_createNewFile));
            }
        }
        public void CreateOrOpenFile(bool createNewFile) {

            //create empty file and get URI for open/save of SAL file.
            if (createNewFile)
            {
                Stream myStream;
                System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();

                saveFileDialog1.Filter = "SimpleAccountLocker files (*.SAL)|*.SAL|All files (*.*)|*.*";
                saveFileDialog1.FilterIndex = 2;
                saveFileDialog1.RestoreDirectory = true;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if ((myStream = saveFileDialog1.OpenFile()) != null)
                    {
                        FileLocationURI = saveFileDialog1.FileName;
                        myStream.Close();
                    }
                }
            }
            //get URI of existing SAL file.
            else
            {
                System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                    FileLocationURI = openFileDialog.FileName;
            }
        
        }
     
    }

}
