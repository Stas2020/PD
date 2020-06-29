using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace PDiscountCard
{
    public partial class UnitKeyboard : UserControl
    {
        private int TPosition = 1;
        int hoursD;
        int hoursEd;
        int minutesD;
        int minutesEd;

        DateTimePicker DTMP;
            
        
        public UnitKeyboard()
        {
            
            InitializeComponent();
            /*
            DTMP = DTmP; 
            DateTime DTM = DTmP.Value; 
             hoursD = Convert.ToInt16(DTM.Hour /10 );
             hoursEd = DTM.Hour - Convert.ToInt16(DTM.Hour / 10)*10 ;
             minutesD = Convert.ToInt16(DTM.Minute / 10); ;
             minutesEd = DTM.Minute - Convert.ToInt16(DTM.Minute / 10) * 10;

             textBox1.Text = hoursD.ToString() + hoursEd.ToString() + ":" + minutesD.ToString() + minutesEd.ToString();
            */

        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            if (TPosition <3 )
            {
                TPosition++;
                if (TPosition < 3)
                {
                    textBox1.SelectionStart = TPosition - 1;
                }
                else
                {
                    textBox1.SelectionStart = TPosition;
                }
            }            
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            ToolStripButton TSB = (ToolStripButton)sender;
            ChangeText(Convert.ToInt16(TSB.Text));
        }

        private void ChangeText(int Val)
        {
            if (textBox1.Text.Length < 11)
            {
                textBox1.Text += Val;
            }
            
            /*
            if (TPosition == 1)
            {
                if (Val < 3)
                {
                    hoursD = Val;

                    if ((hoursD==2)&&(hoursEd>3))
                    {
                        hoursEd =0;
                    }

                    ChangeTextBox();
                }
                
            }
            else  if (TPosition == 2)
            {
                if (hoursD == 2)
                {
                    if (Val < 4)
                    {
                        hoursEd = Val;
                        ChangeTextBox();
                    }
                }
                else
                {
                    hoursEd = Val;
                    ChangeTextBox();
                }
            }
            else if (TPosition == 3)
            {
                if (Val < 7)
                {
                    minutesD  = Val;
                    ChangeTextBox();
                }
            }
            else if (TPosition == 4)
            {
                
                    minutesEd = Val;
                    ChangeTextBox();
                
            }
        */

        }


        private void ChangeTextBox()
        {
           
            
           // textBox1.Text = hoursD.ToString() + hoursEd.ToString() + ":" + minutesD.ToString() + minutesEd.ToString();
            textBox1.SelectionLength = 1;
            /*
            if (TPosition == 4)
            {
                TPosition = 0;
            }
             * */
            TPosition++;

            if (TPosition < 3)
            {
                textBox1.SelectionStart = TPosition-1 ;
            }
            else
            {
                textBox1.SelectionStart = TPosition;
            }

            
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            ToolStripButton TSB = (ToolStripButton)sender;
            ChangeText(Convert.ToInt16(TSB.Text));
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            ToolStripButton TSB = (ToolStripButton)sender;
            ChangeText(Convert.ToInt16(TSB.Text));
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            ToolStripButton TSB = (ToolStripButton)sender;
            ChangeText(Convert.ToInt16(TSB.Text));
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            ToolStripButton TSB = (ToolStripButton)sender;
            ChangeText(Convert.ToInt16(TSB.Text));
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            ToolStripButton TSB = (ToolStripButton)sender;
            ChangeText(Convert.ToInt16(TSB.Text));
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ToolStripButton TSB = (ToolStripButton)sender;
            ChangeText(Convert.ToInt16(TSB.Text));

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            ToolStripButton TSB = (ToolStripButton)sender;
            ChangeText(Convert.ToInt16(TSB.Text));

        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            ToolStripButton TSB = (ToolStripButton)sender;
            ChangeText(Convert.ToInt16(TSB.Text));

        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
            {
                textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.Length - 1);
            }
            /*
            if (TPosition > 0)
            {
                textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.Length - 1);
                TPosition--;
                if (TPosition > 0)
                {
                    textBox1.SelectionStart = TPosition - 1;
                }
                else
                {
                    textBox1.SelectionStart = TPosition;
                }
            }
             * */
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            ToolStripButton TSB = (ToolStripButton)sender;
            ChangeText(Convert.ToInt16(TSB.Text));


            
        }

        private void toolStripButton15_Click(object sender, EventArgs e)
        {
            this.ParentForm.Close();  
        }

        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            ShowScreenVoid(Convert.ToInt32(textBox1.Text));
            //((VoidFrm)this.ParentForm).EnterrNumber(Convert.ToInt32(textBox1.Text));
        }

        public delegate void NumberEnterDelegate(object sender, int Num);

        public event NumberEnterDelegate NumberEnterEvent;

        private void ShowScreenVoid(int Num)
        {
            if (NumberEnterEvent != null)
            {
                NumberEnterEvent(this, Num);
            }
        }

    }
}
