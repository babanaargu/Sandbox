using AutoShare.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoShare.Model
{
    public class MainModel : BindableBase
    {
		private string _stockName;

		public string StockName
		{
			get { return _stockName; }
			set { _stockName = value; }
		}


		private float _stochasticResult;

		public float StochasticResult
        {
			get { return _stochasticResult; }
			set { _stochasticResult = value; }
		}


	}
}
