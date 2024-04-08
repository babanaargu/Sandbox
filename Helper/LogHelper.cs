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
        //public static void AddLog(string message, ObservableCollection<GeneralConfigurationModel> lstGeneralConfigurationModelCollection)
        //{
        //    lstGeneralConfigurationModelCollection.Add(message);

        //}

        public ObservableCollection<string> Messages { get; } = new ObservableCollection<string>();

        public void Log(string message)
        {
            Messages.Insert(0,$"{DateTime.Now}: {message}");
            RemoveLast();
        }

        public void Clear()
        {
            Messages.Clear();
        }
        public void RemoveLast()
        {
            if (Messages.Count > 100)
            {
                Messages.RemoveAt(Messages.Count - 1);
            }
        }
    }
}
