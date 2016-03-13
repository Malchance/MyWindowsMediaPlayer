using System;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Forms;

namespace MyWindowsMediaPlayer
{
    class Ofd
    {
        private OpenFileDialog ofd;
        public string videoExt;
        public string audioExt;
        public string imageExt;
        public static string mediaExt { get; private set; }

        public Ofd()
        {
            videoExt = "*.dat; *.wmv; *.3g2; *.3gp; *.3gp2; *.3gpp; *.amv; *.asf;  *.avi; *.bin; *.cue; *.divx; *.dv; *.flv; *.gxf; *.iso; *.m1v; *.m2v; *.m2t; *.m2ts; *.m4v; *.mkv; *.mov; *.mp2; *.mp2v; *.mp4; *.mp4v; *.mpa; *.mpe; *.mpeg; *.mpeg1; *.mpeg2; *.mpeg4; *.mpg; *.mpv2; *.mts; *.nsv; *.nuv; *.ogg; *.ogm; *.ogv; *.ogx; *.ps; *.rec; *.rm; *.rmvb; *.tod; *.ts; *.tts; *.vob; *.vro; *.webm";
            imageExt = "*.jpeg; *.jpg; *.png; *.bmp; *.gif";
            audioExt = "*.mp3; *.m4a; *.vma; *.riff; *.wav; *.bwf; *.ogg; *.aiff; *.caf; *.flac; *.alac; *.ac3; *.aac; *.flac; *.FLAC";
            mediaExt = videoExt + "; " + audioExt + "; " + imageExt;

            ofd = new OpenFileDialog();
            ofd.InitialDirectory = @"c:\";
            ofd.Filter = "Fichiers multimédias | " + mediaExt + "| Fichiers vidéos | " + videoExt + "| Fichiers audio | " + audioExt + "| Fichiers images | " + imageExt;
            ofd.FilterIndex = 1;
            ofd.RestoreDirectory = true;
            ofd.Multiselect = true;
        }

        public void getShowDialog(Library lib, MediaElement me)
        {
            try
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    foreach (var item in ofd.FileNames)
                    {
                        FileInfo fi = new FileInfo(item);
                        lib.playlist.Add(new File(lib.sh, fi));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception levée dans getShowDialog:\n" + ex.Message);
            }
        }
    }
}
