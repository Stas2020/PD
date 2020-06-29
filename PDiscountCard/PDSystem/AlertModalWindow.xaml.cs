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

namespace PDiscountCard.PDSystem
{
    /// <summary>
    /// Interaction logic for AlertModalWindow.xaml
    /// </summary>
    public partial class AlertModalWindow : Window
    {
        public AlertModalWindow()
        {
            InitializeComponent();
        }
        public void SetContent(System.Windows.Controls.UserControl ctrl)
        {
            MainGrid.Children.Add(ctrl);
        }


    }
}
