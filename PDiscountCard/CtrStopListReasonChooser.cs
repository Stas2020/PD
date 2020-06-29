
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
    public partial class CtrStopListReasonChooser : UserControl
    {
        public CtrStopListReasonChooser()
        {

            InitializeComponent();
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        public  StopListDish Dish;
        public int ReasonId 
        {
            get
            {
                if (SelectedType() == null)
                {
                    return 0;

                }
                else
                {
                    return (int)SelectedType().TypeNum;
                }

            }
        
        }
        public CtrStopListReasonChooser(StopListDishReason mDish, List<StopListService.StopListReasonT> Reasons)
        {
            InitializeComponent();
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            label1.Text = mDish.Name;
            mDish.mCtrStopListReasonChooser = this;
            Dish = mDish;
            
            FillReasons(Reasons);
        }

        public void FillReasons(List<StopListService.StopListReasonT> Reasons)
        {
            comboBox1.ItemHeight = 60;
            comboBox1.DisplayMember="Name";
            foreach (StopListService.StopListReasonT R in Reasons)
            {
                
                comboBox1.Items.Add(R);
                if (Dish.Reason > 0)
                {
                    if (R.TypeNum == Dish.Reason)
                    {
                        comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
                    }
                }
            }




        }
        public StopListService.StopListReasonT SelectedType()
        {
            if (comboBox1.SelectedItem == null)
            {
                return null;
            }
            else
            {
                return (StopListService.StopListReasonT)comboBox1.SelectedItem;
            }
        }

        
    }
}
