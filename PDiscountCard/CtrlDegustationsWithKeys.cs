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
    public partial class CtrlDegustationsWithKeys : UserControl
    {
        public CtrlDegustationsWithKeys()
        {
            InitializeComponent();
        }
        internal void SetCaption(string  Caption)
        {
            label1.Text = Caption;
            
        }

        private void keyBoardControl1_Load(object sender, EventArgs e)
        {

        }
    }
}
