using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Morpher
{
    public class ControlLinePair
    {
        VirtualControlLine src;
        VirtualControlLine dest;

        Canvas srcCanvas;
        Canvas destCanvas;

        public ControlLinePair(Canvas source, Canvas destination, VirtualControlLine srcLine, VirtualControlLine destLine)
        {
            this.src = srcLine;
            this.dest = destLine;
            srcCanvas = source;
            destCanvas = destination;
        }

        public ControlLinePair(Canvas source, Canvas destination, VirtualControlLine first, bool isSrc)
        {
            this.srcCanvas = source;
            this.destCanvas = destination;
            if (isSrc) { 
                src = first;

                LineEndpointControl start = new LineEndpointControl();
                destCanvas.Children.Add(start);
                Canvas.SetZIndex(start, 5);
                Canvas.SetLeft(start, src.X1);
                Canvas.SetTop(start, src.Y1);
                LineEndpointControl end = new LineEndpointControl();
                destCanvas.Children.Add(end);
                Canvas.SetZIndex(end, 5);
                Canvas.SetLeft(end, src.X2);
                Canvas.SetTop(end, src.Y2);

                dest = new VirtualControlLine(start, end, destCanvas);

                Match(true);

            } else { 
                dest = first;

                LineEndpointControl start = new LineEndpointControl();
                destCanvas.Children.Add(start);
                Canvas.SetZIndex(start, 5);
                Canvas.SetLeft(start, src.X1);
                Canvas.SetTop(start, src.Y1);
                LineEndpointControl end = new LineEndpointControl();
                destCanvas.Children.Add(end);
                Canvas.SetZIndex(end, 5);
                Canvas.SetLeft(end, src.X2);
                Canvas.SetTop(end, src.Y2);

                src = new VirtualControlLine(start, end, srcCanvas);

                Match(false);
            }
            src.SetOwner(this);
            dest.SetOwner(this);
        }

        public void Match(bool isSrcOriginal)
        {
            if (isSrcOriginal)
            {
                Canvas.SetLeft(dest.Start, src.X1);
                Canvas.SetTop(dest.Start, src.Y1);
                Canvas.SetLeft(dest.End, src.X2);
                Canvas.SetTop(dest.End, src.Y2);
                dest.Start.updateCoordsManual(Canvas.GetLeft(src.Start), Canvas.GetTop(src.Start));
                dest.End.updateCoordsManual(Canvas.GetLeft(src.End), Canvas.GetTop(src.End));
                dest.Update();
            }
            else
            {
                Canvas.SetLeft(src.Start, dest.X1);
                Canvas.SetTop(src.Start, dest.Y1);
                Canvas.SetLeft(src.End, dest.X2);
                Canvas.SetTop(src.End, dest.Y2);
                src.Start.updateCoordsManual(Canvas.GetLeft(dest.Start), Canvas.GetTop(dest.Start));
                src.End.updateCoordsManual(Canvas.GetLeft(dest.End), Canvas.GetTop(dest.End));
                src.Update();
            }
        }

        public VirtualControlLine srcLine => src;
        public VirtualControlLine destLine => dest;
    }
}
