using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDiscountCard.GuestCount
{
    public static class GuestCount
    {
        public static void SetGuestCount(int checkId)
        {
            if (!AlohaTSClass.IsAlohaTS()) return;
            if (!iniFile.AskGuestCountOnPreCheck) return;

            var chk = AlohaTSClass.GetCheckByIdShort(checkId);
            int tId = chk.TableNumber;
            if (tId >= 146 && tId <= 255) return;
            if (chk.Summ <= 0) return;
            try
            {
                Utils.ToCardLog("SetGuestCount ");
                var ctrl = new CtrlGuestCountAsk();
                var mW = PDSystem.ModalWindowsForegraund.GetModalWindow(ctrl);
                ctrl.SetOwnerWnd(mW);
                mW.ShowDialog();

            }
            catch(Exception e)
            {
                Utils.ToCardLog("Error SetGuestCount " + e.Message);
            }
            

            //wnd.ShowDialog();


        }
    }
}
