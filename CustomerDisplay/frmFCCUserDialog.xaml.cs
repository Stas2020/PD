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
using System.Windows.Threading;

namespace CustomerDisplay
{
    /// <summary>
    /// Interaction logic for frmFCCUserDialog.xaml
    /// </summary>
    public partial class frmFCCUserDialog : Window
    {
        private BitmapImage[] MoneyInImages;
        private BitmapImage[] MoneyOutImages;
        private BitmapImage[] PresentImages;

        private const bool AlanStyle = true;

        public frmFCCUserDialog()
        {
            InitializeComponent();

            GrPictures.Visibility = System.Windows.Visibility.Visible;
            GrPayment.Visibility = System.Windows.Visibility.Hidden;


            PresentImages = new BitmapImage[3];
            PresentImages[0] = new BitmapImage(new Uri("/CustomerDisplay;component/Images/EP_GloryScreen_800x600px3.jpg", UriKind.Relative));
            PresentImages[1] = new BitmapImage(new Uri("/CustomerDisplay;component/Images/EP_GloryScreen_800x600px4.jpg", UriKind.Relative));
            PresentImages[2] = new BitmapImage(new Uri("/CustomerDisplay;component/Images/EP_GloryScreen_800x600px5.jpg", UriKind.Relative));

            if (AlanStyle)
            {
                MoneyInImages = new BitmapImage[2];
                MoneyInImages[0] = new BitmapImage(new Uri("/CustomerDisplay;component/Images/Alan/MoneyIn1.jpg", UriKind.Relative));
                MoneyInImages[1] = new BitmapImage(new Uri("/CustomerDisplay;component/Images/Alan/MoneyIn2.jpg", UriKind.Relative));

                MoneyOutImages = new BitmapImage[2];
                MoneyOutImages[0] = new BitmapImage(new Uri("/CustomerDisplay;component/Images/Alan/MoneyIn1.jpg", UriKind.Relative));
                MoneyOutImages[1] = new BitmapImage(new Uri("/CustomerDisplay;component/Images/Alan/MoneyIn2.jpg", UriKind.Relative));
            
            }
            else
            {
                MoneyInImages = new BitmapImage[2];
                MoneyInImages[0] = new BitmapImage(new Uri("/CustomerDisplay;component/Images/GloriImg1.jpg", UriKind.Relative));
                MoneyInImages[1] = new BitmapImage(new Uri("/CustomerDisplay;component/Images/GloriImg2.jpg", UriKind.Relative));

                MoneyOutImages = new BitmapImage[2];
                MoneyOutImages[0] = new BitmapImage(new Uri("/CustomerDisplay;component/Images/MoneyOut1.jpg", UriKind.Relative));
                MoneyOutImages[1] = new BitmapImage(new Uri("/CustomerDisplay;component/Images/MoneyOut2.jpg", UriKind.Relative));
            }
            DispatcherTimer dT = new DispatcherTimer();
            dT.Interval = new TimeSpan(0, 0, 0,0,500);
            dT.Tick += new EventHandler(dT_Tick);
            dT.Start();
        
        }

        private int PictureState = 0;
        private int counter = 0;

        private int PresentPicturesInterval = 10;
        
        private int EndPaymentInterval = 15;
        
        private bool NeedEndPayment = false;
        private DateTime EndPaymentStartDT;


        public void StartEndPayment()
        {
            EndPaymentStartDT = DateTime.Now;
            NeedEndPayment = true;

        }

        public void ShowPaiment(bool Vis)
        {
            if (Vis)
            {
                
                GrPayment.Visibility = System.Windows.Visibility.Visible;
                GrPictures.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                GrPayment.Visibility = System.Windows.Visibility.Hidden;
                GrPictures.Visibility = System.Windows.Visibility.Visible;
            }
            NeedEndPayment = false;
        }


        void dT_Tick(object sender, EventArgs e)
        {
            if (NeedEndPayment)
            {
                if (DateTime.Now > EndPaymentStartDT.AddSeconds(EndPaymentInterval))
                {
                    ShowPaiment(false);
                    ClearCheck();
                    ClearMoney();
                    mMoneyDialog.SetStatus("Добро пожаловать");
                    
                }
            }

            if (PictureState == 1)
            {
                GloryImg.Source = MoneyInImages[counter % MoneyInImages.Length];
                
            }
            else if (PictureState == 2)
            {
                GloryImg.Source = MoneyOutImages[counter % MoneyInImages.Length];
                
            }


            if (counter % PresentPicturesInterval == 0)
            {
                if (GrPictures.Visibility == System.Windows.Visibility.Visible)
                {
                    ImgPictures1.Source = PresentImages[(counter / PresentPicturesInterval) % PresentImages.Length];
                }
            
            }


            counter++;
        }

        public void SetPictureState(int PS)
        {
            PictureState = PS;
        }
        public void ShowMe()
        {
            this.Show();
        }

        public ctrlMoneyDialog GetMoneyDialog()
        {
            return mMoneyDialog;
        }
        public void SetBill(int Summ)
        {
            mMoneyDialog.SetTotal(Summ);
        }

        public void SetDishez(CustomerCheck Chk)
        {
            if (Chk != null)
            {
                ctrlOrder.SetDishez(Chk);
            }
        }
        public void ClearOrder()
        {
            this.Dispatcher.Invoke((Action)(() =>
                {
                    ctrlOrder.ClearDishez();
                }));
        }
        public void ClearCheck()
        {
            this.Dispatcher.Invoke((Action)(() =>
                {

                    ClearMoney();
                    ctrlOrder.ClearDishez();
                }));

        }
        public void ClearMoney()
        {
            this.Dispatcher.Invoke((Action)(() =>
                {
                    mMoneyDialog.SetTotal(0);
                    mMoneyDialog.SetAlredy(0);
                    mMoneyDialog.SetChange(0);
                    mMoneyDialog.SetDeposit(0);
                }));
        }



    }
}
