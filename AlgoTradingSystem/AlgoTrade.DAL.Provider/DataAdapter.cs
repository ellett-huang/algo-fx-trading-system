using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlgoTrade.Common.Exceptions;

namespace AlgoTrade.DAL.Provider
{
    public class DataAdapter : AlgoTrade.DAL.Provider.IDataAdapter,IDisposable
    {
        public void LogData(Log LogEntity)
        {
            try
            {
                using (var dataContext=new AlgoTradeContainer())
                {
                    lock (this)
                    {
                        dataContext.Logs.Add(LogEntity);
                        dataContext.SaveChanges();
                    }                }
            }
            catch (Exception er)
            {
                throw new DALException(er.Message);
            }
        }

        public List<Log> LookupLogs_All(int PageNumber, int PageSize)
        {
            try
            {
                using (var dataContext = new AlgoTradeContainer())
                {
                    return (from t in dataContext.Logs
                            orderby t.CreateDate descending
                            select t).Skip((PageNumber ) * PageSize).Take(PageSize).ToList();
                }
            }
            catch (Exception er)
            {
                
                throw new DALException(er.Message);
            }
        }

        public void SaveTransaction(Transaction transactionData)
        {
            try
            {
                using (var dataContext = new AlgoTradeContainer())
                {
                    lock (this)
                    {
                        var resultlist = dataContext.EnumTypes.Where(x => x.EnumName == transactionData.PositionTypeName).ToList();
                        if (resultlist.Count > 0)
                        {
                            transactionData.PositionTypeID = resultlist[0].EnumValue;
                            transactionData.ActionTypeID = dataContext.EnumTypes.Where(x => x.EnumName == transactionData.ActionTypeName).ToList()[0].EnumValue;
                        }
                        dataContext.Transactions.Add(transactionData);
                        dataContext.SaveChanges();
                    }
                }
            }
            catch (Exception er)
            {
                throw new DALException(er.Message);
            }
        }

        public void AddPosition(Position PositionData)
        {
            try
            {
                using (var dataContext = new AlgoTradeContainer())
                {
                    lock (this)
                    {
                        var result = dataContext.EnumTypes.First(x => x.EnumName == PositionData.PositionTypeName);
                          if (result != null)
                          {
                              PositionData.PositionTypeID = result.EnumValue;
                              dataContext.Positions.Add(PositionData);
                              dataContext.SaveChanges();
                          }
                    }
                }
            }
            catch (Exception er)
            {
                throw new DALException(er.Message);
            }
        }
        public void RemovePosition(string Symbol)
        {
            try
            {
                using (var dataContext = new AlgoTradeContainer())
                {
                    lock (this)
                    {
                        Position deleteObject = dataContext.Positions.First(t => t.Symbol == Symbol);
                        dataContext.Positions.Remove(deleteObject);
                        dataContext.SaveChanges();
                    }
                }
            }
            catch (Exception er)
            {
                throw new DALException(er.Message);
            }
        }

        public List<Position> LookupPositions_All(int PageNumber, int PageSize)
        {
            try
            {
                using (var dataContext = new AlgoTradeContainer())
                {
                    return (from t in dataContext.Positions
                            orderby t.PositionTypeName
                            select t).Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
                }
            }
            catch (Exception er)
            {
                throw new DALException(er.Message);
            }
        }

        public List<Position> LookupPositions_All(string Symbol)
        {
            try
            {
                using (var dataContext = new AlgoTradeContainer())
                {
                    return (from t in dataContext.Positions
                            where t.Symbol==Symbol
                            orderby t.PositionTypeName
                            select t).ToList();
                }
            }
            catch (Exception er)
            {
                throw new DALException(er.Message);
            }
        }

        public List<Transaction> LookupLatestTransactions(DateTime LastDate, string Symbol)
        {
            try
            {
                using (var dataContext = new AlgoTradeContainer())
                {
                    return (from t in dataContext.Transactions
                            where t.TradingDate > LastDate && t.Symbol == Symbol
                            orderby t.TradingDate descending
                            select t).ToList();
                }
            }
            catch (Exception er)
            {
                throw new DALException(er.Message);
            }
        }
        
        public List<Transaction> LookupTransactions_All(int PageNumber, int PageSize)
        {
            try
            {
                using (var dataContext = new AlgoTradeContainer())
                {
                    return (from t in dataContext.Transactions
                            orderby t.TradingDate descending
                            select t).Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
                }
            }
            catch (Exception er)
            {
                throw new DALException(er.Message);
            }
        }

        public List<SymbolList> LookupSymbols_All()
        {
            try
            {
                using (var dataContext = new AlgoTradeContainer())
                {
                    return (from t in dataContext.SymbolLists
                            select t).ToList();
                }
            }
            catch (Exception er)
            {
                throw new DALException(er.Message);
            }
        }

        public List<AlgoRule> LookupAlgoRules_All()
        {
            try
            {
                using (var dataContext = new AlgoTradeContainer())
                {
                    return (from t in dataContext.AlgoRules
                            where t.IsEnable == true orderby t.ID 
                            select t).ToList();
                }
            }
            catch (Exception er)
            {
                throw new DALException(er.Message);
            }
        }

        public List<RiskRule> LookupAlgoRiskRules_All()
        {
            try
            {
                using (var dataContext = new AlgoTradeContainer())
                {
                    return (from t in dataContext.RiskRules
                            select t).ToList();
                }
            }
            catch (Exception er)
            {
                throw new DALException(er.Message);
            }
        }


        public void SaveAlgoRule(AlgoRule algoRuleData)
        {
            try
            {
                using (var dataContext = new AlgoTradeContainer())
                {
                    lock (this)
                    {
                        algoRuleData.PositionTypeValue = dataContext.EnumTypes.Where(x => x.EnumName == algoRuleData.PositionTypeName).ToList()[0].EnumValue;
                        if (String.IsNullOrEmpty(algoRuleData.ActionTypeName))
                            algoRuleData.ActionTypeValue =0;
                        else
                            algoRuleData.ActionTypeValue = dataContext.EnumTypes.Where(x => x.EnumName == algoRuleData.ActionTypeName).ToList()[0].EnumValue;
                        dataContext.AlgoRules.Attach(algoRuleData);
                        dataContext.Entry(algoRuleData).State = System.Data.EntityState.Modified;
                        dataContext.SaveChanges();
                    }
                }
            }
            catch (Exception er)
            {
                throw new DALException(er.Message);
            }
        }

      
         #region IDisposable

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

     

        #endregion

    }
}
