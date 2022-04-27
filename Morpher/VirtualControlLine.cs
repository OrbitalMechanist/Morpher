using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Numerics;

namespace Morpher
{
    public class VirtualControlLine
    {

        LineEndpointControl start;
        LineEndpointControl end;
        Canvas target;

        Line drawing;

        ControlLinePair owner;

        float b = 2;
        float a = 0.2f;

        public VirtualControlLine(LineEndpointControl beg, LineEndpointControl fin, Canvas space)
        {
            start = beg;
            end = fin;
            target = space;

            start.SetOwner(this);
            end.SetOwner(this);

            drawing = new Line();
            drawing.StrokeThickness = 3;
            drawing.Stroke = new SolidColorBrush(Colors.CadetBlue);
            
            target.Children.Add(drawing);

            Update();
        }
        public void Update()
        {
            drawing.X1 = Canvas.GetLeft(start);
            drawing.Y1 = Canvas.GetTop(start);
            drawing.X2 = Canvas.GetLeft(end);
            drawing.Y2 = Canvas.GetTop(end);
        }

        public double X1 => Canvas.GetLeft(start);
        public double Y1 => Canvas.GetTop(start);
        public double Y2 => Canvas.GetTop(end);
        public double X2 => Canvas.GetLeft(end);

        public ControlLinePair Owner => owner;

        public void SetOwner(ControlLinePair clp)
        {
            owner = clp;
        }

        public LineEndpointControl Start => start;
        public LineEndpointControl End => end;
        
        public Vector calcLineAsVector()
        {
            return end.Vector - start.Vector;
        }
    }
}
