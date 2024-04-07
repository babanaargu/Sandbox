using AutoShare.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoShare.Model
{
    public class ImportFileModel : BindableBase
    {
        public string stockUrl { get; set; }
        public string stockCodeName { get; set;}

    }
}
