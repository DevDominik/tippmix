using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Tippmixx
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Task backgroundTask;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            backgroundTask = Task.Run(() =>
            {
                DataHandler.Initialize();
            });
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (backgroundTask != null && !backgroundTask.IsCompleted)
            {
                backgroundTask.Wait();
            }
            base.OnExit(e);
        }
    }
}
