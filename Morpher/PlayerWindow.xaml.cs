using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Morpher
{
    /// <summary>
    /// Interaction logic for PlayerWindow.xaml
    /// </summary>
    public partial class PlayerWindow : Window
    {
        int currentFrame = 0;
        WriteableBitmap[] images;
        public PlayerWindow(WriteableBitmap[] pictures)
        {
            images = pictures;
            InitializeComponent();
            updateDisplay(0);
        }

        private void updateDisplay(int frame)
        {
            if (frame < 0 || frame >= images.Length)
            {
                return;
            }
            currentFrame = frame;
            img.Source = images[frame];
        }

        private void fwdStepBtn_Click(object sender, RoutedEventArgs e)
        {
            updateDisplay(currentFrame+1);
        }

        private async void playBtn_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < images.Length; i++)
            {
                updateDisplay(i);
                await Task.Delay(42);
            }
        }

        private void bkwdStepBtn_Click(object sender, RoutedEventArgs e)
        {
            updateDisplay(currentFrame-1);
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.DefaultExt = ".gif";
            fileDialog.Filter = "Animated Image|*.gif";

            if (fileDialog.ShowDialog() != true)
            {
                return;
            }

            System.Windows.Media.Imaging.GifBitmapEncoder gEnc = new GifBitmapEncoder();
            foreach (WriteableBitmap bmpImage in images)
            {
                gEnc.Frames.Add(BitmapFrame.Create(bmpImage));
            }

            using (System.IO.FileStream fs = new System.IO.FileStream(fileDialog.FileName, System.IO.FileMode.Create))
            {
                gEnc.Save(fs);
            }
        }

        private void toStartBtn_Click(object sender, RoutedEventArgs e)
        {
            updateDisplay(0);
        }

        private void toEndBtn_Click(object sender, RoutedEventArgs e)
        {
            updateDisplay(images.Length - 1);
        }
    }
}
