using System;
namespace AlgoTrade.DAL.Provider
{
    interface IDataAdapter 
    {
        void LogData(Log LogEntity);
        System.Collections.Generic.List<RiskRule> LookupAlgoRiskRules_All();
        System.Collections.Generic.List<AlgoRule> LookupAlgoRules_All();
        System.Collections.Generic.List<Log> LookupLogs_All(int PageNumber, int PageSize);
        System.Collections.Generic.List<SymbolList> LookupSymbols_All();
        System.Collections.Generic.List<Transaction> LookupTransactions_All(int PageNumber, int PageSize);
        void SaveAlgoRule(AlgoRule algoRuleData);
        void SaveTransaction(Transaction transactionData);
    }
}
