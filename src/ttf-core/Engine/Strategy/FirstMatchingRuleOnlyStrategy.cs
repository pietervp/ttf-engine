using System.Collections.Generic;
using System.Linq;
using Ttf.Server.Core.Contracts;

namespace Ttf.Server.Core.Engine.Strategy
{
    public class FirstMatchingRuleOnlyStrategy : IEngineStrategy
    {
        public IEnumerable<Output> SelectOuputs(IEnumerable<Output> outputs)
        {
            return outputs.OrderByDescending(x => x.RulePrio).Take(1);
        }
    }
}