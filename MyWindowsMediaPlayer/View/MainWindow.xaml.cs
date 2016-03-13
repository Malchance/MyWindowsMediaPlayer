using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shell32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace MyWindowsMediaPlayer.View
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region properties
        private LinkedList<Library> _libs;
        public LinkedListNode<Library> CurrentLib { get; set; }
        private bool isDraggingSlider;
        private DispatcherTimer timer;
        public File CurrentFile { get; set; }
        private Ofd _ofd;
        private Shell _sh;
        #endregion
        #region base
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                Loaded += MainWindow_Loaded;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception levée dans MainWindow:\n" + ex.Message);
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;
            try
            {
                _ofd = new Ofd();
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
        #endregion base
        private void BindData()
        {
            lb_playlist.ItemsSource = null;
            lb_playlist.ItemsSource = CurrentLib.Value.playlist;
            lb_playlist.DisplayMemberPath = "Name";
            playlistTitle.Content = CurrentLib.Value.Name;
        }

        [TestMethod]
        public void LinkedListTest()
        {
            foreach (var foo in _libs)
            {
                Console.WriteLine(foo.Name);
                foreach (var bar in foo.playlist)
                    Console.WriteLine("\t" + bar.FullName);
            }
        }

        private void LibPrev_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentLib.Previous == null)
                    return;
                var n = CurrentLib.Previous;
                CurrentLib = n;
                BindData();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur clic prev:\n" + ex.Message);
            }
        }

        private void LibNext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentLib.Next == null)
                    return;
                var n = CurrentLib.Next;
                CurrentLib = n;
                BindData();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur clic next:\n" + ex.Message);
            }
        }

        /// <summary>
        /// Handle the playlist's Listbox keydowns
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lb_playlistEntry(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (lb_playlist.SelectedIndex != -1)
                {
                    try
                    {
                        CurrentLib.Value.playlist.RemoveAt(lb_playlist.SelectedIndex);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Exception levée dans lb_playlistEntry:\n" + ex.Message);
                    }
                }
            }
        }

        private void RepPrev_Click(object sender, RoutedEventArgs e)
        {
            CurrentLib.Value.Previous();
        }

        // TODO: Gérer le double clic foireux
        private void ListBoxActions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = lb_playlist.SelectedItem;
            if (selectedItem == null)
                return;
            var selectedIndex = lb_playlist.SelectedIndex;
            if (selectedIndex != -1)
            {
                try
                {
                    CurrentFile = CurrentLib.Value.playlist.ElementAt(selectedIndex);
                    CurrentLib.Value.Index = selectedIndex;
                    if (CurrentFile.IsDirectory)
                        CurrentLib.Value.setLibrary(CurrentFile.FullName);
                    else
                    {
                        me.Source = new Uri(CurrentFile.FullName);
                        me.LoadedBehavior = MediaState.Play;
                        UI_btnPlay.Content = "Pause";
                        l_remaningTime.Content = CurrentFile.Duration;
                        timer.Start();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Une erreur est survenue:\n" + ex.Message);
                }
            }
        }

        private void AddLib_Click(object sender, RoutedEventArgs e)
        {
            var name = Microsoft.VisualBasic.Interaction.InputBox("Nommez votre bibliothèque",
                "Nommez la bibliothèque",
                "Bibliothèque" + _libs.Count);
            if (!name.Equals(string.Empty))
            {
                if (name.Length > 50)
                    name = name.Substring(0, 50);
                var newLib = new Library(name, _sh);
                var newNode = new LinkedListNode<Library>(newLib);
                _libs.AddLast(newNode);
                CurrentLib = newNode;
                BindData();
            }
        }

        private void RenameLib_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string name;

            name = Microsoft.VisualBasic.Interaction.InputBox("Renommez votre bibliothèque",
                                                              "Renommez la bibliothèque",
                                                              CurrentLib.Value.Name);
            if (!name.Equals(string.Empty))
            {
                if (name.Length > 50)
                    name = name.Substring(0, 50);
                CurrentLib.Value.Name = name;
                BindData();
            }
        }

        private void AddFile_Click(object sender, RoutedEventArgs e)
        {
            _ofd.getShowDialog(CurrentLib.Value, me);
        }

        private void MediaPlay_Click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = lb_playlist.SelectedIndex;
            if (selectedIndex != -1)
            {
                try
                {
                    var currentFile = CurrentLib.Value.playlist.ElementAt(selectedIndex);
                    if (!currentFile.IsDirectory)
                    {
                        if (me.LoadedBehavior != MediaState.Play)
                        {
                            var source = new Uri(currentFile.FullName);
                            if (!Equals(source, me.Source))
                                me.Source = source;
                            me.LoadedBehavior = MediaState.Play;
                            UI_btnPlay.Content = "Pause";
                        }
                        else
                        {
                            me.LoadedBehavior = MediaState.Pause;
                            UI_btnPlay.Content = "Play";
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur survenue dans MediaPlay_Click:\n" + ex.Message);
                    throw;
                }
            }
        }

        private void MediaStop_Click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = lb_playlist.SelectedIndex;
            if (selectedIndex != -1)
            {
                try
                {
                    var currentFile = CurrentLib.Value.playlist.ElementAt(selectedIndex);
                    if (!currentFile.IsDirectory)
                    {
                        me.Source = new Uri(currentFile.FullName);
                        me.LoadedBehavior = MediaState.Stop;
                        UI_btnPlay.Content = "Play";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur survenue dans MediaPlay_Click:\n" + ex.Message);
                    throw;
                }
            }
        }

        private void playlistTitle_MouseEnter(object sender, MouseEventArgs e)
        {
            playlistTitle.Foreground = Brushes.DodgerBlue;
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void playlistTitle_MouseLeave(object sender, MouseEventArgs e)
        {
            playlistTitle.Foreground = Brushes.Black;
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            File file = CurrentLib.Value.NextMedia();
            me.Source = new Uri(file.FullName);
            CurrentFile = file;
        }
        private void MediaElement_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentFile != null)
                    l_remaningTime.Content = CurrentFile.Duration;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception levée: " + ex.Message);
            }
        }

        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            me.LoadedBehavior = MediaState.Play;
            currentPlay.Content = CurrentFile.Name;
            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if ((me.Source != null) && (me.NaturalDuration.HasTimeSpan) && !(isDraggingSlider))
            {
                slider.Minimum = 0;
                slider.Maximum = me.NaturalDuration.TimeSpan.TotalSeconds;
                slider.Value = me.Position.TotalSeconds;
            }
        }
        private void slider_DragStarted(object sender, RoutedEventArgs e)
        {
            isDraggingSlider = true;
        }

        private void slider_DragCompleted(object sender, RoutedEventArgs e)
        {
            isDraggingSlider = false;
            me.Position = TimeSpan.FromSeconds(slider.Value);
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            l_currentTime.Content = TimeSpan.FromSeconds(slider.Value).ToString(@"hh\:mm\:ss");
        }
    }
}
