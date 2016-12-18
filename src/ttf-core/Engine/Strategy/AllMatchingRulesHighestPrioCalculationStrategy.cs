using System.Collections.Generic;
using System.Linq;
using Ttf.Server.Core.Contracts;

namespace Ttf.Server.Core.Engine.Strategy
{
    public class AllMatchingRulesHighestPrioCalculationStrategy : IEngineStrategy
    {
        public IEnumerable<Output> SelectOuputs(IEnumerable<Output> outputs)
        {
            return outputs.GroupBy(x => x.X).SelectMany(y => y.OrderByDescending(x => x.CalcPrio).Take(1));
        }
    }
}