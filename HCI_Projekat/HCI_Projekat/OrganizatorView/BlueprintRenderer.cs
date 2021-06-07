using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HCI_Projekat.OrganizatorView
{
    class BlueprintRenderer : FrameworkElement
    {
        public BlueprintRenderer()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                //this.BackgroundImage = new BitmapImage(new Uri("../../Source/map.jpg", UriKind.Relative));
            }
        }

        public ImageSource BackgroundImage
        {
            get
            {
                return base.GetValue(BackgroundImageProperty) as ImageSource;
            }
            set
            {
                base.SetValue(BackgroundImageProperty, value);
                InvalidateVisual();
            }
        }

        public static readonly DependencyProperty BackgroundImageProperty = DependencyProperty.Register("BackgroundImage", typeof(ImageSource), typeof(BlueprintRenderer), new PropertyMetadata(Changed));

        //Reagujemo kada se property promeni preko binding-a
        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as BlueprintRenderer;
            if (c.BackgroundImage == null)
            {
                c.BackgroundImage = null;
            }
            c.InvalidateVisual();
            GC.Collect();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawImage(BackgroundImage, new Rect(0, 0, ActualWidth, ActualHeight));
        }
    }
}
