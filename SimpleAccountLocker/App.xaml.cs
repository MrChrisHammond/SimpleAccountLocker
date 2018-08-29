using SimpleAccountLocker.Model;
using SimpleAccountLocker.View;
using SimpleAccountLocker.View.DialogViews;
using SimpleAccountLocker.Viewmodel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleAccountLocker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            IDialogService dialogService = new DialogService(MainWindow);
            dialogService.Register<DialogViewModel, DialogView>();
            dialogService.Register<NewAccountDialogViewModel, NewAccountDialogView>();
            var viewModel = new MainWindowViewModel(dialogService);
            var view = new MainWindow { DataContext = viewModel };
            view.ShowDialog();
           // base.OnStartup(e);
        }
    }
}
