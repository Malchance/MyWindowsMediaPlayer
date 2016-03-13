using System;
using System.IO;
using Shell32;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MyWindowsMediaPlayer
{
    public class File
    {
        #region Properties
        private Shell sh;
        public SolidColorBrush TextColor { get; set; }
        public SolidColorBrush BackgroundColor { get; set; }
        public string Name { get; private set; }
        public string FullName { get; private set; }

        public bool IsDirectory { get; private set; }

        public string Extension { get; private set; }

        public string Size { get; private set; }

        public string LastWriteTime { get; private set; }

        public string Type { get; private set; }

        public string Duration { get; private set; }
        #endregion Properties
        public Image img { get; set; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe File à partir d'un FileInfo.
        /// </summary>
        /// <param name="shell">Le Shell permettant de récupérer des informations sur le fichier.</param>
        /// <param name="file">Le nom du chemin complet vers le fichier.</param>
        public File(Shell shell, FileInfo file)
        {
            TimeSpan ts;
            sh = shell;
            var folder = sh.NameSpace(Path.GetDirectoryName(file.FullName));
            var item = folder.ParseName(Path.GetFileName(file.FullName));

            Name = folder.GetDetailsOf(item, 0);
            FullName = file.FullName;
            Extension = file.Extension;
            Size = folder.GetDetailsOf(item, 1);
            Type = folder.GetDetailsOf(item, 2);
            LastWriteTime = folder.GetDetailsOf(item, 3);
            var propValue = folder.GetDetailsOf(item, 27);
            if (TimeSpan.TryParse(propValue, out ts))
                Duration = ts.ToString();
            TextColor = Brushes.DarkBlue;
            IsDirectory = false;
            #region test
            //TagLib.File f = new TagLib.Mpeg.AudioFile(FullName);
            //if (f.Tag.Pictures.Length > 0)
            //{
            //    try
            //    {
            //        TagLib.IPicture pic = f.Tag.Pictures[0];
            //        MemoryStream ms = new MemoryStream(pic.Data.Data);
            //        BitmapImage bitmap = new BitmapImage();
            //        bitmap.BeginInit();
            //        bitmap.StreamSource = ms;
            //        bitmap.EndInit();
            //        ms.Close();
            //        img = new Image();
            //        img.Source = bitmap;
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine("Exception levée dans File:\n" + ex.Message);
            //    }
            //}
            //else
            //    Console.WriteLine("f.Tag.Pictures null");
            #endregion
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe File à partir d'un DirectoryInfo.
        /// </summary>
        /// <param name="shell">Le Shell permettant de récupérer des informations sur le fichier.</param>
        /// <param name="dir">Le nom du chemin complet vers le dossier.</param>
        public File(Shell shell, DirectoryInfo dir)
        {
            TimeSpan ts;
            sh = shell;
            var folder = sh.NameSpace(Path.GetDirectoryName(dir.FullName));
            var item = folder.ParseName(Path.GetFileName(dir.FullName));

            Name = folder.GetDetailsOf(item, 0);
            FullName = dir.FullName;
            Extension = null;
            Size = folder.GetDetailsOf(item, 1);
            Type = folder.GetDetailsOf(item, 2);
            LastWriteTime = folder.GetDetailsOf(item, 3);
            var propValue = folder.GetDetailsOf(item, 27);
            if (TimeSpan.TryParse(propValue, out ts))
                Duration = ts.ToString();
            TextColor = Brushes.DodgerBlue;
            IsDirectory = true;
        }
    }
}
