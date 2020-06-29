using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;


namespace PDiscountCard
{

    public class CCommandQuere
    {
        public CCommandQuere(string CommandStr)
        {
            _Id = new Guid();
            Command = CommandStr;
        }
        Guid _Id;
        public Guid Id
        {
            get
            {
                return _Id;
            }
        }
        public List<CProp> PropertysList = new List<CProp>();
        public string Command="";
        public string ResultName = "";
    }
    public class CCommandQuereBlock
    {
        public CCommandQuereBlock()
        {
            _Id = new Guid();
        }
        public List<CCommandQuere> CommandList = new List<CCommandQuere>();
        Guid _Id;
        public Guid Id
        {
            get
            {
                return _Id;
            }
        }
        
    }
    public struct  CProp
    {

        public object PropVal ;
        public string PropName;
    }
}
