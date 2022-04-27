using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Morpher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<ControlLinePair> controlLinePairs;

        bool isDrawing = false;

        Line drawnLine;
        Line otherLine;

        BitmapImage srcBmp;
        BitmapImage destBmp;
        public MainWindow()
        {
            InitializeComponent();
            
            controlLinePairs = new List<ControlLinePair>();

        }

        private void srcCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (editMode.IsChecked == true && !isDrawing)
            {
                isDrawing = true;
                drawnLine = new Line();
                otherLine = new Line();
                drawnLine.Stroke = System.Windows.Media.Brushes.OrangeRed;
                otherLine.Stroke = System.Windows.Media.Brushes.OrangeRed;
                drawnLine.X1 = e.GetPosition(srcCanvas).X;
                drawnLine.Y1 = e.GetPosition(srcCanvas).Y;
                drawnLine.X2 = e.GetPosition(srcCanvas).X;
                drawnLine.Y2 = e.GetPosition(srcCanvas).Y;
                otherLine.X1 = e.GetPosition(srcCanvas).X;
                otherLine.Y1 = e.GetPosition(srcCanvas).Y;
                otherLine.X2 = e.GetPosition(srcCanvas).X;
                otherLine.Y2 = e.GetPosition(srcCanvas).Y;

                srcCanvas.Children.Add(drawnLine);
                destCanvas.Children.Add(otherLine);

            }
        }

        private void destCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Clicked on dest");
        }

        private void srcCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(isDrawing && drawnLine != null)
            {
                isDrawing = false;
                LineEndpointControl start = new LineEndpointControl();
                srcCanvas.Children.Add(start);
                Canvas.SetZIndex(start, 5);
                Canvas.SetLeft(start, drawnLine.X1);
                Canvas.SetTop(start, drawnLine.Y1);
                LineEndpointControl end = new LineEndpointControl();
                srcCanvas.Children.Add(end);
                Canvas.SetZIndex(end, 5);
                Canvas.SetLeft(end, drawnLine.X2);
                Canvas.SetTop(end, drawnLine.Y2);

                VirtualControlLine vclSrc = new VirtualControlLine(start, end, srcCanvas);

                start = new LineEndpointControl();
                destCanvas.Children.Add(start);
                Canvas.SetZIndex(start, 5);
                Canvas.SetLeft(start, drawnLine.X1);
                Canvas.SetTop(start, drawnLine.Y1);
                end = new LineEndpointControl();
                destCanvas.Children.Add(end);
                Canvas.SetZIndex(end, 5);
                Canvas.SetLeft(end, drawnLine.X2);
                Canvas.SetTop(end, drawnLine.Y2);

                VirtualControlLine vclDest = new VirtualControlLine(start, end, destCanvas);

                ControlLinePair clp = new ControlLinePair(srcCanvas, destCanvas, vclSrc, vclDest);

                controlLinePairs.Add(clp);

                srcCanvas.Children.Remove(drawnLine);
                destCanvas.Children.Remove(otherLine);
                drawnLine = null;
                otherLine = null;
            }
        }

        private void srcCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if(isDrawing && drawnLine != null && otherLine != null)
            {
                drawnLine.X2 = e.GetPosition(srcCanvas).X;
                drawnLine.Y2 = e.GetPosition(srcCanvas).Y;
                otherLine.X2 = e.GetPosition(srcCanvas).X;
                otherLine.Y2 = e.GetPosition(srcCanvas).Y;
            }
        }

        private void srcUploadBtn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Common Images|*.bmp; *.png; *.jpeg; *.jpg";
            Nullable<Boolean> didSelect = dlg.ShowDialog();
            if (!didSelect.Value)
            {
                return;
            }
            BitmapImage inFile = new BitmapImage();
            inFile.BeginInit();
            inFile.UriSource = new Uri(dlg.FileName);
            inFile.EndInit();
            srcBmp = inFile;
            srcImage.Source = inFile;
            srcImage.Width = srcCanvas.ActualWidth;
            srcImage.Height = srcCanvas.ActualHeight;
        }

        private void destUploadBtn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Common Images|*.bmp; *.png; *.jpeg; *.jpg";
            Nullable<Boolean> didSelect = dlg.ShowDialog();
            if (!didSelect.Value)
            {
                return;
            }
            BitmapImage inFile = new BitmapImage();
            inFile.BeginInit();
            inFile.UriSource = new Uri(dlg.FileName);
            inFile.EndInit();
            destBmp = inFile;
            destImage.Source = inFile;
            destImage.Width = destCanvas.ActualWidth;
            destImage.Height = destCanvas.ActualHeight;

        }

        private Byte[] imgAvg(Byte[] srcPx, Byte[] destPx, int xMax, int yMax, int bytesPerPixel, double weight = 0.5)
        {
            if(destPx == null || srcPx == null)
            {
                return null;
            }
            Byte[] pixels = new Byte[xMax * yMax * bytesPerPixel];
            for (int y = 0; y < (yMax-1) * bytesPerPixel; y++)
            {
                for(int x = 0; x < (xMax-1) * bytesPerPixel; x++)
                {
                    pixels[y * (xMax - 1) + x] = (byte)((srcPx[y * (xMax - 1) + x] * weight) 
                        + (byte)(destPx[y * (xMax - 1) + x] * (1 - weight)));
                }
            }
            return pixels;
        }

        private Byte[] imgWarp(Byte[] srcPx, Byte[] destPx, int xMax, int yMax, int bytesPerPixel,
            double p, double a, double b, double weight = 1, bool reverse = false)
        {
            if(controlLinePairs.Count == 0)
            {
                return srcPx;
            }

            Byte[] pixels = new Byte[xMax * yMax * bytesPerPixel];

            for (int y = 0; y < yMax; y++)
            {
                for (int x = 0; x < xMax; x++)
                {
                    Vector res = new Vector(x, y);
                    Vector dSum = new Vector(0, 0);
                    double wtsum = 0.0;

                    foreach(ControlLinePair clp in controlLinePairs)
                    {

                        Vector c = new Vector(x, y);
                        VirtualControlLine srcLine = (reverse ? clp.destLine : clp.srcLine);
                        VirtualControlLine destLine = (reverse ? clp.srcLine: clp.destLine);


                        Vector pq = destLine.calcLineAsVector();

                        Vector px = c - destLine.Start.Vector;

                        double d = MorpherMath.ProjectionMagnitude(px, MorpherMath.CreateNormal(pq));
                        double lengthFrac = MorpherMath.ProjectionMagnitude(px, pq)/MorpherMath.Magnitude(pq);

                        Vector ppqq = srcLine.calcLineAsVector();

                        Vector final = srcLine.Start.Vector + lengthFrac * ppqq + d * (MorpherMath.CreateNormal(ppqq) 
                            / MorpherMath.Magnitude(MorpherMath.CreateNormal(ppqq)));
                        final -= res;

                        //[lenght(to pow of P) / (a + distance)](to pow of b);
                        double wt = Math.Pow(MorpherMath.Magnitude(clp.srcLine.calcLineAsVector()), p) 
                            / Math.Pow((a + Math.Abs(d)), b);
                        dSum += final * wt;
                        wtsum += wt;

                    }
                    dSum /= wtsum;

                    res += (dSum * weight);

                    int resX = (int)Math.Round(res.X);
                    int resY = (int)Math.Round(res.Y);

                    //This could be done in less lines with ceil I guess
                    if (resX < 0)
                    {
                        resX = 0;
                    }
                    else if (resX > xMax - 1)
                    {
                        resX = xMax - 1;
                    }

                    if (resY < 0)
                    {
                        resY = 0;
                    }
                    else if (resY > yMax - 1)
                    {
                        resY = yMax - 1;
                    }


                    for (int i = 0; i < bytesPerPixel; i++)
                    {
                        pixels[y * bytesPerPixel * (xMax) + x * bytesPerPixel + i]
                            += (byte)((srcPx[resY * bytesPerPixel * (xMax) + resX * bytesPerPixel + i]));
                    }

                }
            }

            return pixels;

        }

        private void avgBtn_Click(object sender, RoutedEventArgs e)
        {
            int frames = int.Parse(framesTxtBox.Text);

            int threads = int.Parse(threadsTxtBox.Text);

            if (frames < 1 || threads < 1)
            {
                return;
            }

            if (threads > frames)
            {
                threads = frames;
            }

            WriteableBitmap[] images = new WriteableBitmap[frames];

            Thread[] tasks = new Thread[threads];

            int yMax = Math.Min(srcBmp.PixelHeight, destBmp.PixelHeight);
            int xMax = Math.Min(srcBmp.PixelWidth, destBmp.PixelWidth);

            WriteableBitmap src = new WriteableBitmap(srcBmp);
            WriteableBitmap dest = new WriteableBitmap(destBmp);

            Byte[] srcPx = new Byte[src.PixelWidth * src.PixelHeight * src.Format.BitsPerPixel / 8];
            src.CopyPixels(srcPx, src.BackBufferStride, 0);
            Byte[] destPx = new Byte[dest.PixelWidth * dest.PixelHeight * dest.Format.BitsPerPixel / 8];
            dest.CopyPixels(destPx, dest.BackBufferStride, 0);

            src.Freeze();
            dest.Freeze();

            foreach (ControlLinePair pair in controlLinePairs)
            {
                pair.destLine.Start.triggerCoordinateUpdate();
                pair.destLine.End.triggerCoordinateUpdate();
                pair.srcLine.Start.triggerCoordinateUpdate();
                pair.srcLine.End.triggerCoordinateUpdate();
            }

            int framesPerThread;
            int remnantFrames;

            if (threads > 1) { 
                framesPerThread = frames / (threads - 1);
                remnantFrames = frames - framesPerThread * (threads - 1);
                if( remnantFrames == 0 && (frames/threads) * threads == frames)
                {
                    framesPerThread = frames / threads;
                    remnantFrames = frames / threads;
                }
            } else
            {
                framesPerThread = 0;
                remnantFrames = frames;
            }

            Console.WriteLine("FPT: " + framesPerThread + " R: " + remnantFrames);

            double p = p_slider.Value;
            double a = a_slider.Value;
            double b = b_slider.Value;

            int currentFrame = 0;

            for(int t = 0; t < threads; t++)
            {
                tasks[t] = new Thread((o) =>
                {
                    for (int i = (int)o; i < ((int)o == 0 ? remnantFrames : (int)o + framesPerThread); i++)
                    {
                        Console.WriteLine(i.ToString());
                        WriteableBitmap result = new WriteableBitmap(Math.Min(src.PixelWidth, dest.PixelWidth),
                        Math.Min(src.PixelHeight, dest.PixelHeight), 0, 0, src.Format, src.Palette);

                        Byte[] warpFwdResut = imgWarp(srcPx, destPx, xMax, yMax,
                            result.Format.BitsPerPixel / 8, p, a,
                            b, 1f / (frames -1) * i, false);

                        Byte[] warpBkwdResut = imgWarp(destPx, srcPx, xMax, yMax,
                            result.Format.BitsPerPixel / 8, p, a,
                            b, 1 - 1f / (frames-1) * i, true);

                        Byte[] resultPx = imgAvg(warpFwdResut, warpBkwdResut, xMax, yMax,
                            result.Format.BitsPerPixel / 8, 1 - 1f / (frames - 1) * i);

                        result.WritePixels(new Int32Rect(0, 0, xMax, yMax),
                                resultPx, Math.Min(src.BackBufferStride, dest.BackBufferStride), 0, 0);

                        result.Freeze();

                        images[i] = result;
                    }
                });
                tasks[t].Start(t < 2 ? remnantFrames * t : (((t - 1) * framesPerThread) + remnantFrames));
                currentFrame += framesPerThread;
            }

            foreach(Thread t in tasks)
            {
                t.Join();
            }
            
            PlayerWindow pw = new PlayerWindow(images);
            pw.Show();
        }

        private void EnsureTextboxNumber(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("0-9");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void benchBtn_Click(object sender, RoutedEventArgs e)
        {
            int threads = int.Parse(threadsTxtBox.Text);

            threadsTxtBox.Text = "1";

            DateTime startUnthread = DateTime.Now;
            avgBtn_Click(null, null);
            DateTime endUnthread = DateTime.Now;

            threadsTxtBox.Text = threads.ToString();

            DateTime startThread = DateTime.Now;
            avgBtn_Click(null, null);
            DateTime endThread = DateTime.Now;

            double msUnthread = (endUnthread - startUnthread).TotalMilliseconds;
            double msThread = (endThread - startThread).TotalMilliseconds;

            MessageBox.Show("1 thread took " + msUnthread + "ms to complete this image.\n" 
                + threads + " threads took " + msThread + "ms. \n" + "It took 1 thread " 
                + msUnthread/msThread + " times as long.", "Benchmark Complete");

        }
    }
}
