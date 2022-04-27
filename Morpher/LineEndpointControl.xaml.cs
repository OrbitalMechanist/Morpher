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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Morpher
{
    /// <summary>
    /// Interaction logic for LineEndpointControl.xaml
    /// </summary>
    public partial class LineEndpointControl : UserControl
    {

        VirtualControlLine owner;

        bool isDragged = true;

        double x;
        double y;

        public LineEndpointControl()
        {
            InitializeComponent();
        }

        public void SetOwner(VirtualControlLine o)
        {
            owner = o;
        }

        public LineEndpointControl(VirtualControlLine creator)
        {
            owner = creator;
            InitializeComponent();
        }

        public void triggerCoordinateUpdate()
        {
            x = Canvas.GetLeft(this);
            y = Canvas.GetTop(this);
        }

        public void updateCoordsManual(double newX, double newY)
        {
            x = newX;
            y = newY;
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                Canvas.SetLeft(this, e.GetPosition((Canvas)this.Parent).X);
                Canvas.SetTop(this, e.GetPosition((Canvas)this.Parent).Y);
                x = e.GetPosition((Canvas)this.Parent).X;
                y = e.GetPosition((Canvas)this.Parent).Y;
                owner.Update();
            }
            
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragged)
            {
                owner.Owner.Match(true);
                isDragged = false;
                Console.WriteLine("match");
            }
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragged = false;
        }

        public Vector Vector => new Vector(x, y);
    }
}
