using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FiskalDrivers
{
    public partial class MF3 : Form
    {
        public   MF3(string text)
        {
            InitializeComponent();



            this.GotFocus += new EventHandler(MF3_GotFocus);
            this.Activated += new EventHandler(MF3_Activated);
            label1.Text = text;
        }
        public MF3()
        {
            InitializeComponent();



            this.GotFocus += new EventHandler(MF3_GotFocus);
            this.Activated += new EventHandler(MF3_Activated);
        }

        void MF3_Activated(object sender, EventArgs e)
        {
            //MessageBox.Show("MF3_Activated");
          //  this.Focus(); 
        }

        void MF3_GotFocus(object sender, EventArgs e)
        {
            /*
            MessageBox.Show("MF3_GotFocus");
            this.Focus(); 
             * */
        }
        
        

        public bool CancelCheck = false;
        public bool ContinuePrint = false;
        internal void SetLabHeight(int Count)
        {
            label1.Height = 20 + (Count) * 30;
        }
        

       

        private void MF3_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Q)
            {
                this.TopMost = !this.TopMost;
        
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            ContinuePrint = true;
            this.Close(); 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CancelCheck = true;
            this.Close(); 
        }

       
       
    }
}
