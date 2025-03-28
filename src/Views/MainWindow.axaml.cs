using System;
using System.Diagnostics;
using System.Reactive;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using HanTianZun.Service;
using ReactiveUI;

namespace HanTianZun.Views
{
    public partial class MainWindow : Window
    {
        private WindowsGlobalHotKeyManager hotKeyManager;
        public MainWindow()
        {
           
            InitializeComponent();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                hotKeyManager = new WindowsGlobalHotKeyManager(this);

                var d = hotKeyManager.RegisterHotKey(1, Key.W, KeyModifiers.Control, (() =>
                {
                    Dispatcher.UIThread.InvokeAsync((() =>
                    {
                        if (this.WindowState != WindowState.Minimized)
                        {
                            this.WindowState = WindowState.Minimized;
                        }
                        else
                        {
                            this.WindowState = WindowState.Normal;
                        }
                    }));
                }));
            }
            
           
        }

        protected override void OnClosing(WindowClosingEventArgs e)
        {
            base.OnClosing(e);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                hotKeyManager.UnregisterAllHotkey();
            }
        }
    }
}