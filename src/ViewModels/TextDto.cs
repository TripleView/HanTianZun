using ReactiveUI;
using System.Xml.Linq;

namespace HanTianZun.ViewModels;

public class TextDto:ViewModelBase
{
    private int index;

    public int Index
    {
        get
        {
            return index;
        }
        set
        {
            this.RaiseAndSetIfChanged(ref index,value);
        }
    }

    private string text;

    public string Text
    {
        get
        {
            return text;
        }
        set
        {
            this.RaiseAndSetIfChanged(ref text, value);
        }
    }

    private void Te()
    {
        using (DelayChangeNotifications()) // 批量更新时不立即触发通知
        {
        } // 此处统一触发 PropertyChanged 事件
    }
}