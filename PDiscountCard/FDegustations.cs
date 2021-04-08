using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PDiscountCard
{
    public partial class FDegustations : Form
    {
        public FDegustations()
        {
            InitializeComponent();
            ctrlDegustationsWithKeys1.keyBoardControl1.BtnCancelEvent += new KeyBoardControl.BtnCancelEventHandler(keyBoardControl1_BtnCancelEvent);
            ctrlDegustationsWithKeys1.keyBoardControl1.BtnOkEvent += new KeyBoardControl.BtnOkEventHandler(keyBoardControl1_BtnOkEvent);

         /*
            btn18.Visible = AlohaTSClass.CompEnable(18);
            btn17.Visible = AlohaTSClass.CompEnable(17);
            btn16.Visible = AlohaTSClass.CompEnable(16);
            btn21.Visible = AlohaTSClass.CompEnable(15);
           button10.Visible = AlohaTSClass.CompEnable(20);
         */
            button10.Visible = AlohaTSClass.CompEnable(20);
            ctrlHotelBreakfastCount1.Visible = false;

        }

        string CurrentComment = "";
        CEmpl CurrentEmp = null;
        void ShowEndQuestion()
        {
            btnOk.Width = (this.Width - 6) / 2;
            btnOk.Visible = true;
            btnCancel.Text = "Отмена";
            pnlButtons.Visible = false;
            ctrlDegustationsManagers1.Visible = false;
            ctrlDegustationsWithKeys1.Visible = false;
            ctrlHotelBreakfastCount1.Visible = false;

            if (DType == 20)
            {
                lblEndQuestion.Text = "Наложить компенсацию " + Environment.NewLine + AlohaTSClass.GetCompName(DType) + " c комментарием " + Environment.NewLine + CurrentComment  + Environment.NewLine +
                     "Количество гостей: "+ ctrlHotelBreakfastCount1.GuestCount.ToString() + "?";
            }
            else
            {

                if (CurrentEmp == null)
                {
                    if (CurrentComment == "")
                    {
                        lblEndQuestion.Text = "Наложить компенсацию " + Environment.NewLine + AlohaTSClass.GetCompName(DType) + "?";
                    }
                    else
                    {
                        lblEndQuestion.Text = "Наложить компенсацию " + Environment.NewLine + AlohaTSClass.GetCompName(DType) + " c комментарием " + Environment.NewLine + CurrentComment + "?";
                    }
                }

                else
                {
                    if (CurrentComment == "")
                    {
                        lblEndQuestion.Text = "Наложить компенсацию " + Environment.NewLine + AlohaTSClass.GetCompName(DType) + " Сотрудник: " + Environment.NewLine +
                            CurrentEmp.Id.ToString() + " " + CurrentEmp.Name + "?";
                    }
                    else
                    {
                        lblEndQuestion.Text = "Наложить компенсацию " + Environment.NewLine + AlohaTSClass.GetCompName(DType) + " Сотрудник: " + Environment.NewLine +
                            CurrentEmp.Id.ToString() + " " + CurrentEmp.Name + " c комментарием " + Environment.NewLine + CurrentComment + "?"; 
                    }
                }
            }
            //lblEndQuestion.BringToFront();
            lblEndQuestion.Visible = true;
            
        }
        private void EndApplyComp()
        {
            AlohaTSClass.CheckWindow();
            string outMess = "";

            //int OrderMode = ((DType==20)?10:1);
            bool OrderRes = AlohaTSClass.OrderAllDishez(AlohaTSClass.AlohaCurentState.TerminalId, (int)AlohaTSClass.AlohaCurentState.CheckId, (int)AlohaTSClass.AlohaCurentState.TableId);
            if (!OrderRes)
            {
                frmAllertMessage Mf = new frmAllertMessage("Не могу заказать блюда. Попробуйте еще раз. Либо закажите самостоятельно. Если ошибка будет повторяться свяжитесь со службой техподдержки для перезагрузки Алохи.");
                Utils.ToLog("Не могу заказать блюда. Выхожу");
                Mf.ShowDialog();
                return;
            }


         int CompId=   AlohaTSClass.ApplyComp(DType, "", out outMess);

            if (AlohaTSClass.GetCurentCheckSumm() != 0)
            {
                AlohaTSClass.ShowMessage("Не смог наложить 100% компенсацию. Проверьте, что все блюда чека входят в группу соответствующую скидки " + DType.ToString());
                AlohaTSClass.DeleteComp(CompId);
                return;
            }
            

            if (outMess != "")
            {
                AlohaTSClass.ShowMessage(outMess);
            }
            else
            {
                if (CurrentEmp == null)
                {
                    if (CurrentComment != "")
                    {
                        AlohaTSClass.SetManagerDiscountAttr((int)AlohaTSClass.AlohaCurentState.CheckId, CurrentComment, AlohaTSClass.AlohaCurentState.EmployeeNumberCode);
                    }
                }
                else 
                {
                    if (CurrentComment != "")
                    {
                        AlohaTSClass.SetManagerDiscountAttr((int)AlohaTSClass.AlohaCurentState.CheckId, CurrentComment, CurrentEmp.Id);
                    }
                    else
                    {
                        AlohaTSClass.SetManagerDiscountAttr((int)AlohaTSClass.AlohaCurentState.CheckId, CurrentEmp.Name, CurrentEmp.Id);
                    }
                }

                if (DType == 20)
                {

                    for (int i = 0; i < ctrlHotelBreakfastCount1.GuestCount; i++)
                    {
                        double price = ctrlHotelBreakfastCount1.Price;
                        if (ctrlHotelBreakfastCount1.IsSmall)
                        {
                            AlohaTSClass.AddDishToCurentChk(iniFile.HotelBreakfastBarCodeSmall,price);
                        }
                        else
                        {
                            AlohaTSClass.AddDishToCurentChk(iniFile.HotelBreakfastBarCode,price);
                        }
                    }


                    
                //    AlohaTSClass.ApplyFullPaymentToCurentChk(iniFile.HotelBreakfastPaymentId);
              
                }
                if (DType != 20)
                {
                    AlohaTSClass.CloseCurentCheckAndTableByCurentUser();
                }
              
                AlohaTSClass.ShowMessage("Компенсация успешно применена");
            }
            this.Close();
        }

        void keyBoardControl1_BtnOkEvent(string Mess, object sender)
        {
            CurrentComment = Mess;
            ShowEndQuestion();
        }

        void keyBoardControl1_BtnCancelEvent(object sender)
        {
            this.Close(); 
        }

       


        private void DiscTypeSelector(int DiscId)
        {
            DType = DiscId;
            switch (DiscId)
            {
              case  11:
                    ShowManagersSelector();
                    break;
              case 12:
                    ShowKeyboard("Фамилия");
                    break;
              case 13:
                    ShowEndQuestion();
                    break;
              case 14:
                    ShowManagersSelector();
                    break;
              case 16:
                    ShowKeyboard("Опишите причину");
                    break;
              case 17:
                    ShowKeyboard("Фамилия виновного");
                    break;
              case 18:
                    ShowEndQuestion();
                    break;
              case 19:
                    ShowKeyboard("Опишите причину");
                    break;
              case 21:
                    ShowManagersSelector();
                    break;
              case 22:
                    ShowManagersSelector();
                    break;
              case 23:
                    ShowManagersSelector();
                    break;
              case 24:
                    ShowKeyboard("Укажите инициатора");
                    break;
              case 25:
                    ShowKeyboard("Комментарий");
                    //ShowEndQuestion();
                    break;
             case 26:
                    ShowKeyboard("Комментарий");
                    break;
             case 27:
                    ShowKeyboard("Комментарий");
                    break;
             default:
                    break;
            }
        
        }

        private void ShowManagersSelector()
        {
            ctrlDegustationsManagers1.FillManagerList();
            ctrlDegustationsManagers1.Location = new Point(3, 3);
            ctrlDegustationsManagers1.Size = new System.Drawing.Size(this.Width - 6, this.Height - 6);
            ctrlDegustationsManagers1.Visible = true;
            ctrlDegustationsManagers1.BringToFront();
            

        }
        private void ShowKeyboard (string Caption)
        {
            ctrlDegustationsWithKeys1.SetCaption(Caption);
            ctrlDegustationsWithKeys1.Location = new Point(3, 3);
            ctrlDegustationsWithKeys1.Size = new System.Drawing.Size(this.Width - 6, this.Height - 6);
            ctrlDegustationsWithKeys1.Visible = true;
            ctrlDegustationsWithKeys1.BringToFront();
        }
        

        internal void ManagersSelectOk(CEmpl Empl)
        {
            CurrentEmp = Empl;
            if (DType == 11)
            {
                ShowKeyboard("Наименование тренинга");
            }
            else if (DType == 23)
            {
                ShowKeyboard("Наименование конкурса");
            }
            else
            {
                ShowEndQuestion();
            }
        }
        internal void ManagersSelectCancel()
        {
            this.Close(); 
        }

        /*
        private void SetTrenning()
        {
            DType = 11;
            ctrlDegustationsWithKeys1.SetCaption("Ответственный за проведение");
            ShowKeyboard();            
 
        
        }
        */
        

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close(); 
        }


        public void KbCancel()
        {
            this.Close(); 
        }
        public void KbOk()
        { 
        }
        private int DType = 0;

        private void button2_Click(object sender, EventArgs e)
        {
            //SetTrenning();
            DiscTypeSelector(11);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DiscTypeSelector(12);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DiscTypeSelector(13);
        }
        /*
        private void ApplyComp(int Id)
        {
            AlohaTSClass.CheckWindow();
            string outMess = "";
            AlohaTSClass.ApplyComp(Id, out outMess);


            this.Close();
            if (outMess != "")
            {
                AlohaTSClass.ShowMessage(outMess);
            }
        }
        */
        private void button5_Click(object sender, EventArgs e)
        {
            DiscTypeSelector(14);
        }
        private void button8_Click(object sender, EventArgs e)
        {
            DiscTypeSelector(16);
        }
        private void button6_Click(object sender, EventArgs e)
        {
            DiscTypeSelector(17);
            
            /*
            DType = 17;
            ctrlDegustationsWithKeys1.SetCaption("Ответственный");
            ShowKeyboard();
             * */
        }


        private void button7_Click(object sender, EventArgs e)
        {
            DiscTypeSelector(19);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            DiscTypeSelector(18);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            DiscTypeSelector(21);
        }

        private void btn22_Click(object sender, EventArgs e)
        {
            DiscTypeSelector(22);
        }
        private void btn23_Click(object sender, EventArgs e)
        {
            DiscTypeSelector(23);
        }
        
        private void button1_Click_2(object sender, EventArgs e)
        {
            DiscTypeSelector(24);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            EndApplyComp();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close(); 
        }

        
       

        private void button10_Click(object sender, EventArgs e)
        {
            //Завтраки
            DType = 20;
            ctrlDegustationsWithKeys1.Location = new Point(3, 203);
            ctrlDegustationsWithKeys1.Size = new System.Drawing.Size(this.Width - 6, this.Height - 206);
            ctrlDegustationsWithKeys1.Visible = true;
            ctrlDegustationsWithKeys1.BringToFront();
            ctrlDegustationsWithKeys1.SetCaption("Комментарий");

            ctrlHotelBreakfastCount1.GuestCount = 1;
            ctrlHotelBreakfastCount1.Visible = true;
            ctrlHotelBreakfastCount1.BringToFront();
            ctrlHotelBreakfastCount1.Location = new Point(3, 2);
            ctrlHotelBreakfastCount1.Size = new System.Drawing.Size(this.Width - 6, 205);
           // ShowEndQuestion();

            

        }

        private void btn25_Click(object sender, EventArgs e)
        {
            DiscTypeSelector(25);
        }

        private void button1_Click_3(object sender, EventArgs e)
        {
            DiscTypeSelector(26);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            DiscTypeSelector(27);
        }
    }
    
}
