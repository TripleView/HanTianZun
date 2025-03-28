using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform;
using Avalonia.Win32.Input;
using Avalonia;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using Avalonia.Win32.Input;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Avalonia.Media.Transformation;

namespace HanTianZun.Service;
public class WindowsGlobalHotKeyManager: IGlobalHotKeyManager
{
    private const int WM_HOTKEY = 0x0312;

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private IntPtr _windowHandle;
    private TopLevel topLevel;
    private Dictionary<int, Action> ActionMaps = new Dictionary<int, Action>();

    public WindowsGlobalHotKeyManager(TopLevel topLevel)
    {
        this.topLevel = topLevel;
        _windowHandle = topLevel.TryGetPlatformHandle()!.Handle;
        Win32Properties.AddWndProcHookCallback(topLevel, ((IntPtr wnd, uint msg, IntPtr param, IntPtr lParam,
            ref bool handled) =>
        {
            if (wnd == _windowHandle && msg == WM_HOTKEY) // WM_HOTKEY
            {
                if (ActionMaps.TryGetValue(param.ToInt32(), out var callbackAction))
                {
                    callbackAction();
                    handled = true;
                }
            }
            return _windowHandle;
        }));
    }

    public bool RegisterHotKey(int id, Key key, KeyModifiers modifiers, Action callbackAction)
    {
        if (ActionMaps.TryGetValue(id,out _))
        {
            throw new Exception($"Duplicate registration ID {id}");
        }

    
        uint virtualKeyCode = (uint)KeyInterop.VirtualKeyFromKey(key);
        uint modifier = (uint)modifiers;

        var result = RegisterHotKey(_windowHandle, id, modifier, virtualKeyCode);

        ActionMaps.Add(id, callbackAction);

        return result;
    }

    public void UnregisterHotkey(int id)
    {
        UnregisterHotKey(_windowHandle, id);
        ActionMaps.Remove(id);
    }

    public void UnregisterAllHotkey()
    {
        var ids= ActionMaps.Select(it => it.Key).ToList();
        foreach (var id in ids)
        {
            UnregisterHotkey(id);
        }
    }

}