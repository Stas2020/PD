using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OrderToAlohaTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddOrder();
        }
        private void AddOrder()
        { 
            List<OrderToAloha.Item> itms = new List<OrderToAloha.Item> ();
            itms.Add(new  OrderToAloha.Item{
            BarCode = 22110,
            Count = 2,
            Price = 100,
            SourceBase = 8
            });
            /*
            itms.Add(new OrderToAloha.Item
            {
                BarCode = 22110,
                Count = 3,
                Price = 1250,
                SourceBase = 8
            });
            
            itms.Add(new OrderToAloha.Item
            {
                BarCode = 8438,
                Count = 1,
                Price = 1243,
                SourceBase = 0
            });
             * */
            try
            {
                int TableNum = 0;
                int ChNum = 0;
                //OrderToAloha.OrderToAloha.SendOrderToAloha(125, itms, 1,"Север","", 1, 0, out ChNum, out TableNum);
                MessageBox.Show(TableNum.ToString());
            }
             
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {

                //OrderToAloha.OrderToAloha.DeleteOrder(;
                MessageBox.Show("Ok ");
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                //OrderToAloha.OrderToAloha.CloseOrder (3,1);
                MessageBox.Show("Ok ");
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

     
    }
}
