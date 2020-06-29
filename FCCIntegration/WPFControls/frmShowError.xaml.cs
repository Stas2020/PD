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

namespace FCCIntegration.WPFControls
{
    /// <summary>
    /// Interaction logic for frmShowError.xaml
    /// </summary>
    public partial class frmShowError : Window
    {
        public frmShowError(string Message, string Url)
        {
            InitializeComponent();
            //string M =
            txtMsg.Text = String.Format("Устройство в состоянии ошибки. Код ошибки: {0}. ", Message)+Environment.NewLine;
            txtMsg.Text += String.Format("Url: {0}", Url);
            
            //ImgError.Source = new BitmapImage(new Uri(Url));
            //ImgError.Source = new Uri(Url);
            ImgError.Navigate(Url);
            /*
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(Url);
            image.EndInit();
            WpfAnimatedGif.ImageBehavior.SetAnimatedSource(ImgError, image);
             * */
           
        }

        private void btnErrorExit_Click(object sender, RoutedEventArgs e)
        {
            ImgError.Source = null;
            
            this.Close();
        }
    }
}
