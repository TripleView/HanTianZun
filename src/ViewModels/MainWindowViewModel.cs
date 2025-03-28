using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;

namespace HanTianZun.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting { get; } = "Welcome to Avalonia!";
        public ObservableCollection<TextDto> Items { set; get; } = new ObservableCollection<TextDto>() { };

        public ICommand CopyTextCommand { get; }
        public ICommand PasteTextCommand { get; set; }
        public ICommand ClearTextCommand { get; set; }
        public ICommand AddNewItem { get; }

        public ICommand DeleteItemCommand { get; }
        public MainWindowViewModel()
        {
            Items.CollectionChanged += (s, e) =>
            {
                foreach (var (item, idx) in Items.Select((x, i) => (x, i)))
                    item.Index = idx;
            };
            CopyTextCommand = ReactiveCommand.Create((async (int index) =>
            {
                await CopyText(index);
            }));
            PasteTextCommand = ReactiveCommand.Create(async (int index) =>
            {
                await PasteText(index);
            });

            ClearTextCommand = ReactiveCommand.Create(async (int index) =>
            {
                await ClearText(index);
            });

            DeleteItemCommand = ReactiveCommand.Create(async (int index) =>
            {
                await DeleteItem(index);
            });

            AddNewItem = ReactiveCommand.Create(async () =>
            {
                Items.Add(new TextDto());
            });

            //初始化
            for (int i = 0; i < 3; i++)
            {
                Items.Add(new TextDto());
            }
           
        }

        public async Task CopyText(int index)
        {
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
                desktop.MainWindow?.Clipboard is not { } provider)
                throw new NullReferenceException("Missing Clipboard instance.");

            var txt = Items[index].Text;
            await provider.SetTextAsync(txt);
        }

        public async Task PasteText(int index)
        {
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
                desktop.MainWindow?.Clipboard is not { } provider)
            {
                throw new NullReferenceException("Missing Clipboard instance.");
            }

            var txt = await provider.GetTextAsync();
            this.Items[index].Text = txt;
        }
        public async Task ClearText(int index)
        {
            this.Items[index].Text = "";
        }

        public async Task DeleteItem(int index)
        {
            this.Items.RemoveAt(index);
        }
        
    }
}
