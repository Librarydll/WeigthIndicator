using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace WeigthIndicator.Controls
{
    public class Modal : ContentControl
    {
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(Modal),
                new PropertyMetadata(false));

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }


        public HorizontalAlignment HorizontalModalAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalModalAlignmentProperty); }
            set { SetValue(HorizontalModalAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorizontalModalAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalModalAlignmentProperty =
            DependencyProperty.Register("HorizontalModalAlignment", typeof(HorizontalAlignment), typeof(Modal), new PropertyMetadata(HorizontalAlignment.Center));



        public VerticalAlignment VerticalModalAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalModalAlignmentProperty); }
            set { SetValue(VerticalModalAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalModalAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalModalAlignmentProperty =
            DependencyProperty.Register("VerticalModalAlignment", typeof(VerticalAlignment), typeof(Modal), new PropertyMetadata(VerticalAlignment.Center));


        public double ControlHeigth
        {
            get { return (double)GetValue(ControlHeigthProperty); }
            set { SetValue(ControlHeigthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ControlHeigth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ControlHeigthProperty =
            DependencyProperty.Register("ControlHeigth", typeof(double), typeof(Modal), new PropertyMetadata(0.0));



        public double ControlWidth
        {
            get { return (double)GetValue(ControlWidthProperty); }
            set { SetValue(ControlWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ControlWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ControlWidthProperty =
            DependencyProperty.Register("ControlWidth", typeof(double), typeof(Modal), new PropertyMetadata(0.0));




        static Modal()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Modal), new FrameworkPropertyMetadata(typeof(Modal)));
            BackgroundProperty.OverrideMetadata(typeof(Modal), new FrameworkPropertyMetadata(CreateDefaultBackground()));
        }

        private static object CreateDefaultBackground()
        {
            return new SolidColorBrush(Colors.Black)
            {
                Opacity = 0.3
            };
        }
    }
}
