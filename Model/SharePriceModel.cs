using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoShare.Model
{
    public class SharePriceModel
    {
        public long epochDateTime { get; set; }
        public float open { get; set; }
        public float close { get; set; }
        public float high { get; set; }
        public float low { get; set; }

    }
}
