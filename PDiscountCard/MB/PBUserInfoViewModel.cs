using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;

namespace PDiscountCard.MB
{
    class PBUserInfoViewModel : INotifyPropertyChanged
    {
        private Window OwnerWnd;
        public string FullName { get; set; } = string.Empty;
        public bool BlockedFlag { get; set; } = false;
        public string PhoneStr { get; set; } = string.Empty;
        public string LastCardNumStr { get; set; } = string.Empty;
        /// <summary>
        /// "yyyy-MM-dd"
        /// </summary>              
        public string BirthDateStr { get; set; } = string.Empty;      
        public decimal SaleSumm { get; set; }

        public string Gender { get; set; } = string.Empty;
        /// <summary>
        /// НЕ УНИКАЛЕН!!!
        /// </summary>       
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// UUID 
        /// </summary>      
        public string GroupId { get; set; } = string.Empty;
       public string GroupName { get; set; } = string.Empty;

        private int writeOfBalance { get; set; }
        public int WriteOfBalance 
        {
            get { return writeOfBalance; }
            set
            {
                writeOfBalance = value;
                OnPropertyChanged("WriteOfBalance");
            }
        }

        public decimal Balance { get; set; }

        
        public decimal BalanceAccumulated { get; set; }


        public decimal BalancePresent { get; set; }

        public decimal BalanceAction { get; set; }

        private ICommand _commandCancel;
        public ICommand CommandCancel => _commandCancel ?? ( _commandCancel = new RelayCommand( rc => { CloseAction(); }, _ => { return !(CloseAction == null || BlockedFlag); }));
        public Action CloseAction { get; set; }

        private ICommand _commandRemoveGuest;
        public ICommand CommandRemoveGuest => _commandRemoveGuest ?? (_commandRemoveGuest = new RelayCommand(rc => { RemoveGuestAction(); }, _ => { return !(RemoveGuestAction == null ); }));
        public Action RemoveGuestAction { get; set; }

        private ICommand _commandAddPoint;
        public ICommand CommandAddPoint => _commandAddPoint ?? (_commandAddPoint = new RelayCommand(rc => { AddPointAction();}, _ => { return !(AddPointAction == null || BlockedFlag); }));
        public Action AddPointAction { get; set; }

        private ICommand _commandWriteOffPoint;
        public ICommand CommandWriteOffPoint => _commandWriteOffPoint ?? (_commandWriteOffPoint = new RelayCommand(rc => { WriteOffPointAction(); }, _ => { return !(WriteOffPointAction == null || BlockedFlag); }));
        public Action WriteOffPointAction { get; set; }

        private ICommand _commandGiveGift;
        public ICommand CommandGiveGift => _commandGiveGift ?? (_commandGiveGift = new RelayCommand(rc => { GiveGiftAction(); }, _ => { return !(GiveGiftAction == null || BlockedFlag); }));
        public Action GiveGiftAction { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));                            
        }

        public PBUserInfoViewModel(MBProxi.PBCustomerInfo customer)
        {
            Fill(customer);
        }


        private void Fill(MBProxi.PBCustomerInfo customer)
        {
            FullName = customer.FullName ?? "не указано";
            BlockedFlag = customer.BlockedFlag;
            PhoneStr = customer.PhoneStr ?? "не указано";

            /// <summary>
            /// "yyyy-MM-dd"
            /// </summary>
            
            if(string.IsNullOrWhiteSpace(customer.BirthDate))
            {
                BirthDateStr = "не указано";
            }
            else
            {
                BirthDateStr = DateTime.TryParseExact(customer.BirthDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime bDate) ? bDate.ToString("dd/MM/yyyy") : "не указано";
            }

            Gender = string.IsNullOrWhiteSpace(customer.Gender)? "не указано":GetGender(customer.Gender);

            /// <summary>
            /// НЕ УНИКАЛЕН!!!
            /// </summary>       
            Email = customer.Email ?? "не указано";

            /// <summary>
            /// UUID 
            /// </summary>      
            GroupId = customer.GroupId ?? string.Empty;
            GroupName = customer.GroupName ?? string.Empty;

            Balance = customer.Balance;
            BalanceAccumulated = customer.BalanceAccumulated;
            BalancePresent = customer.BalancePresent;
            BalanceAction = customer.BalanceAction;
            LastCardNumStr = GetFriendlyCardNum(customer.LastCardNumber);
        }

        internal void SetOwnerWnd(Window _OwnerWnd)
        {
            OwnerWnd = _OwnerWnd;
        }

        internal void SetChkSumm(decimal summ)
        {
            SaleSumm = summ;
        }

        internal string GetGender(string engGender)
        {
            string result = string.Empty;

            switch(engGender)
            {
                case "male":
                    result = "муж.";
                    break;

                case "female":
                    result = "жен.";
                    break;

                default:
                    result = "не указано";
                    break;
            }

            return result;
        }


        private string GetFriendlyCardNum(string numStr)
        {
            string result = string.Empty;
            
            if(!string.IsNullOrWhiteSpace(numStr))
            {
                if (numStr.Length > 10)
                {
                    switch (numStr.Substring(0, 5))
                    {
                        case "90658":
                            result = "Zav15 " + numStr.Substring(5).TrimStart('0');
                            break;
                        case "80826":
                            result = "Zav20 " + numStr.Substring(5).TrimStart('0');
                            break;
                        case "80827":
                            result = "Vip50 " + numStr.Substring(5).TrimStart('0');
                            break;
                        case "80828":
                            result = "Zav30 " + numStr.Substring(5).TrimStart('0');
                            break;
                        case "80830":
                            result = "ToGo10 " + numStr.Substring(5).TrimStart('0');
                            break;
                        case "80840":
                            result = "Hello10 " + numStr.Substring(5).TrimStart('0');
                            break;
                        case "20189":
                            result = "Sber10 " + numStr.Substring(numStr.Length - 6);
                            break;
                        case "20180":
                            result = "Sber10 " + numStr.Substring(numStr.Length - 6);
                            break;
                        case "20187":
                            result = "Sber10 " + numStr.Substring(numStr.Length - 6);
                            break;
                        case "86738":
                            result = "Pre " + numStr.Substring(5).TrimStart('0'); ;
                            break;

                        default:
                            result = numStr;
                            break;
                    }
                }
                else
                {
                    result = numStr;
                }
            }
            
            return result;
        }

    }
}
