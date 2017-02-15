using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NAudio;
using NAudio.Wave;
using System.IO;

namespace Mediaspeler
{
    public partial class Form1 : Form
    {
        IWavePlayer player;
        AudioFileReader reader;
        NAudio.Gui.WaveViewer viewer = new NAudio.Gui.WaveViewer();
        public Form1()
        {
            InitializeComponent();
            viewer.Dock = DockStyle.Fill;
            viewer.BackColor = Color.White;
            viewer.SamplesPerPixel = 10;
            panel1.Controls.Add(viewer);
            viewer.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Muziek|*.mp3;*.wav";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string tmpname = Path.GetTempFileName();
                if (ofd.FileName.Split('.').Last() == "mp3")
                {
                    using (Mp3FileReader reader = new Mp3FileReader(ofd.FileName))
                    {
                        WaveFileWriter.CreateWaveFile(tmpname, reader);
                    }
                }
                lblBestand.Text = ofd.FileName;
                viewer.WaveStream = new NAudio.Wave.WaveFileReader(tmpname);                
                btnPlay.Enabled = true;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            player.Stop();
            btnPlay.Enabled = true;
            btnPause.Enabled = false;
            btnStop.Enabled = false;
        }

        void player_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            btnPlay.Enabled = true;
            btnPause.Enabled = false;
            btnStop.Enabled = false;
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if(player == null)
            {
                player = new WaveOut();
                reader = new AudioFileReader(lblBestand.Text);
                player.Init(reader);
                player.PlaybackStopped += player_PlaybackStopped;
                player.Play();
            }
            else if (player.PlaybackState == PlaybackState.Paused)
                player.Play();
            else if (player.PlaybackState == PlaybackState.Stopped)
            {
                player = new WaveOut();
                reader = new AudioFileReader(lblBestand.Text);
                player.Init(reader);
                player.Play();
            }
            btnPlay.Enabled = false;
            btnPause.Enabled = true;
            btnStop.Enabled = true;
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            player.Pause();
            btnPlay.Enabled = true;
            btnPause.Enabled = false;
            btnStop.Enabled = true;
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            viewer.Invalidate();
        }

        private void nudSPP_ValueChanged(object sender, EventArgs e)
        {
            if (viewer != null)
                viewer.SamplesPerPixel = (int)nudSPP.Value;
        }
    }
}
