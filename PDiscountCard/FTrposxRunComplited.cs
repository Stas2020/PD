using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PDiscountCard
{
    public partial class FTrposxRunComplited : MF3
    {
        public FTrposxRunComplited()
        {
            InitializeComponent();
            button1.Tag = this;
            button2.Tag = this;
            button3.Tag = this; 
        }
        public void Init(string Caption, string Mess)
        {
            label1.Text = Caption ;
            label2.Text = Mess;
        }
        public void UpdateMess(string Mess)
        {
            Utils.ToCardLog("[UpdateMess] " + Mess);
            label2.Text = Mess;
            if (WindowInFone)
            {
                Alert();
            }

        }
        public void AddMess(string Mess)
        {
            Utils.ToCardLog("[AddMess] " + Mess);
            label2.Text += Mess;
            if (WindowInFone)
            {
                Alert();
            }
        }
        Size OldS;
        Point  OldPosition;
        bool WindowInFone=false ;
        public void ToFone()
        {
            OldS = this.Size;
            OldPosition = new Point (this.Left, this.Top);
            this.Size = new Size(100, 50);
            //this.Click += new EventHandler(FTrposxRunComplited_Click);
            panel13.Visible = true ;
            WindowInFone = true;
        }
        
        private  void mActivate()
        {
            try
            {
                this.Size = OldS;
                this.Top = OldPosition.Y;
                this.Left = OldPosition.X;
                WindowInFone = false;
                timer1.Enabled = false;
                panel13.BackColor = Color.WhiteSmoke;
                this.panel13.BackgroundImage = global::PDiscountCard.Properties.Resources.phone;
            }
            catch
            { }
        }

        void FTrposxRunComplited_Click(object sender, EventArgs e)
        {
            try
            {
                this.Size = OldS;
            }
            catch
            { }
        }

        

        private bool FormMove = false;
        private void panel13_MouseDown(object sender, MouseEventArgs e)
        {
            FormMove = true;
            OldMousePos.X = this.PointToScreen(e.Location).X;
            OldMousePos.Y = this.PointToScreen(e.Location).Y;
            OldP.X = e.Location.X;
            OldP.Y = e.Location.Y; 
        }
        Point OldP = new Point();
        Point OldMousePos = new Point();

        bool MouseMoveEn = true ;
        private void panel13_MouseMove(object sender, MouseEventArgs e)
        {
           // if (MouseMoveEn)
            {
                if (FormMove)
                {
                    this.SuspendLayout();
                    this.Left = (this.PointToScreen(e.Location).X - OldP.X);
                    this.Top = (this.PointToScreen(e.Location).Y - OldP.Y);
                    this.ResumeLayout();
                }
            }
            MouseMoveEn = !MouseMoveEn;
        }

        private void panel13_MouseUp(object sender, MouseEventArgs e)
        {
            FormMove = false;
            if ((Math.Abs(OldMousePos.X - this.PointToScreen(e.Location).X) < 10) && (Math.Abs(OldMousePos.Y - this.PointToScreen(e.Location).Y) < 10))
            {
                panel13.Visible = false;
                mActivate();                
            }
        }
        
        public void Alert()
        {
            timer1.Enabled = true;
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (panel13.BackColor == Color.WhiteSmoke )
            {
                panel13.BackColor = Color.Red;
                this.panel13.BackgroundImage = global::PDiscountCard.Properties.Resources.phone_sound;
            }
            else
            {
                panel13.BackColor = Color.WhiteSmoke;
                this.panel13.BackgroundImage = global::PDiscountCard.Properties.Resources.phone;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Cancel = true;
            this.Close(); 
        }




    }
}
