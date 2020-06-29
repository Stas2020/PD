using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;

namespace PDiscountCard.PrintOrder
{
    /// <summary>
    /// Interaction logic for TextBlockFilled.xaml
    /// </summary>
    public partial class TextBlockFilled : Control
    {
        public TextBlockFilled()
        {
            InitializeComponent();
        }

         public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text",
        typeof(string),
        typeof(TextBlockFilled),
         new FrameworkPropertyMetadata(
                        string.Empty, 
                        FrameworkPropertyMetadataOptions.AffectsMeasure |
                        FrameworkPropertyMetadataOptions.AffectsRender)
        )        ;

        public string Text
        {
            get { return GetValue(TextProperty) as string; }
            set { SetValue(TextProperty, value); }
        }
        

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            FormattedText formattedText = formattedText = new FormattedText(
            Text,
            CultureInfo.GetCultureInfo("en-us"),
            FlowDirection.LeftToRight,
            new Typeface("Verdana"),
            ActualHeight,
            Brushes.Black);
            formattedText.TextAlignment = TextAlignment.Left;
            formattedText.MaxTextWidth = ActualWidth - Padding.Left - Padding.Right;
            formattedText.Trimming = TextTrimming.None;
            double step = ActualHeight / 20;
            double fontSize = 0;
            for (double i = ActualHeight - step; i >= step; i -= step)
            {
                if (formattedText.Height <= ActualHeight - Padding.Top - Padding.Bottom) break;
                formattedText.SetFontSize(i);
                fontSize = i;
            }
            if (fontSize > 14) formattedText.SetFontSize(14);
            formattedText.MaxTextHeight = ActualHeight;
            drawingContext.DrawText(formattedText, new Point(Padding.Left, Padding.Top));
        }
    }

    
}
