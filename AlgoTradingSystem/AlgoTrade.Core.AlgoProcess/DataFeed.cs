using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlgoTrade.Common.Entities;

namespace AlgoTrade.Core.AlgoProcess
{

    public class DataFeed : IDataFeed
    {
        private string userName;
        private string password;
        private string theAPIAddress;
        private string providerName;
        private int userID;

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }


        public string Password
        {
            get { return password; }
            set { password = value; }
        }


        public string APIAddress
        {
            get { return theAPIAddress; }
            set { theAPIAddress = value; }
        }


        public string ProviderName
        {
            get { return providerName; }
            set { providerName = value; }
        }


        public int UserID
        {
            get { return userID; }
            set { userID = value; }
        }

        public DataFeed(string userName,string password,string APIAddress, string ProvideerName, int UserID)
        {
            this.userName = userName;
            this.password = password;
            this.theAPIAddress = APIAddress;
            this.ProviderName = ProviderName;
            this.UserID = UserID;
        }

        public TradingData RetrieveData(string Symbol, int Interval)
        {
            throw new System.NotImplementedException();
        }

        public void Connect()
        {
            throw new System.NotImplementedException();
        }

        public void Disconnect()
        {
            throw new System.NotImplementedException();
        }

    }
}

