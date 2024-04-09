using AutoShare.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoShare.Helper
{
    public  class LogHelper
    {
        public ObservableCollection<string> Messages { get; } = new ObservableCollection<string>();

        public void Log(string message)
        {
            Messages.Insert(0,$"{DateTime.Now}: {message}");
            RemoveIfMoreThan100();
        }
        public void RemoveIfMoreThan100()
        {
            if (Messages.Count > 100)
            {
                Messages.RemoveAt(Messages.Count - 1);
            }
        }
    }
}
