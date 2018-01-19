using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;

namespace WinHue3.Functions.HotKeys
{
    public class HotKeyHandle : IDisposable
    {
        private static Dictionary<int, HotKeyHandle> _dictHotKeyToCalBackProc;

        private const int WmHotKey = 0x0312;

        private bool _disposed = false;

        public Key Key { get; private set; }
        public ModifierKeys KeyModifiers { get; private set; }
        private Action<HotKeyHandle> Action { get; set; }
        private int Id { get; set; }

        // ******************************************************************
        public HotKeyHandle(HotKey h, Action<HotKeyHandle> action, bool register = true)
        {
            Key = h.Key;
            KeyModifiers = h.Modifier;
            Action = action;
        }

        // ******************************************************************
        public bool Register()
        {
            int virtualKeyCode = KeyInterop.VirtualKeyFromKey(Key);
            Id = virtualKeyCode + ((int)KeyModifiers * 0x10000);
            bool result = NativeMethods.RegisterHotKey(IntPtr.Zero, Id, (uint)KeyModifiers, (uint)virtualKeyCode);

            if (_dictHotKeyToCalBackProc == null)
            {
                _dictHotKeyToCalBackProc = new Dictionary<int, HotKeyHandle>();
                ComponentDispatcher.ThreadFilterMessage += new ThreadMessageEventHandler(ComponentDispatcherThreadFilterMessage);
            }

            if (result)
            {
                _dictHotKeyToCalBackProc.Add(Id, this);
                Debug.Print(result.ToString() + ", " + Id + ", " + virtualKeyCode);
            }

            return result;
        }

        // ******************************************************************
        public void Unregister()
        {
            if (_dictHotKeyToCalBackProc.TryGetValue(Id, out HotKeyHandle hotKey))
            {
                if (NativeMethods.UnregisterHotKey(IntPtr.Zero, Id))
                {
                    _dictHotKeyToCalBackProc.Remove(Id);
                }
            }
        }

        // ******************************************************************
        private static void ComponentDispatcherThreadFilterMessage(ref MSG msg, ref bool handled)
        {
            if (!handled)
            {
                if (msg.message == WmHotKey)
                {

                    if (_dictHotKeyToCalBackProc.TryGetValue((int)msg.wParam, out HotKeyHandle hotKey))
                    {
                        if (hotKey.Action != null)
                        {
                            hotKey.Action.Invoke(hotKey);
                        }
                        handled = true;
                    }
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    Unregister();
                }

                _disposed = true;
            }
        }

        internal static class NativeMethods
        {
            [DllImport("user32.dll")]
            internal static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vlc);

            [DllImport("user32.dll")]
            internal static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        }

    }



    // ******************************************************************
    [Flags]
    public enum KeyModifier
    {
        None = 0x0000,
        Alt = 0x0001,
        Ctrl = 0x0002,
        NoRepeat = 0x4000,
        Shift = 0x0004,
        Win = 0x0008
    }

    // ******************************************************************
}