using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Maxine.LogViewer.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class LogViewerApp : Application
    {
        internal Action<ItemsControlLogBroker> OnLoad { get; }

        public LogViewerApp(Action<ItemsControlLogBroker> onLoad)
        {
            OnLoad = onLoad;
        }
    }
}