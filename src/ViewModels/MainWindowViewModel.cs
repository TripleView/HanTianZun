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
        /// <summary>
        /// 命名助手中，需要转命名的原始语句
        /// </summary>
        public string OriginalMethodName
        {
            set
            {
                this.RaiseAndSetIfChanged(ref originalMethodName, value);
            }
            get => originalMethodName;
        }

        private string originalMethodName;

        private string convertMethodName;
        public string ConvertMethodName {
            set
            {
                this.RaiseAndSetIfChanged(ref convertMethodName, value);
            }
            get => convertMethodName;
        }
        public ICommand CopyTextCommand { get; }
        public ICommand PasteTextCommand { get; set; }
        public ICommand ClearTextCommand { get; set; }
        public ICommand AddNewItem { get; }

        public ICommand DeleteItemCommand { get; }
        /// <summary>
        /// 将英文句子转换为大写字母开头的方法名
        /// </summary>
        public ICommand ConvertWordsToMethodNameWithCapitalLetterFunc { get; }
        /// <summary>
        /// 将英文句子转换为小写字母开头的方法名
        /// </summary>
        public ICommand ConvertWordsToMethodNameWithLowercaseLetterFunc { get; }

        /// <summary>
        /// 清空转换前后的文本
        /// </summary>
        public ICommand ClearConvertTextsFunc { get; }
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

            ConvertWordsToMethodNameWithCapitalLetterFunc = ReactiveCommand.Create(async () =>
            {
                if (!string.IsNullOrWhiteSpace(OriginalMethodName))
                {
                    ConvertMethodName = string.Join("",
                        OriginalMethodName.Split(' ').Select(x =>
                            GetWordsStartingWithUppercase(x)));
                    await SetTextToClipboard(ConvertMethodName);
                }
            });
            ConvertWordsToMethodNameWithLowercaseLetterFunc = ReactiveCommand.Create(async () =>
            {
                if (!string.IsNullOrWhiteSpace(OriginalMethodName))
                {
                    ConvertMethodName = GetWordsStartingWithLowercase(string.Join("",
                        OriginalMethodName.Split(' ').Select(x =>
                            GetWordsStartingWithUppercase(x))));
                    await SetTextToClipboard(ConvertMethodName);
                }
            });
            ClearConvertTextsFunc = ReactiveCommand.Create((() =>
            {
                this.ConvertMethodName = "";
                this.OriginalMethodName = "";
            }));
        }

        private string GetWordsStartingWithUppercase(string word)
        {
            if (!string.IsNullOrWhiteSpace(OriginalMethodName))
            {
                return word[0].ToString().ToUpper() + word.Substring(1);
            }

            return word;

        }

        private string GetWordsStartingWithLowercase(string word)
        {
            if (!string.IsNullOrWhiteSpace(OriginalMethodName))
            {
                return word[0].ToString().ToLower() + word.Substring(1);
            }

            return word;

        }

        public async Task CopyText(int index)
        {
            var txt = Items[index].Text;
            await SetTextToClipboard(txt);
        }

        private async Task SetTextToClipboard(string txt)
        {
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
                desktop.MainWindow?.Clipboard is not { } provider)
            {
                throw new NullReferenceException("Missing Clipboard instance.");
            }
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
