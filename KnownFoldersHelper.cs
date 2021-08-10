using System;
using System.Runtime.InteropServices;

namespace PasteDownload
{
    public static class KnownFoldersHelper
    {
        public static readonly Guid Downloads = new Guid("374DE290-123F-4565-9164-39C4925E467B");

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out string pszPath);

        public static string GetPath(Guid folderId)
        {
            string path;
            SHGetKnownFolderPath(folderId, 0, IntPtr.Zero, out path);
            return path;
        }
    }



}
