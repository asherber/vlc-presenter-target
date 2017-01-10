using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VlcPresenterMode
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private static readonly List<string> _vlcFiles = new List<string>()
        {
            ".mp4",
            ".avi",
            ".wmv",
            ".mpg"
        };
        private static readonly List<string> _argList = new List<string>()
        {
            "--fullscreen",
            "--no-embedded-video",
            "--no-qt-fs-controller",
            $"--video-x={Screen.PrimaryScreen.WorkingArea.Width + 200}",
            "--video-y=200",       
            "--sub-track=0"
        };
        private static readonly string _vlcArgs = String.Join(" ", _argList);
        private static readonly string _vlcCmd = @"C:\Program Files (x86)\VideoLAN\VLC\vlc.exe";


        private void LaunchVlc(string videoFile)
        {
            var psi = new ProcessStartInfo()
            {
                FileName = _vlcCmd,
                Arguments = $"{_vlcArgs} \"{videoFile}\"",
                UseShellExecute = false
            };
            Process.Start(psi);
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)
                    && GetVlcFiles(e).Count() == 1)
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
                e.Effect = DragDropEffects.None;
        }
        private bool IsVlcFile(string filename)
        {
            return _vlcFiles.Contains(Path.GetExtension(filename).ToLower());
        }

        private IEnumerable<string> GetVlcFiles(DragEventArgs e)
        {
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            return files.Where(f => IsVlcFile(f));
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            LaunchVlc(GetVlcFiles(e).Single());
        }

        private void TestMenuItem_Click(object sender, EventArgs e)
        {
            var filename = Path.GetTempFileName();
            File.WriteAllBytes(filename, Properties.Resources.SampleVideo);
            LaunchVlc(filename);
        }
    }
}
