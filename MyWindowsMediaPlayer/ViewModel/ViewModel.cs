using MyWindowsMediaPlayer.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyWindowsMediaPlayer.ViewModel
{
    public static class ViewModel
    {
        public static void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow.DataContext = this;
            try
            {
                MainWindow._ofd = new Ofd();
                var toto = new string("toto");
                _sh = new Shell();
                me.Loaded += MediaElement_Loaded;
                #region setup Library
                _libs = new LinkedList<Library>();
                var lib = new Library("Ma Musique", _sh);
                lib.setLibrary(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
                CurrentLib = new LinkedListNode<Library>(lib);
                _libs.AddLast(CurrentLib);
                #region linkedlist tests
                var myVideos = new Library("Mes Vidéos", _sh);
                myVideos.setLibrary(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));
                _libs.AddLast(myVideos);
                #endregion linkedlist tests
                #endregion setup library
                lb_playlist.PreviewKeyDown += lb_playlistEntry;
                isDraggingSlider = false;
                #region setup Timer
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += Timer_Tick;
                #endregion setup Timer
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception levée dans MainWindow_Loaded:\n" + ex.Message);
            }
        }
    }
}
