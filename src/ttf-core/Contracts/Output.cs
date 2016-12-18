using NetJSON;

namespace Ttf.Server.Core.Contracts
{
    public struct Output
    {
        [NetJSONProperty("x")]
        public OutputType X { get; set; }

        [NetJSONProperty("y")]
        public double Y { get; set; }
        
        public int RulePrio;
        
        public int CalcPrio;
        
        public int Prio => RulePrio + CalcPrio;
    }
}