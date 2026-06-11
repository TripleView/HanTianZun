using System.Runtime.InteropServices;
using System;
using System.Threading;

namespace HanTianZun.Service.Windows;

public class KeyboardSender
{
    // 导入keybd_event函数
    [DllImport("user32.dll", SetLastError = true)]
    public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

    // 虚拟键码
    const byte VK_CONTROL = 0x11;
    const byte VK_V = 0x56;
    const uint KEYEVENTF_KEYDOWN = 0x0000;
    const uint KEYEVENTF_KEYUP = 0x0002;
    const int VK_C = 0x43;
    const byte VK_RIGHT = 0x27;
    public static void SendCtrlAndV()
    {
        // 按下Ctrl
        keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
        //Thread.Sleep(50);
        // 按下V
        keybd_event(VK_V, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
        //Thread.Sleep(50);
        // 松开V
        keybd_event(VK_V, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        //Thread.Sleep(50);
        // 松开Ctrl
        keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        //Thread.Sleep(50);
        // 模拟右箭头
        keybd_event(VK_RIGHT, 0, 0, 0);
        keybd_event(VK_RIGHT, 0, KEYEVENTF_KEYUP, 0);

    }
    public static void SendCtrlAndC()
    {
        // 按下Ctrl
        keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
        // 按下C
        keybd_event(VK_C, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);

        // 松开C
        keybd_event(VK_C, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        // 松开Ctrl
        keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
    }
}