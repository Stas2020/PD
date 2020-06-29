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
using System.ComponentModel;

namespace FCCIntegration.WPFControls
{
    /// <summary>
    /// Interaction logic for ctrlShowMoney2.xaml
    /// </summary>
    public partial class ctrlShowMoney2 : UserControl, INotifyPropertyChanged 
    
    {



        public ctrlShowMoney2()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        } 
        public string MoneyName
        {
            get { return (string)GetValue(MoneyNameProperty); }
            set { SetValue(MoneyNameProperty, value);
          
                
            }
        }

        private static void OnMoneyNamePropertyChanged(DependencyObject dependencyObject,
               DependencyPropertyChangedEventArgs e)
        {
            ctrlShowMoney2 myUserControl = dependencyObject as ctrlShowMoney2;
            myUserControl.OnPropertyChanged("MoneyName");
            myUserControl.OnCaptionPropertyChanged(e);
        }
        private void OnCaptionPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            lblName.Content = MoneyName;
        }

        // Using a DependencyProperty as the backing store for CurrentNumber.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MoneyNameProperty =
            DependencyProperty.Register("MoneyName", typeof(string),
            typeof(ctrlShowMoney2), new UIPropertyMetadata("", OnMoneyNamePropertyChanged));

        public void SetMoney(int Money)
        {

            int Kop = Money % 100;
            int Rub = (Money - Kop) / 100;
            lblKop.Content = Kop.ToString("00");
            lblRub.Content = Rub.ToString();

        
        }

    }
}
