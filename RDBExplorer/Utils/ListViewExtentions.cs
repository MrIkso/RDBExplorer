using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace RDBExplorer.Utils
{
    internal class ListViewExtentions
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct LVITEM
        {
            public uint mask;
            public int iItem;
            public int iSubItem;
            public uint state;
            public uint stateMask;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszText;
            public int cchTextMax;
            public int iImage;
            public IntPtr lParam;
            public int iIndent;
            public int iGroupId;
            public uint cColumns;
            public IntPtr puColumns;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, ref LVITEM lParam);

        public const int LVM_SETITEMSTATE = 0x1000 + 43;
        public const uint LVIS_SELECTED = 0x0002;
    }
}
