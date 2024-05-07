using AutoShare.ApiResponseHandler;
using AutoShare.Commands;
using AutoShare.Helper;
using AutoShare.Model;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace AutoShare.ViewModel
{
    public class MainViewModel : BindableBase
    {
        public BaseCommand StartCommand { get; }
        public BaseCommand ImportCommand { get; }
        public BaseCommand DeleteCommand { get; }

        public SharePriceModel _sharepriceModel;
        public List<ImportFileModel> _importStockList;
        ImportFileModel importfilemodel;
        MainModel mainmodel;

        private LogHelper _logger = new LogHelper();

        public LogHelper Logger
        {
            get { return _logger; }
            set { SetProperty(ref _logger, value); }
        }


        private GeneralConfigurationModel _generalConfigurationModel;

        public GeneralConfigurationModel GeneralConfigurationModel
        {
            get { return _generalConfigurationModel; }
            set { _generalConfigurationModel = value; }
        }


        private ObservableCollection<MainModel> _stockLstObservableCollection = new ObservableCollection<MainModel>();

        public ObservableCollection<MainModel> StockLstObservableCollection
        {
            get { return _stockLstObservableCollection; }
            set { _stockLstObservableCollection = value; }
        }

        private ObservableCollection<GeneralConfigurationModel> _lstGeneralConfigurationModelCollection = new ObservableCollection<GeneralConfigurationModel>();

        public ObservableCollection<GeneralConfigurationModel> LstGeneralConfigurationModelCollection
        {
            get { return _lstGeneralConfigurationModelCollection; }
            set { _lstGeneralConfigurationModelCollection = value; }
        }

        public MainViewModel()
        {
            StartCommand = new BaseCommand(ExecuteStartCommand);
            ImportCommand = new BaseCommand(ExecuteImportCommand);
            DeleteCommand = new BaseCommand(ExecuteDeleteCommand);
            _sharepriceModel = new SharePriceModel();
            _importStockList = new List<ImportFileModel>();
            importfilemodel = new ImportFileModel();
            mainmodel = new MainModel();
            _generalConfigurationModel = new GeneralConfigurationModel();

        }

        private void ExecuteDeleteCommand(object obj)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete all stocks?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            // If user clicks Yes, remove the item from the collection
            if (result == MessageBoxResult.Yes)
            {
                if (StockLstObservableCollection.Count >= 1)
                {
                    StockLstObservableCollection.Clear();
                    Logger.Log("Successfully deleted all stocks");
                }
            }
            
        }

        private async void ExecuteImportCommand(object obj)
        {
            string filePath = null;

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Open File",
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            // Show the dialog and capture the result
            bool? result = openFileDialog.ShowDialog();

            // Process the result
            if (result == true)
            {
                // The user selected a file
                filePath = openFileDialog.FileName;

            }
            else if (result == false)
            {
                // The user cancelled or closed the dialog
            }

            GeneralConfigurationModel.FilePath = filePath;
            Logger.Log("Started Importing. Please Wait....");

            // Check if the file exists
            if (File.Exists(filePath))
            {
                // Open the file for reading
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;

                    // Read and display lines from the file until the end of the file is reached
                    while ((line = sr.ReadLine()) != null)
                    {
                        importfilemodel = new ImportFileModel();
                        ApiResponse apiResponse = new ApiResponse();
                        var response = await apiResponse.GetStockCodeName(line);
                        importfilemodel.stockCodeName = JsonConvert.DeserializeObject<string>("\"" + ExtractData.GetBetween(response, "nseScriptCode\":\"", "\",\"") + "\"");
                        

                        importfilemodel.stockUrl = line;
                        _importStockList.Add(importfilemodel);
                        Logger.Log($"Stock Imported Successfully {importfilemodel.stockUrl}");
                    }
                }
                Logger.Log("Importing Completed....");

            }
        }



        /// <summary>
        /// Method use to get the Apis details
        /// </summary>
        /// <param name="obj"></param>
        private async void ExecuteStartCommand(object obj)
        {
            ApiResponse apiResponse;
            try
            {

                if(GeneralConfigurationModel.StartStopSearching== "Start Searching")
                {
                    if (_importStockList.Count <= 0)
                    {
                        Logger.Log("No data found! Please import");
                        return;
                    }
                    Constant.IsSearchingStopped = false;
                    GeneralConfigurationModel.StartStopSearching = "Stop Searching";
                    GeneralConfigurationModel.StartStopSearchingBackgroundColor = "red";

                    foreach (var stockdetail in _importStockList)
                    {
                        apiResponse = new ApiResponse();
                        mainmodel = new MainModel();

                        if (Constant.IsSearchingStopped)
                            return;

                        var result = await apiResponse.GetApi(stockdetail, GeneralConfigurationModel.StockTimeInMinute);
                        if (string.IsNullOrEmpty(result))
                        {
                            Logger.Log($"Data not found for {stockdetail.stockCodeName}");
                            continue;
                        }
                        else
                        {
                            JObject jsonObject = JObject.Parse(result);
                            var jsonList = jsonObject["candles"];

                            JArray paramsArray = (JArray)jsonObject["candles"];
                            List<SharePriceModel> listName = new List<SharePriceModel>();

                            foreach (JToken param in jsonList.Reverse().Take(GeneralConfigurationModel.StochInput))
                            {
                                _sharepriceModel = new SharePriceModel();
                                _sharepriceModel.epochDateTime = (long)param[0];
                                _sharepriceModel.open = (float)param[1];
                                _sharepriceModel.high = (float)param[2];
                                _sharepriceModel.low = (float)param[3];
                                _sharepriceModel.close = (float)param[4];
                                listName.Add(_sharepriceModel);

                            }
                            mainmodel.StockName = stockdetail.stockCodeName;
                            mainmodel.StochasticResult = getstochasticResult(listName);
                            if (mainmodel.StochasticResult <= 20)
                            {
                                mainmodel.StockBuySell = "BUY";
                                mainmodel.StockBuySellBackgroundColor = "Green";
                                StockLstObservableCollection.Add(mainmodel);
                                mainmodel.SerialNumber = StockLstObservableCollection.Count;
                                Logger.Log($"Successfully fetched details for {mainmodel.StockName}");
                            }
                            else if (mainmodel.StochasticResult >= 80)
                            {
                                mainmodel.StockBuySell = "SELL";
                                mainmodel.StockBuySellBackgroundColor = "Red";
                                StockLstObservableCollection.Add(mainmodel);
                                mainmodel.SerialNumber = StockLstObservableCollection.Count;
                                Logger.Log($"Successfully fetched details for {mainmodel.StockName}");
                            }
                            else
                            {
                                Logger.Log($"Skipping {mainmodel.StockName} stock because result is={mainmodel.StochasticResult}");
                            }
                            await Task.Delay(5000);
                        }
                    }
                    _importStockList.Clear();
                    Logger.Log("Successfully completed searching");
                    GeneralConfigurationModel.StartStopSearching = "Start Searching";
                    GeneralConfigurationModel.StartStopSearchingBackgroundColor = "green";
                }
                else
                {
                    Constant.IsSearchingStopped = true;
                    GeneralConfigurationModel.StartStopSearching = "Start Searching";
                    GeneralConfigurationModel.StartStopSearchingBackgroundColor = "green";
                    Logger.Log("Successfully Stopped searching");
                }

                
            }
            catch (Exception ex)
            {
                Logger.Log("Exception =" + ex.Message);
            }
        }

        private float getstochasticResult(List<SharePriceModel> listName)
        {
            //Formula for the Stochastic Oscillator

            //% K = %K=(C−L14/H14−L14)×100
            //where:
            // C = The most recent closing price
            //L14 = The lowest price traded of the 14 previous
            //trading sessions
            //H14 = The highest price traded during the same
            //14 - day period
            //% K = The current value of the stochastic indicator
            float L14 = getL14Value(listName);
            float H14 = getH14Value(listName);

            float k = ((listName[0].close - L14) / (H14 - L14)) * 100;
            return k;
        }

        //get last 14days lowest value
        private float getL14Value(List<SharePriceModel> listName)
        {
            float min = listName[0].low;

            for (int i = 0; i < listName.Count; i++)
            {
                if (listName[i].low < min)
                {
                    min = listName[i].low;
                }
            }
            return min;
        }

        //get last 14days highest value
        private float getH14Value(List<SharePriceModel> listName)
        {
            float max = 0;

            for (int i = 0; i < listName.Count; i++)
            {
                if (listName[i].high > max)
                {
                    max = listName[i].high;
                }
            }

            return max;
        }
    }

}
