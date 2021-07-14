using  AlgoTrade.Common.Entities;
using Microsoft.Practices.Prism.Events;

namespace AlgoTradeClient.Infrastructure
{
   public class SystemShutdownOperation
   {
       public static IEventAggregator eventAggregator = new EventAggregator();
   }
}
