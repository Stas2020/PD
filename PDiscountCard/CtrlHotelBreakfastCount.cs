using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PDiscountCard
{
    public partial class CtrlHotelBreakfastCount : UserControl
    {
        public CtrlHotelBreakfastCount()
        {
            InitializeComponent();
        }
        internal void SetCaption(string  Caption)
        {
            label1.Text = Caption;
            
        }

        public bool IsSmall
        {
            get
            {
                return checkBox1.Checked;
            }
        }

        public double Price
        {
            get
            {
                if (checkBox2.Checked) return 1800;
                if (checkBox3.Checked) return 900;

                return -999999999.000000;
            }
        }


        
        int guestCount = 1;
        internal int GuestCount {
            set
            {
                if (value > 0)
                {
                    guestCount = value;
                    textBox1.Text = guestCount.ToString();
                }
            }
            get
            {
                return guestCount;
            }
        
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GuestCount++;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GuestCount--;
        }

    }
}
