using AutoShare.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoShare.Model
{
    public class GeneralConfigurationModel : BindableBase
    {
		private string _logMessage;

		public string LogMessage
		{
			get { return _logMessage; }
			set { _logMessage = value; }
		}
		private string _filePath;

		public string FilePath
		{
			get { return _filePath; }
			set {SetProperty(ref _filePath , value); }
		}


	}
}
