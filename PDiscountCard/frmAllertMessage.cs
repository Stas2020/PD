using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PDiscountCard
{
    public partial class frmAllertMessage : Form
    {
        private System.Timers.Timer T1;
        string message;
        public frmAllertMessage(string _message)
        {
            Utils.ToCardLog("frmAllertMessage " + _message);
            message = _message;
            InitializeComponent();
            TopMost = true;
            label1.Text = message;
            T1 = new System.Timers.Timer() ;
            T1.Interval = 1000;
            T1.Elapsed += new System.Timers.ElapsedEventHandler(T1_Elapsed);
            T1.Start();
            this.FormClosing += new FormClosingEventHandler(frmAllertMessage_FormClosing);
                        
        }

        void frmAllertMessage_FormClosing(object sender, FormClosingEventArgs e)
        {
            T1.Stop();
        }

        void T1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (T1.Enabled)
            {
                try
                {
                    Utils.ToCardLog("Window frmAllertMessage is shown. Text:" + message);
                    WinApi.ShowTopmost(this,Top ,Left ,Width,Height);
                }
                catch
                { 
                
                }
            }
        }

        
        

        public bool Cancel = false;
        internal void SetLabHeight(int Count)
        {
            label1.Height = 20 + (Count) * 30;
        }
        

        private void button3_Click(object sender, EventArgs e)
        {
            Cancel = true;
            this.Close(); 
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
            T1.Stop();
            this.Close();
        }

       
       
    }
}
