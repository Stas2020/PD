using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDiscountCard.AlohaExternal
{
    public  class AlohaMnuExt
    {
        public int DepId { set; get; }
        public  List<AlohaSubMnuExt> SubMnus { set; get; }
    }
    public class AlohaSubMnuExt
    {
        public int Id { set; get; }
        public string LongName { set; get; }
        public string EngName { set; get; }
        public List<AlohaItemExt> Items { set; get; }
    }
    public class AlohaItemExt
    {
        public int Id { set; get; }
        public string LongName { set; get; }
        public string EngName { set; get; }
        public int Price { set; get; }
        public List<AlohaModGroupeExt> ModGroups { set; get; }
    }
    public class AlohaModGroupeExt
    {
        public int Id { set; get; }
        public string LongName { set; get; }
        public string EngName { set; get; }
        public List<AlohaModExt> Mods { set; get; }
    }
    public class AlohaModExt
    {
        public int Id { set; get; }
        public string LongName { set; get; }
        public string EngName { set; get; }
        public int Price { set; get; }
        public int Min { set; get; }
        public int Max { set; get; }
        public int MaxFree { set; get; }
    }
}
