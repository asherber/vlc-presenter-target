/**
 * Copyright 2017 Aaron Sherber
 * 
 * This file is part of VlcPresenterTarget.
 *
 * VlcPresenterTarget is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * VlcPresenterTarget is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with VlcPresenterTarget. If not, see <http://www.gnu.org/licenses/>.
 */

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

namespace VlcPresenterTarget
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        // See https://wiki.videolan.org/VLC_Features_Formats
        private static readonly List<string> _vlcExtensions = new List<string>()
        {
            ".3g2",
            ".3gp",
            ".asf",
            ".avi",
            ".f4v",
            ".flv",
            ".m1v",
            ".m2v",
            ".m4v",
            ".mkv",
            ".mls",
            ".mov",
            ".mp4",
            ".mpeg",
            ".mpg",
            ".ogg",
            ".ogv",
            ".wmv",
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
                    && GetVlcFilesFromDragEvent(e).Count() == 1)
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }
        private bool IsVlcFile(string filename)
        {
            var fileExtension = Path.GetExtension(filename).ToLower();
            return _vlcExtensions.Contains(fileExtension);
        }

        private IEnumerable<string> GetVlcFilesFromDragEvent(DragEventArgs e)
        {
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            return files.Where(f => IsVlcFile(f));
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            LaunchVlc(GetVlcFilesFromDragEvent(e).Single());
        }

        private void TestMenuItem_Click(object sender, EventArgs e)
        {
            var tempFile = Path.GetTempFileName();
            File.WriteAllBytes(tempFile, Properties.Resources.SampleVideo);
            LaunchVlc(tempFile);
        }
    }
}
