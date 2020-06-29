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

namespace PDiscountCard.GuestCount
{
    /// <summary>
    /// Interaction logic for CtrlGuestCountAsk.xaml
    /// </summary>
    public partial class CtrlGuestCountAsk : UserControl
    {
        public CtrlGuestCountAsk()
        {
            InitializeComponent();
            digitKeyboard.OnOk += new ctrlDigitalKeyBoard.OkEventHandler(digitKeyboard_OnOk);
        }

        void digitKeyboard_OnOk(object sender, double Value)
        {
            if (digitKeyboard.CurentDouble > 0)
            {
                AlohaTSClass.SetGuestCountAttr((int)digitKeyboard.CurentDouble);
                OwnerWnd.Close();
            }
        }

        

        private void SetGuestCount()
        { 
        
        }

        private Window OwnerWnd;
        internal void SetOwnerWnd(Window _OwnerWnd)
        {
            OwnerWnd = _OwnerWnd;
        }

    }
}
