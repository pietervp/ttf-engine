using System.Collections.Generic;
using System.Linq;
using Ttf.Server.Core.Contracts;

namespace Ttf.Server.Core.Engine.Strategy
{
    public class AllMatchingRulesStrategy : IEngineStrategy
    {
        public IEnumerable<Output> SelectOuputs(IEnumerable<Output> outputs)
        {
            return outputs.OrderByDescending(x => x.Prio);
        }
    }
}