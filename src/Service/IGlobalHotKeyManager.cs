using Avalonia.Input;
using System;

namespace HanTianZun.Service;

public interface IGlobalHotKeyManager
{
    bool RegisterHotKey(int id, Key key, KeyModifiers modifiers, Action callbackAction);
    void UnregisterHotkey(int id);

    void UnregisterAllHotkey();
}