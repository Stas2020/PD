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
    /// Interaction logic for ctrlBaraban.xaml
    /// </summary>
    public partial class ctrlBaraban : UserControl, INotifyPropertyChanged 
    {
        public ctrlBaraban()
        {
            InitializeComponent();
           
        }


        internal FCCSrv2.DenominationType DenomType { set; get; }

        private bool btnsVisible = false;
        public bool BtnsVisible
        {
            set {
                btnsVisible = value;
                if (value)
                {
                    pnlBtns.Visibility = System.Windows.Visibility.Visible;
                    
                    
                }
                else
                {
                    pnlBtns.Visibility = System.Windows.Visibility.Hidden;
                    
                    
                
                }
            }
            get
            {
                return btnsVisible;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string BarabanName
        {
            get { return (string)GetValue(BarabanNameProperty); }
            set
            {
                SetValue(BarabanNameProperty, value);
            }
        }
        

        private static void OnBarabanNamePropertyChanged(DependencyObject dependencyObject,
               DependencyPropertyChangedEventArgs e)
        {
            ctrlBaraban myUserControl = dependencyObject as ctrlBaraban;
            myUserControl.OnPropertyChanged("BarabanName");
            myUserControl.lblName.Content = myUserControl.BarabanName;
        }
        

        // Using a DependencyProperty as the backing store for CurrentNumber.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BarabanNameProperty =
            DependencyProperty.Register("BarabanName", typeof(string),
            typeof(ctrlBaraban), new UIPropertyMetadata("", OnBarabanNamePropertyChanged));


        public int DenomValue
        {
            get { return (int)GetValue(DenomValueProperty); }
            set
            {
                SetValue(DenomValueProperty, value);
            }
        }
        private static void OnDenomValuePropertyChanged(DependencyObject dependencyObject,
               DependencyPropertyChangedEventArgs e)
        {
            ctrlBaraban myUserControl = dependencyObject as ctrlBaraban;
            myUserControl.OnPropertyChanged("DenomValue");

            if (myUserControl.DenomValue == 0)
            {
                myUserControl.lblDenomValue.Content = "";
                //myUserControl.txtMoneyValue.Text = "";
            }
            else
            {
                string Post = myUserControl.DenomValue >= 100 ? "р." : "коп.";
                int del = myUserControl.DenomValue >= 100 ? 100 : 1;
                myUserControl.lblDenomValue.Content = (myUserControl.DenomValue / del).ToString() + " " + Post;
                //myUserControl.txtMoneyValue.Text = (myUserControl.DenomValue / del).ToString() + " " + Post;

                
            }
            }





        // Using a DependencyProperty as the backing store for CurrentNumber.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DenomValueProperty =
            DependencyProperty.Register("DenomValue", typeof(int),
            typeof(ctrlBaraban), new UIPropertyMetadata(0, OnDenomValuePropertyChanged));


        public int MoneyValue
        {
            get { return (int)GetValue(MoneyValueProperty); }
            set
            {
                SetValue(MoneyValueProperty, value);

                if (value == -1)
                {
                    lblMoneyColor.Content = "Неизвестно";
                }
                else
                {
                    lblMoneyColor.Height = Math.Max(20, (int)(double)(lblMoneyValue.ActualHeight * PercentOfFull));
                    string Bank = "";
                    int C = MoneyCount % 10;
                    if (moneyType == 0)
                    {
                        if (C == 1)
                        {
                            Bank = " банкнота";
                        }
                        else if ((C > 2) && (C < 5))
                        {
                            Bank = " банкноты";
                        }
                        else
                        {
                            Bank = " банкнот";
                        }
                    }
                    else
                        if (moneyType == 1)
                        {
                            if (C == 1)
                            {
                                Bank = " монета";
                            }
                            else if ((C > 2) && (C < 5))
                            {
                                Bank = " монеты";
                            }
                            else
                            {
                                Bank = " монет";
                            }
                        }

                    lblMoneyColor.Content = MoneyCount + Bank;
                    
                }
            }
        }
        private static void OnMoneyValuePropertyChanged(DependencyObject dependencyObject,
               DependencyPropertyChangedEventArgs e)
        {
            ctrlBaraban myUserControl = dependencyObject as ctrlBaraban;
            myUserControl.OnPropertyChanged("MoneyValue");
            //myUserControl.lblMoneyValue.Content = myUserControl.MoneyValue/100+ " р.";
            myUserControl.txtMoneyValue.Text = myUserControl.MoneyValue / 100 + " р.";
            
          //  myUserControl.lblMoneyColor.Height = Math.Max(30, (int)(double)(myUserControl.lblMoneyValue.ActualHeight * myUserControl.PercentOfFull));
         //   myUserControl.lblMoneyColor.Content = myUserControl.MoneyCount + " банкнот";
        }


        // Using a DependencyProperty as the backing store for CurrentNumber.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MoneyValueProperty =
            DependencyProperty.Register("MoneyValue", typeof(int),
            typeof(ctrlBaraban), new UIPropertyMetadata(-1, OnMoneyValuePropertyChanged));

        public int maxMoneyCount = 110;
        public int MaxMoneyCount { 
            set{
                maxMoneyCount = value;
            }
            get
            {
                return maxMoneyCount;
            }
        }

        public double PercentOfFull
        {
            get
            {
                if (MaxMoneyCount == 0)
                { 
                    return 0;
                }
                else
                {
                    return (double)MoneyCount / (double)MaxMoneyCount;
                }
            }
        
        }
        public bool OneDenom = true;
        private int moneyCount = 0;
        public int MoneyCount
        {
            get {
                if (OneDenom)
                {
                    if (DenomValue == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return MoneyValue / DenomValue;

                    }
                }
                else
                {
                    return moneyCount;
                }
                
            }
            set {
                moneyCount = value;
            
            }
        }

        private int moneyType = 0;
        public int MoneyType
        {
            set {
                moneyType = value;
            }
            get
            {
                return moneyType;
            }
        
        }

        //public int MoneyCount
        //{
        //    get { return (int)GetValue(MoneyCountProperty); }
        //    set
        //    {
        //        SetValue(MoneyCountProperty, value);
        //    }
        //}
        //private static void OnMoneyCountPropertyChanged(DependencyObject dependencyObject,
        //       DependencyPropertyChangedEventArgs e)
        //{
        //    ctrlBaraban myUserControl = dependencyObject as ctrlBaraban;
        //    myUserControl.OnPropertyChanged("MoneyCount");
        //}
        //public static readonly DependencyProperty MoneyCountProperty =
        //    DependencyProperty.Register("MoneyCount", typeof(int),
        //    typeof(ctrlBaraban), new UIPropertyMetadata(0, OnMoneyCountPropertyChanged));

        public int MoneyState
        {
            get { return (int)GetValue(MoneyStateProperty); }
            set
            {
                SetValue(MoneyStateProperty, value);
            }
        }
        private static void OnMoneyStatePropertyChanged(DependencyObject dependencyObject,
               DependencyPropertyChangedEventArgs e)
        {
            ctrlBaraban myUserControl = dependencyObject as ctrlBaraban;
            myUserControl.OnPropertyChanged("MoneyState");

            switch (myUserControl.MoneyState)
            {
                case 0:
                    myUserControl.lblMoneyColor.Background = new SolidColorBrush(Colors.Red);
                    break;
                case 1:
                    myUserControl.lblMoneyColor.Background = new SolidColorBrush(Colors.LightPink);
                    break;
                case 2:
                    myUserControl.lblMoneyColor.Background = new SolidColorBrush(Colors.LightGray);
                    break;
                case 3:
                    myUserControl.lblMoneyColor.Background = new SolidColorBrush(Colors.LightBlue);
                    break;
                case 4:
                    myUserControl.lblMoneyColor.Background = new SolidColorBrush(Colors.Blue);
                    break;

                    

                default:
                    break;
            }


            //myUserControl.lblMoneyValue.b = myUserControl.MoneyValue / 100 + " р.";
        }


        // Using a DependencyProperty as the backing store for CurrentNumber.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MoneyStateProperty =
            DependencyProperty.Register("MoneyState", typeof(int),
            typeof(ctrlBaraban), new UIPropertyMetadata(2, OnMoneyStatePropertyChanged));


        public int AddMoneyValue
        {
            get { return (int)GetValue(AddMoneyValueProperty); }
            set
            {
                SetValue(AddMoneyValueProperty, value);
            }
        }
        private static void OnAddMoneyValuePropertyChanged(DependencyObject dependencyObject,
               DependencyPropertyChangedEventArgs e)
        {
            ctrlBaraban myUserControl = dependencyObject as ctrlBaraban;
            myUserControl.OnPropertyChanged("AddMoneyValue");
            if (myUserControl.AddMoneyValue == 0)
            {
                myUserControl.lblAddValue.Background = new SolidColorBrush(Colors.White);
                myUserControl.lblAddValue.Content = "";

            }
            else
            {
                myUserControl.lblAddValue.Background = new SolidColorBrush(Colors.LightGreen);
                myUserControl.lblAddValue.Content = "+"+(myUserControl.AddMoneyValue/100).ToString()+ " р.";

            }

           // myUserControl.lblMoneyValue.Content = myUserControl.AddMoneyValue / 100 + " р.";
        }


        // Using a DependencyProperty as the backing store for CurrentNumber.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddMoneyValueProperty =
            DependencyProperty.Register("AddMoneyValue", typeof(int),
            typeof(ctrlBaraban), new UIPropertyMetadata(0, OnAddMoneyValuePropertyChanged));

        private void btnPlus_Click(object sender, RoutedEventArgs e)
        {
            if (true)
            {
                AddMoneyValue = AddMoneyValue - DenomValue;
            }
        }

        private void btnMinus_Click(object sender, RoutedEventArgs e)
        {
            if (AddMoneyValue < 0)
            {
                AddMoneyValue = AddMoneyValue + DenomValue;
            }
        }

    }
}
