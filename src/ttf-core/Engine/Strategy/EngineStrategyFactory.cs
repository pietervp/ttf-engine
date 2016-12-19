namespace Ttf.Server.Core.Engine.Strategy
{
    public class EngineStrategyFactory
    {
        public static IEngineStrategy GetStrategy()
        {
            if (EngineConfig.IsFirstMatchingRuleOnly)
                return new FirstMatchingRuleOnlyStrategy();

            if (EngineConfig.IsAllMatchingRules)
                return new AllMatchingRulesStrategy();

            if (EngineConfig.IsAllMatchingRulesHighestPrioCalculation)
                return new AllMatchingRulesHighestPrioCalculationStrategy();

            return new FirstMatchingRuleOnlyStrategy();
        }
    }
}