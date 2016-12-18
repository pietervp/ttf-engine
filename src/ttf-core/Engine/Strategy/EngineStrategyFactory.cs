namespace Ttf.Server.Core.Engine.Strategy
{
    public class EngineStrategyFactory
    {
        public static IEngineStrategy GetStrategy()
        {
            if (EngineConfig.FirstMatchingRuleOnly)
                return new FirstMatchingRuleOnlyStrategy();

            if (EngineConfig.AllMatchingRules)
                return new AllMatchingRulesStrategy();

            if (EngineConfig.AllMatchingRulesHighestPrioCalculation)
                return new AllMatchingRulesHighestPrioCalculationStrategy();

            return new FirstMatchingRuleOnlyStrategy();
        }
    }
}