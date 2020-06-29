using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OrderToAlohaTest
{
    public partial class CtrlOrderItems : UserControl
    {
        public CtrlOrderItems()
        {
            InitializeComponent();

            bindingSource2.DataSource = DGToOrderItms;
            dataGridView2.DataSource = bindingSource2;

          
            bindingSource1.DataSource = DGAllItms;
            dataGridView1.DataSource = bindingSource1;

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
        }

        List<OrderToAloha.Item> DGToOrderItms = new List<OrderToAloha.Item>();
        List<OrderToAloha.ItemExt> DGAllItms = new List<OrderToAloha.ItemExt>();

        private void button2_Click(object sender, EventArgs e)
        {
            AddFreeItm();
        }

        private void AddFreeItm()
        {
            try
            {
                OrderToAloha.Item itm = new OrderToAloha.Item()
                {
                    BarCode = int.Parse(textBox1.Text),
                    Count = int.Parse(textBox4.Text),
                    Price = int.Parse(textBox3.Text),

                };

                DGToOrderItms.Add(itm); 
                bindingSource2.ResetBindings(true);
                
            }

            catch
            { }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            RemItem();
        }

        private void RemItem()
        {
            DGToOrderItms.Remove((OrderToAloha.Item)dataGridView2.SelectedRows[0].DataBoundItem);
            bindingSource2.ResetBindings(true);
        }


        OrderToAloha.OrderToAloha OA;

        private void AddOrder()
        {
            try
            {
                OA = new OrderToAloha.OrderToAloha();
                OA.ResponseEvent += new OrderToAloha.OrderToAloha.ResponseEventHandler(OA_ResponseEvent);
                DataReciver.SendOrderToAlohaRequest req = new DataReciver.SendOrderToAlohaRequest()
                {
                    OrderId = int.Parse(textBox5.Text),
                    Items = DGToOrderItms,
                    CompanyId = int.Parse(textBox6.Text),
                    CompanyName = textBox7.Text,
                    BortName = textBox8.Text,
                    DiscountId = comboBox1.SelectedIndex,
                    Margin = comboBox2.SelectedIndex,
                    TimeOfShipping = DateTime.Now,
                    RemoteCompName = "Colstend4",
                    port = 64788
                };

                OA.SendOrderToAloha(req);
            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        private void AddOrderFromMe()
        {
            try
            {
                OA = new OrderToAloha.OrderToAloha();
                OA.ResponseEvent += new OrderToAloha.OrderToAloha.ResponseEventHandler(OA_ResponseEvent);
                DataReciver.SendOrderToAlohaRequest req = new DataReciver.SendOrderToAlohaRequest()
                {
                    OrderId = int.Parse(textBox5.Text),
                 //   Items = DGToOrderItms,
                    CompanyId = int.Parse(textBox6.Text),
                    CompanyName = textBox7.Text,
                    BortName = textBox8.Text,
                    DiscountId = comboBox1.SelectedIndex,
                    Margin = comboBox2.SelectedIndex,
                    TimeOfShipping = DateTime.Now,
                    RemoteCompName = "Colstend4",
                    port = 64788
                };
                req.Items = new List<OrderToAloha.Item>();
                OrderToAloha.Item it = new OrderToAloha.Item()
                {
                    BarCode = int.Parse(textBox1.Text),
                    Count = int.Parse(textBox4.Text),
                    Price = decimal.Parse(textBox3.Text),

                };
                req.Items.Add(it);
                OA.SendOrderToAloha(req);
            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        void OA_ResponseEvent(object sender, DataReciver.AlohaResponse e)
        {
            if (e.ResultId == 0)
            {
                MessageBox.Show(e.Err);
            }
            else
            {
                MessageBox.Show(e.AlohaTableNum.ToString());
            }

            ((OrderToAloha.OrderToAloha)sender).CloseConnection();

        }

        

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {

                OrderToAloha.OrderToAloha OA = new OrderToAloha.OrderToAloha(0);

                DGAllItms.AddRange(OA.GetAllItms());
                bindingSource1.ResetBindings(true);
            }
            catch
            {

            }
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            OrderToAloha.ItemExt itm = (OrderToAloha.ItemExt)dataGridView1.SelectedRows[0].DataBoundItem;
            itm.Count = int.Parse(textBox2.Text);
            DGToOrderItms.Add(itm);
            bindingSource2.ResetBindings(true);
        }

        private void button6_Click(object sender, EventArgs e)
        {

            

            AddOrder();
        }


     
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                DataReciver.DeleteOrderRequest r = new DataReciver.DeleteOrderRequest()
                {
                    OrderId = 1,
                    port = 0,
                    RemoteCompName = "ColStend4"
                };
                //OrderToAloha.OrderToAloha.DeleteOrder(r);
                MessageBox.Show("Ok ");
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                //OrderToAloha.OrderToAloha.CloseOrder(3, 1);
                MessageBox.Show("Ok ");
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            AddOrderFromMe();
        }

    }

    class DGItem : OrderToAloha.Item, INotifyPropertyChanged
    {


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }


}
