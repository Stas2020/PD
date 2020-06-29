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

namespace PDiscountCard
{
    /// <summary>
    /// Interaction logic for ctrlDigitalKeyBoard.xaml
    /// </summary>
    public partial class ctrlDigitalKeyBoard : UserControl
    {
        public ctrlDigitalKeyBoard()
        {
            InitializeComponent();
            CurentText = "";
            BtnPoint.Content = GetDemSep().ToString();
        }



   public static readonly RoutedEvent ClickOkEvent;
   
   // Регистрация события
   static ctrlDigitalKeyBoard()
   {
       ctrlDigitalKeyBoard.ClickOkEvent = EventManager.RegisterRoutedEvent(
         "ClickOk", RoutingStrategy.Direct,
         typeof(RoutedEventHandler), typeof(ctrlDigitalKeyBoard));
   }
   
   // Традиционная оболочка события
   public event RoutedEventHandler ClickOk
   {
      add
      {
          base.AddHandler(ctrlDigitalKeyBoard.ClickOkEvent, value);
      }
      remove
      {
          base.RemoveHandler(ctrlDigitalKeyBoard.ClickOkEvent, value);
      }
   }

        public static readonly DependencyProperty CanCanceledProperty =
    DependencyProperty.Register("CanCanceled", typeof(bool), typeof(ctrlDigitalKeyBoard), new UIPropertyMetadata(true, OnCanCanceledPropertyChanged));

        public bool CanCanceled
        {
            get { return (bool)GetValue(CanCanceledProperty); }
            set { SetValue(CanCanceledProperty, value); }
        }

        private static void OnCanCanceledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool newPropertyValue = (bool)e.NewValue;
            ctrlDigitalKeyBoard instance = (ctrlDigitalKeyBoard)d;
            instance.CancelEnable = newPropertyValue;
        }

        public static readonly DependencyProperty IntOnlyProperty =
    DependencyProperty.Register("IntOnly", typeof(bool), typeof(ctrlDigitalKeyBoard), new UIPropertyMetadata(false, OnIntOnlyPropertyChanged));

        public bool IntOnly
        {
            get { return (bool)GetValue(IntOnlyProperty); }
            set { SetValue(IntOnlyProperty, value); }
        }

        private static void OnIntOnlyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool newPropertyValue = (bool)e.NewValue;
            ctrlDigitalKeyBoard instance = (ctrlDigitalKeyBoard)d;
            instance.BtnPoint.Visibility = (newPropertyValue ? Visibility.Hidden : Visibility.Visible);
        }


        
        


        public delegate void OkEventHandler(object sender, double Value);
        public  event OkEventHandler OnOk;
        public event OkEventHandler OnCancel;


        private  bool _OkEnable = true;
        public bool OkEnable
        {
            set {
                _OkEnable = value;
                if (value)
                {
                    BtnOk.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    BtnOk.Visibility = System.Windows.Visibility.Collapsed;
                }
            
            }
            get {
                return _OkEnable;
            }
        }

        private bool _CancelEnable = true;
        public bool CancelEnable
        {
            set
            {
                _CancelEnable = value;
                if (value)
                {
                    BtnCancel.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    BtnCancel.Visibility = System.Windows.Visibility.Collapsed;
                }

            }
            get
            {
                return _CancelEnable;
            }
        }
        

        private char GetDemSep()
        {
            return (1.1).ToString()[1];
        }

        string curentText = "";

        public bool NextTouchClear = false;
      public  string CurentText
        {
            set
            {
                curentText = value;
                txt.Text = curentText;
            }
            get
            {
                return curentText;
            }
        }

        public  double CurentDouble
        {
            get
            { 
                double res=0;
                Double.TryParse(CurentText, out res);
                return res;
            }
        }

        private void Btn1_Click(object sender, RoutedEventArgs e)
        {
            if (NextTouchClear)
            {
                CurentText = "";
                NextTouchClear = false;
            }
            string Cont = ((Button)sender).Content.ToString();
            if (CurentText.Length < 13)
            {
                if (Cont == GetDemSep().ToString())
                {
                    if (CurentText.Contains(GetDemSep().ToString()))
                    {
                        return;
                    }

                }
                CurentText = CurentText + Cont;
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (OnOk != null)
            {
                OnOk(this, CurentDouble);
            }
            
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (OnCancel != null)
            {
                OnCancel(this, CurentDouble);
            }
        }

        private void BtnErase_Click(object sender, RoutedEventArgs e)
        {
            if (CurentText.Length > 0)
            {
                CurentText = CurentText.Substring(0, CurentText.Length - 1);
            }
        }
    }
}
