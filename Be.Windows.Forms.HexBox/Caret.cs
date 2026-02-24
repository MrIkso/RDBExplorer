using System;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;

namespace Be.Windows.Forms
{
    internal static class Caret
    {
        public static bool Create(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight)
        {
            try
            {
                //GC.Collect();
                //GC.WaitForPendingFinalizers();
                //GC.Collect();
                return PInvoke.CreateCaret(new HWND(hWnd), new HBITMAP(hBitmap), nWidth, nHeight);
            }
            catch (Exception ex)
            {
                // Let's pretend CreateCaret() is available on Linux
                if ((ex is DllNotFoundException) || (ex is EntryPointNotFoundException))
                    return true;

                throw;
            }
        }

        public static bool Show(IntPtr hWnd)
        {
            try
            {
                //GC.Collect();
                //GC.WaitForPendingFinalizers();
                //GC.Collect();
                return PInvoke.ShowCaret(new HWND(hWnd));
            }
            catch (Exception ex)
            {
                // Let's pretend ShowCaret() is available on Linux
                if ((ex is DllNotFoundException) || (ex is EntryPointNotFoundException))
                    return true;

                throw;
            }
        }

        public static bool Destroy()
        {
            try
            {
                //GC.Collect();
                //GC.WaitForPendingFinalizers();
                //GC.Collect();
                return PInvoke.DestroyCaret();
            }
            catch (Exception ex)
            {
                // Let's pretend DestroyCaret() is available on Linux
                if ((ex is DllNotFoundException) || (ex is EntryPointNotFoundException))
                    return true;

                throw;
            }
        }

        public static bool SetPos(int X, int Y)
        {
            try
            {
                //GC.Collect();
                //GC.WaitForPendingFinalizers();
                //GC.Collect();
                return PInvoke.SetCaretPos(X, Y);
            }
            catch (Exception ex)
            {
                // Let's pretend SetCaretPos() is available on Linux
                if ((ex is DllNotFoundException) || (ex is EntryPointNotFoundException))
                    return true;

                throw;
            }
        }
    }
}
