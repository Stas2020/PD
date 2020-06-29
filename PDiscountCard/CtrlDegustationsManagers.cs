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
    public partial class CtrlDegustationsManagers : UserControl
    {
        public CtrlDegustationsManagers()
        {
            InitializeComponent();
            
           
        }
        internal void FillManagerList()
        {
            comboBox1.Items.Clear();
            foreach (CEmpl emp in AlohaTSClass.GetManagersList())
            {
                comboBox1.Items.Add(emp);
            }
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            ((FDegustations)this.ParentForm).ManagersSelectOk((CEmpl)comboBox1.SelectedItem);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.ParentForm.Close();  
        }
        private int PnlScrollBarWidth = 60;
        private int PnlScrollBarHeight = 40;
        private void CtrlDegustationsManagers_Resize(object sender, EventArgs e)
        {
            
            PnlScrollBar.Width =PnlScrollBarWidth ;
            PnlScrollBar.Left = comboBox1.Width - PnlScrollBar.Width;
            PnlScrollBar.Top = comboBox1.Top + (44);
            PnlScrollBar.Height = comboBox1.Height - 44;
            btnOk.Width = this.Width / 2; 
            

        }

        private void btnScrollUp_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex > 0)
            {
                comboBox1.SelectedIndex--;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex < comboBox1.Items.Count-1)
            {
                comboBox1.SelectedIndex++;
            }
        }
    }
    
}
