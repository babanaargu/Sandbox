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

		private int _stochInput=14;

		public int StochInput
        {
			get { return _stochInput; }
			set {SetProperty(ref _stochInput , value); }
		}

        private string _StartStopSearching="Start Searching";

        public string StartStopSearching
        {
            get { return _StartStopSearching; }
            set { SetProperty(ref _StartStopSearching, value); }
        }

        private string _StartStopSearchingBackgroundColor = "Green";

        public string StartStopSearchingBackgroundColor
        {
            get { return _StartStopSearchingBackgroundColor; }
            set { SetProperty(ref _StartStopSearchingBackgroundColor, value); }
        }

    }
}
