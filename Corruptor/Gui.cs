using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Corruptor
{
    public partial class Gui : Form
    {
        private Bitmap bmp;
        private Config config;
        public Gui()
        {
            InitializeComponent();
            propertyGrid1.SelectedObject = config = new Config();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                pictureBox1.Image = bmp = new Bitmap(openFileDialog1.FileName);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Could not load file: " + exception.Message);
            }
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                pictureBox1.Image.Save(saveFileDialog1.FileName);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Could not save file: " + exception.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = new Corruptor(config).CorruptImage(bmp);
        }

        private void saveGifToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog2.ShowDialog();
        }

        private void saveFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            GifWriter gifWriter = new GifWriter(saveFileDialog2.FileName, Repeat:0);
            Random r = new Random();
            Corruptor corruptor = new Corruptor(config);
            int count = r.Next(20, 100);
            for (int i = 0; i < count; i++)
            {
                gifWriter.WriteFrame(corruptor.CorruptImage(bmp), r.Next(10, 50));
            }
            gifWriter.Dispose();
        }

        private void saveGifIncrementalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog3.ShowDialog();
        }

        private void saveFileDialog3_FileOk(object sender, CancelEventArgs e)
        {
            Bitmap lastImage = bmp;
            GifWriter gifWriter = new GifWriter(saveFileDialog3.FileName, Repeat:0);
            Random r = new Random();
            Corruptor corruptor = new Corruptor(config);
            int count = r.Next(20, 100);
            for (int i = 0; i < count; i++)
            {
                gifWriter.WriteFrame(lastImage = corruptor.CorruptImage(lastImage), r.Next(10, 50));
            }
            gifWriter.Dispose();
        }
    }
}