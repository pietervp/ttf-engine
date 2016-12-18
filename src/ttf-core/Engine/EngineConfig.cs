namespace Ttf.Server.Core.Engine
{
    public static class EngineConfig
    {
        public static bool IsFirstMatchingRuleOnly => Mode == EngineOutputMode.FirstMatchingRuleOnly;
        public static bool IsAllMatchingRulesHighestPrioCalculation => Mode == EngineOutputMode.AllMatchingRulesHighestPrioCalculation;
        public static bool IsAllMatchingRules => Mode == EngineOutputMode.AllMatchingRules;

        public static EngineOutputMode Mode = EngineOutputMode.AllMatchingRules;
    }

    public enum EngineOutputMode
    {
        FirstMatchingRuleOnly,
        AllMatchingRulesHighestPrioCalculation,
        AllMatchingRules
    }
}