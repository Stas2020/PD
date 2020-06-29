using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PDiscountCard
{
    public partial class frmCardMoover : Form
    {
        public frmCardMoover()
        {
            InitializeComponent();
        }

        internal void SetInfo(string txt)
        {
            this.BeginInvoke((Action)(() => 
                {
                    //Utils.ToCardLog("[SetInfo] " + txt); 
                    label2.Text = txt;
                }
            ));
            //Utils.ToCardLog("[SetInfo] " + txt);
            //label2.Text = txt;
        }

        internal void DisableButton()
        {
            this.BeginInvoke((Action)(() =>
            {
                button2.Enabled = false ;
                
            }
           ));
        }
        internal void EnableButton()
        {
            this.BeginInvoke((Action)(() =>
            {
                button2.Enabled = true ;

            }
           ));
        }

        internal void SetOk()
        {
             this.BeginInvoke((Action)(() => 
                {
                    button2.Enabled = true;
            button2.Text = "Ok";
                }
            ));
        }
        internal bool Cancel = false;
        private void button2_Click(object sender, EventArgs e)
        {
            Cancel = true;
            Utils.ToCardLog("frmCardMoover Нажал на кнопку " + button2.Text);
            //AlohaTSClass.DeleteLoyaltyMember(AlohaTSClass.AlohaCurentState.WaterId, (int)AlohaTSClass.AlohaCurentState.TableId, (int)AlohaTSClass.AlohaCurentState.CheckId);
            MainClass.CurentfrmCardMooverEnable = false;
            MainClass.AssignLoyaltiStateTimerEnabled = false;
            MainClass.AssignLoyaltiStateInProcess = false;
            this.Close();
            //this = null;

        }

    }
}
