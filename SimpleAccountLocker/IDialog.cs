using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleAccountLocker
{
    public interface IDialog
    {
        object DataContext { get; set; }
        bool? DialogResult { get; set; }
        object Content { get; set; }
        Window Owner { get; set; }
        void Close();
         bool? ShowDialog();
      //  DialogResults ShowDialog();
    }
    public interface IDialogService
    {
        void Register<TViewModel, TView>() where TViewModel : IDialogRequestClose
                                           where TView : IDialog;

       // bool? ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : IDialogRequestClose;
        IDialog ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : IDialogRequestClose;

    }
    public class DialogResults{/* : IDialog
    {
        public object DataContext { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string DialogMessageResult { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Window Owner { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
         bool? IDialog.DialogResult { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public DialogResults ShowDialog()
        {
            throw new NotImplementedException();
        }*/
    }

    public interface IDialogRequestClose
    {
        event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;
    }
    public class DialogCloseRequestedEventArgs : EventArgs
    {
        public bool? DialogResult { get; }
        public object DialogContentResult { get; }
        public DialogCloseRequestedEventArgs(bool? dialogResult, string message)
        {
            DialogResult = dialogResult;
            DialogContentResult = message;
        }
        public DialogCloseRequestedEventArgs(bool? dialogResult, object content)
        {
            DialogResult = dialogResult;
            DialogContentResult = content;
        }

    }
    public class DialogService : IDialogService
    {
        private readonly Window owner;
        public DialogService(Window owner)
        {
            this.owner = owner;
            Mappings = new Dictionary<Type, Type>();
        }
        public IDictionary<Type, Type> Mappings { get; }

        public void Register<TViewModel, TView>()
            where TViewModel : IDialogRequestClose
            where TView : IDialog
        {
            if (Mappings.ContainsKey(typeof(TViewModel)))
            {
                throw new ArgumentException($"Type { typeof(TViewModel) } is already mapped");
            }
            Mappings.Add(typeof(TViewModel), typeof(TView));
        }

      /*  public bool? ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : IDialogRequestClose
        {
            Type viewType = Mappings[typeof(TViewModel)];
            IDialog dialog = (IDialog)Activator.CreateInstance(viewType);
            EventHandler<DialogCloseRequestedEventArgs> handler = null;

            handler = (sender, e) =>
            {
                viewModel.CloseRequested -= handler;

                if (e.DialogResult.HasValue)
                {
                    dialog.DialogResult = e.DialogResult;
                  //  dialog.DialogMessageResult = e.DialogMessageResult;
                }
                else
                {
                    dialog.Close();
                }
            };
                viewModel.CloseRequested += handler;
                dialog.DataContext = viewModel;
                dialog.Owner = owner;

                return dialog.ShowDialog();
            
        }*/
       public IDialog ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : IDialogRequestClose
        {
            Type viewType = Mappings[typeof(TViewModel)];
            IDialog dialog = (IDialog)Activator.CreateInstance(viewType);
            EventHandler<DialogCloseRequestedEventArgs> handler = null;

            handler = (sender, e) =>
            {
                viewModel.CloseRequested -= handler;

                if (e.DialogResult.HasValue)
                {
                    dialog.DialogResult = e.DialogResult;
                    dialog.Content = e.DialogContentResult;
                }
                else
                {
                    dialog.Close();
                }
            };
            viewModel.CloseRequested += handler;
            dialog.DataContext = viewModel;
            dialog.Owner = owner;

            return dialog;//ShowDialog();

        }
    }


}
