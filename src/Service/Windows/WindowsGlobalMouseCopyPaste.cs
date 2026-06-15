using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace HanTianZun.Service.Windows;

public sealed class WindowsGlobalMouseCopyPaste : IGlobalMouseCopyPaste
{
    /// <summary>
    /// 是否拦截并吞掉原始鼠标事件（true 可避免中键自动滚动等默认行为）
    /// </summary>
    private readonly bool _suppressOriginal;

    private nint _hookId = nint.Zero;
    private LowLevelMouseProc? _proc;
    public WindowsGlobalMouseCopyPaste(
        bool suppressOriginal = true
       )
    {
        _suppressOriginal = suppressOriginal;

        if (!OperatingSystem.IsWindows())
            return;

        _proc = HookCallback;
        _hookId = SetHook(_proc);
    }

    public void Dispose()
    {
        if (_hookId != nint.Zero)
        {
            UnhookWindowsHookEx(_hookId);
            _hookId = nint.Zero;
        }

        GC.SuppressFinalize(this);
    }

    ~WindowsGlobalMouseCopyPaste()
    {
        Dispose();
    }

    private nint SetHook(LowLevelMouseProc proc)
    {
        using var curProcess = Process.GetCurrentProcess();
        using var curModule = curProcess.MainModule!;
        return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
    }

    private nint HookCallback(int nCode, nint wParam, nint lParam)
    {

        if (nCode >= 0)
        {
            int msg = wParam.ToInt32();
            var data = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);

            // 中键按下 = 复制
            if (msg == WM_MBUTTONDOWN || msg == WM_MBUTTONUP)
            {
                if (msg == WM_MBUTTONDOWN)
                {
                    KeyboardSender.SendCtrlAndC();
                    Debug.WriteLine("开始复制");
                }

                return (IntPtr)1; // 吞掉事件
            }

            // 按下和弹起都要拦截
            if (msg == WM_XBUTTONDOWN || msg == WM_XBUTTONUP)
            {
                uint button = data.mouseData >> 16 & 0xFFFF;
                if (button == XBUTTON1)
                {
                    if (msg == WM_XBUTTONDOWN)
                    {
                        Debug.WriteLine("黏贴");
                        KeyboardSender.SendCtrlAndV();
                    }

                    return (IntPtr)1; // 吞掉事件

                }
            }

        }

        return CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    // Win32 P/Invoke

    private const int WH_MOUSE_LL = 14;

    private const int WM_MBUTTONDOWN = 0x0207;
    private const int WM_MBUTTONUP = 0x0208;
    private const int WM_XBUTTONDOWN = 0x020B;
    private const int WM_XBUTTONUP = 0x020C;
    private const uint XBUTTON1 = 0x0001;
    private const uint XBUTTON2 = 0x0002;

    private delegate nint LowLevelMouseProc(int nCode, nint wParam, nint lParam);

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public nint dwExtraInfo;
    }


    [DllImport("user32.dll", SetLastError = true)]
    private static extern nint SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, nint hMod, uint dwThreadId);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnhookWindowsHookEx(nint hhk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern nint CallNextHookEx(nint hhk, int nCode, nint wParam, nint lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern nint GetModuleHandle(string? lpModuleName);

}