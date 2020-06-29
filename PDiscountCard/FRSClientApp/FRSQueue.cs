using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace PDiscountCard.FRSClientApp
{
    static  class FRSQueue
    {
        static internal Thread FRSQueueThread;
        internal static void StartFRSQueueThread()
        {
            FRSQueueThread = new Thread(StartFRSQueue);
            FRSQueueThread.SetApartmentState(ApartmentState.STA);
            FRSQueueThread.Start();
        }

        internal static void StopFRSQueueThread()
        {
            mExitFRSThread = true;
        }


        private static bool mExitFRSThread = false;


        internal static bool TmpChecksWithBDExist(DateTime BD)
        {
            DirectoryInfo Di = new DirectoryInfo(PDiscountCard.CloseCheck.ChecksPath);
            
            foreach (FileInfo fi in Di.GetFiles("*.5"))
            {
                try
                {
                    Check Ch = PDiscountCard.CloseCheck.ReadCheckFromTmp(fi.FullName);
                    if (Ch.BusinessDate == BD) return true;
                }
                catch (Exception e)
                {
                    Utils.ToCardLog("Error TmpChecksWithBDExist " + e.Message);
                }
            }
            return false;
        }

        private static void StartFRSQueue()
        {
            //FRSClient.Init();
            Utils.ToCardLog("StartFRSQueue " );
            DirectoryInfo Di = new DirectoryInfo(PDiscountCard.CloseCheck.ChecksPath);
            DirectoryInfo DiBugs = new DirectoryInfo(PDiscountCard.CloseCheck.ChecksPath+@"\bugs");

            if (!Di.Exists) Di.Create();
            List<DateTime> ZReps = new List<DateTime>();
            while (!mExitFRSThread)
            {
                Thread.Sleep(5000);
                ZReps.Clear();
                foreach (FileInfo fi in Di.GetFiles("*.zrep"))
                {
                    try
                    {
                        
                        int d = Convert.ToInt32(fi.Name.Substring(4, 2));
                        int m = Convert.ToInt32(fi.Name.Substring(6, 2));
                        int y = Convert.ToInt32(fi.Name.Substring(8, 4));
                        DateTime bd = new DateTime(y, m, d);
                        Utils.ToCardLog("Find ZRepFile "+bd.ToString());
                        if (ZReps.Contains(bd)) continue;
                        if (TmpChecksWithBDExist(bd))
                        {
                            Utils.ToCardLog("Find ZRepFile checks exist" );
                            continue;
                        }
                        fi.Delete();
                        ZReps.Add(bd);
                        FRSClient.ZReport(bd);
                    }
                    catch(Exception e)
                    {
                        Utils.ToCardLog("Error find ZRepFile "+e.Message);
                    }
                }

                foreach (FileInfo fi in Di.GetFiles("*.5").OrderBy(a=>a.CreationTime))
                {
                    try
                    {

                        Utils.ToCardLog("FRSQueue Read file "+fi.FullName);
                        Check Ch = PDiscountCard.CloseCheck.ReadCheckFromTmp(fi.FullName);
                        if (Ch == null)
                        { 
                            //Di
                            if (!DiBugs.Exists)
                            {
                                DiBugs.Create();
                            }
                            fi.MoveTo(DiBugs.FullName + @"\" + fi.Name);
                            continue;
                        }
                        Utils.ToCardLog("FRSQueue send");
                        FRSSrv.AddCheckResponce res= FRSClient.SendAlohaChk(Ch);
                        if (res == null) { break; }
                        if (res.Check.NeedUpdateItems)
                        {
                            Utils.ToCardLog("NeedUpdateItems");

                            FRSClient.UpdateItems();
                        }

                        if ((res.ChkAlreadyExistInSQL) && (res.Check != null) && (res.Check.Sucсess))
                        {
                        
                        }

                        if (res.SQLAddErrorException != null && res.SQLAddErrorException != "")
                        {
                            Utils.ToCardLog("StartFRSQueue res.SQLAddErrorException: " + res.SQLAddErrorException);
                            continue;
                        }
                        fi.Delete();
                        res = null;
                        Utils.ToCardLog("FRSQueue file complited " + fi.FullName);
                        

                    }
                    catch(Exception e)
                    {
                        Utils.ToCardLog("Error StartFRSQueue " + e.Message);
                        break;
                    }
                }
            }
        }

    }
}
