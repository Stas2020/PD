using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PDiscountCard.RusMessage
{
    public partial class FrmRussMessage : Form
    {
        Dish ModifDish;
        Check ModifChk;
        public FrmRussMessage(string DishName)
        {
            InitializeComponent();
            label1.Text = "Модификатор для блюда " + DishName;
            keyBoardControl1.BtnCancelEvent += new KeyBoardControl.BtnCancelEventHandler(keyBoardControl1_BtnCancelEvent);
            keyBoardControl1.BtnOkEvent += new KeyBoardControl.BtnOkEventHandler(keyBoardControl1_BtnOkEvent);
            keyBoardControl1.MaxLenght = 14;
            keyBoardControl1.TextBoxRowCount = 6;
        }

        void keyBoardControl1_BtnOkEvent(string Mess, object sender)
        {
            AddModif = true;
            ModifTxt = Mess;

            this.Close(); 

        }



        internal bool AddModif = false;
        internal string ModifTxt = "";
        void keyBoardControl1_BtnCancelEvent(object sender)
        {
            this.Close();
        }

        private void keyBoardControl1_Load(object sender, EventArgs e)
        {

        }
    }
}
