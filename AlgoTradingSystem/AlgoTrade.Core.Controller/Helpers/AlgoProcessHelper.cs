using AlgoTrade.Common.Entities;
using AlgoTrade.DAL.Provider;
using AlgoTradeLib.Algo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using AlgoTrade.Common.Log;

namespace AlgoTrade.Core.Dispatcher.Helpers
{
    class AlgoProcessHelper
    {

        /// <summary>
        /// Serialize Live Data list to File
        /// </summary>
        /// <param name="LiveData"></param>
        /// <param name="Symbol"></param>
        public static void SaveLiveDataToFile(List<TradingData> LiveData,string Symbol)
        {
            string dir = @".\LiveData";
            try
            {
                Directory.CreateDirectory(dir);
            }
            catch (Exception ex)
            {
                LoggerManager.LogError(ex.Message);
                throw;
            }
            string serializationFile = Path.Combine(dir, "LiveData"+Symbol+".bin");

            //serialize
            using (Stream stream = File.Open(serializationFile, FileMode.Create))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                bformatter.Serialize(stream, LiveData);
                stream.Close();
            }

        }

        /// <summary>
        /// Deserialize Live Data From File
        /// </summary>
        /// <param name="Symbol"></param>
        public static List<TradingData> LoadLiveDataFromFile( string Symbol)
        {
            string dir = @".\LiveData";
            List<TradingData> LiveData = new List<TradingData>();
            try
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                    return LiveData;
                }
            }
            catch (Exception ex)
            {
                LoggerManager.LogError(ex.Message);
                throw;
            }
            string serializationFile = Path.Combine(dir, "LiveData" + Symbol + ".bin");

            //deserialize
            if (!File.Exists(serializationFile))
                return LiveData;
            using (Stream stream = File.Open(serializationFile, FileMode.Open))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                LiveData = (List<TradingData>)bformatter.Deserialize(stream);
                stream.Close();
            }
            return LiveData;

        }


    }
}