using Shell32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace MyWindowsMediaPlayer
{
    public class Library
    {
        #region properties
        private Stack<string> historyPathLib;
        public Shell sh { get; set; }
        public string Name { get; set; }
        public ObservableCollection<File> playlist { get; set; }
        public int Index { get; set; }
        #endregion properties
        public Library(string lb_name, Shell shell)
        {
            Index = 0;
            historyPathLib = new Stack<string>();
            playlist = new ObservableCollection<File>();
            Name = lb_name;
            sh = shell;
        }

        /// <summary>
        /// Fill the playlist with directories and files found in the mediaPath directory
        /// but only files with extensions from the Ofd.mediaExt string
        /// </summary>
        /// <param name="mediaPath">The path to the directory</param>
        public void setLibrary(string mediaPath)
        {
            try
            {
                string mediaExt;
                string[] mediaExtList;
                File n;
                DirectoryInfo dirInfo;
                DirectoryInfo[] dirs;
                FileInfo[] files;

                playlist.Clear();
                if (!historyPathLib.Contains(mediaPath))
                    historyPathLib.Push(mediaPath);
                mediaExt = Ofd.mediaExt.Replace("*", string.Empty);
                mediaExtList = mediaExt.Split(new[] { ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                dirInfo = new DirectoryInfo(mediaPath);
                dirs = dirInfo.GetDirectories();
                files = dirInfo.GetFiles();
                foreach (DirectoryInfo item in dirs)
                {
                    n = new File(sh, item);
                    playlist.Add(n);
                }
                foreach (FileInfo item in files)
                {
                    if (mediaExtList.Any(item.Extension.Contains))
                    {
                        n = new File(sh, item);
                        playlist.Add(n);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception levée dans setLibray:\n" + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public void Previous()
        {
            if (historyPathLib.Count > 1)
                historyPathLib.Pop();
            if (historyPathLib.Count >= 1)
                setLibrary(historyPathLib.Peek());
        }

        public File PreviousMedia()
        {
            if (Index == 0)
                return (null);
            --Index;
            if (playlist.ElementAt(Index) == null)
                return (null);
            return (playlist.ElementAt(Index));
        }

        public File NextMedia()
        {
            if (Index == playlist.Count)
                return (null);
            ++Index;
            if (playlist.ElementAt(Index) == null)
                return (null);
            return (playlist.ElementAt(Index));
        }
    }
}
