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

namespace FCCIntegration
{
    /// <summary>
    /// Interaction logic for ctrlShowOrder.xaml
    /// </summary>
    public partial class ctrlShowOrder : UserControl
    {
        public ctrlShowOrder()
        {
            InitializeComponent();
        }
        public void SetDishez(FCCCheck Chk)
        {
            CheckPanel.Children.Clear();
            foreach (FCCDish D in Chk.Dishes)
            {
                DockPanel Dp = new DockPanel();
                Dp.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                Dp.Margin = new Thickness(0, -5, 0, 0);

                Label lbName = new Label();
               
                    lbName.Content = D.Name.Replace(Environment.NewLine, " ").Replace("  ", " ");
               
               
                lbName.FontSize = 20;

                Label lbPrice = new Label();
                lbPrice.Content = ((decimal)D.Price/(decimal)100).ToString("0.00");
                lbPrice.FontSize = 20;
                lbPrice.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;

                DockPanel.SetDock(lbName, Dock.Left);
                DockPanel.SetDock(lbPrice, Dock.Right);

                Dp.Children.Add(lbName);
                Dp.Children.Add(lbPrice);

                CheckPanel.Children.Add(Dp);

                
            }
            SumLabel.Content = ((decimal)Chk.Ammount/(decimal)100).ToString("0.00р");
        }
        public void ClearDishez()
        {
            CheckPanel.Children.Clear();
            SumLabel.Content = (0).ToString("0.00р");
        }
        
    }
}
