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

namespace SVSistem
{
    /// <summary>
    /// Interaction logic for ctrlNumPad.xaml
    /// </summary>
    public partial class ctrlNumPad : UserControl
    {
        public ctrlNumPad()
        {
            InitializeComponent();
        }

        public delegate void SendKeyEventHandler(object sender, Char C);
        public event SendKeyEventHandler SendKeyEvent;

        public delegate void SendEventHandler(object sender);
        public event SendEventHandler SendOkEvent;

       
        public event SendEventHandler SendClearEvent;

       

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            char s = ((Button)sender).Content.ToString()[0];
            if (s >= '0' && s <= '9')
            {
                if (SendKeyEvent != null)
                {
                    SendKeyEvent(this, s);
                }
            }
            else if (s == 'C')
            {
                if (SendClearEvent != null)
                {
                    SendClearEvent(this);
                }
            }
            else if (s == 'O')
            {
                if (SendOkEvent != null)
                {
                    SendOkEvent(this);
                }
            }
        }
    }
}
