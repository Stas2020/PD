using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace PDiscountCard
{
    public partial class KeyBoardControl : UserControl
    {
        bool LowReg = true;
        bool FirstLetter = true;
        TextBox  OwnerTB;
        ComboBox OwnerCB;

        int _MaxLenght = -1;
        internal int MaxLenght
        {
            set {

                _MaxLenght = value;
                textBox1.MaxLength = value;
            }
            get
            {
                return _MaxLenght;
            }
        }
        int _TextBoxRowCount = -1;
        internal int TextBoxRowCount
        {
            set
            {

                _TextBoxRowCount = value;
                textBox1.Height = TextBoxRowCount * 26 + 3;
                this.Height = TextBoxRowCount + 370;

            }
            get
            {
                return _TextBoxRowCount;
            }
        }


        public KeyBoardControl()
        {
            InitializeComponent();
            AddKeys();
            
            //textBox1.Text = OwnerTB.Text;

            ToUpper();

        }


        public KeyBoardControl(TextBox _OwnerTB)
        {
            InitializeComponent();
            AddKeys();
            OwnerTB = _OwnerTB;
            textBox1.Text = OwnerTB.Text;

            ToUpper();

        }

        public KeyBoardControl(ComboBox _OwnerCB)
        {
            InitializeComponent();
            AddKeys();
            OwnerCB = _OwnerCB;
           
            if (OwnerCB != null)
            {
                textBox1.Text = OwnerCB.Text;
            } 
            ToUpper();
        }


        private void UpperCaseInit()
        {
            ToUpper();
        }

        private void AddKeys()
        {
            string[] Leters1 = { "й", "ц", "у", "к", "е", "н", "г", "ш", "щ", "з", "х", "ъ" };
            string[] Leters2 = { "ф", "ы", "в", "а", "п", "р", "о", "л", "д", "ж", "э", "!" };
            string[] Leters3 = { "я", "ч", "с", "м", "и", "т", "ь", "б", "ю", ".", "," };
            string[] Leters4 = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "-", "=", @"\" };

            for (int i = 0; i < Leters1.GetLength(0); i++)
            {
                AddButton(Leters1[i],toolStrip1 );
            }
            for (int i = 0; i < Leters2.GetLength(0); i++)
            {
                AddButton(Leters2[i], toolStrip2);
            }
            for (int i = 0; i < Leters3.GetLength(0); i++)
            {
                AddButton(Leters3[i], toolStrip3);
            }
            for (int i = 0; i < Leters4.GetLength(0); i++)
            {
                AddButton(Leters4[i], toolStrip4);
            }
        }

        private void ToUpper()
        {
            foreach (ToolStripItem TSI in toolStrip1.Items)
            {

                 ToolStripButton TSS = new ToolStripButton();
                if (TSI.GetType()== TSS.GetType()   )
                {
                    ToolStripButton TB = (ToolStripButton)TSI;
                TB.Text = TB.Text.ToUpper(); 
                }
            }
            foreach (ToolStripItem TSI in toolStrip2.Items)
            {

                ToolStripButton TSS = new ToolStripButton();
                if (TSI.GetType() == TSS.GetType())
                {
                    ToolStripButton TB = (ToolStripButton)TSI;
                    TB.Text = TB.Text.ToUpper();
                }
            }
            foreach (ToolStripItem TSI in toolStrip3.Items)
            {

                ToolStripButton TSS = new ToolStripButton();
                if (TSI.GetType() == TSS.GetType())
                {
                    ToolStripButton TB = (ToolStripButton)TSI;
                    TB.Text = TB.Text.ToUpper();
                }
            }
        }

        private void ToLow()
        {
            foreach (ToolStripItem TSI in toolStrip1.Items)
            {

                ToolStripButton TSS = new ToolStripButton();
                if (TSI.GetType() == TSS.GetType())
                {
                    ToolStripButton TB = (ToolStripButton)TSI;
                    TB.Text = TB.Text.ToLower();
                }
            }
            foreach (ToolStripItem TSI in toolStrip2.Items)
            {

                ToolStripButton TSS = new ToolStripButton();
                if (TSI.GetType() == TSS.GetType())
                {
                    ToolStripButton TB = (ToolStripButton)TSI;
                    TB.Text = TB.Text.ToLower();
                }
            }
            foreach (ToolStripItem TSI in toolStrip3.Items)
            {

                ToolStripButton TSS = new ToolStripButton();
                if (TSI.GetType() == TSS.GetType())
                {
                    ToolStripButton TB = (ToolStripButton)TSI;
                    TB.Text = TB.Text.ToLower();
                }
            }
        }

        private void AddButton(string Letter,ToolStrip Tp)
        {
            ToolStripButton TSb = new ToolStripButton(Letter);
            TSb.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            TSb.Name = "toolStripButton" + toolStrip1.Items.Count + 1;
            TSb.AutoSize = false;
            
            TSb.Size = new System.Drawing.Size(60, 50);
            TSb.ForeColor = Color.SlateBlue;
            TSb.Click += new EventHandler(TSb_Click);
            
            Tp.Items.Add(TSb);
            Tp.Items.Add(new ToolStripSeparator());



            // this.toolStrip1.  
        }


        int CurrentRowCount = 0;
        void TSb_Click(object sender, EventArgs e)
        {
            ToolStripButton TsB = (ToolStripButton)sender  ;
            if ((TextBoxRowCount>0)&& (CurrentRowCount > TextBoxRowCount))
            {
                return;
            }
            textBox1.Text += TsB.Text;
            if (MaxLenght > 0)
            {
                if ((textBox1.Text.Replace(Environment.NewLine,"").Length)  % (MaxLenght) == 0)
                {
                    CurrentRowCount++;
                    textBox1.Text += Environment.NewLine;
                }
            }
            
            
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            if (LowReg)
            {
                ToUpper();
            }
            else
            {
                ToLow();
            }
            LowReg = !LowReg;
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            if ((TextBoxRowCount > 0) && (CurrentRowCount > TextBoxRowCount))
            {
                return;
            }
            textBox1.Text += " ";
            if (MaxLenght > 0)
            {
                if ((textBox1.Text.Replace(Environment.NewLine, "").Length) % (MaxLenght) == 0)
                {
                    CurrentRowCount++;
                    textBox1.Text += Environment.NewLine;
                }
            }
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
            {
                textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length - 1);
            }
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (FirstLetter)
            {
                FirstLetter = false;
                ToLow();
            }

            if (textBox1.Text == "")
            {
                ToUpper();
            }
            
            if (OwnerTB != null)
            {
                OwnerTB.Text = textBox1.Text;
            }
            if (OwnerCB != null)
            {
                OwnerCB.Text = textBox1.Text;
            }
            textBox1.SelectionStart = textBox1.Text.Length; 



            //textBox1.  
        }

        public delegate void BtnOkEventHandler(string Mess,object sender);
         public event BtnOkEventHandler BtnOkEvent;
        private void SendBtnOkEvent(string Message)
        {
            if (BtnOkEvent != null)
            {
                BtnOkEvent(Message,this);
            }
        }
        public delegate void BtnCancelEventHandler(object sender);
        public event BtnCancelEventHandler BtnCancelEvent;
        private void SendBtnCancelEvent()
        {
            if (BtnCancelEvent != null)
            {
                BtnCancelEvent(this);
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            SendBtnOkEvent(textBox1.Text);
            
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            SendBtnCancelEvent();    
       }

        private void textBox1_Resize(object sender, EventArgs e)
        {
            
        }

        private void KeyBoardControl_Resize(object sender, EventArgs e)
        {
            textBox1.Height = Height - toolStrip1.Height -
                toolStrip2.Height - toolStrip3.Height
                 - toolStrip4.Height - toolStrip5.Height - toolStrip6.Height;

        }
    }
}
