using Autofac;
using digibox.services.Models;
using digibox.wind.Modules;
using digibox.wind.View;
using digibox.wind.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace digibox.wind
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
           IContainer container = iocInit.ConfigureContainer();
            using (var scope = container.BeginLifetimeScope())
            {
                var _mainWindow = scope.Resolve<MainWindow>();
                var _mainViewModel = scope.Resolve<MainViewModel>();
                _mainWindow.DataContext = _mainViewModel;
                _mainWindow.Show();
            }
        }
    }
}

