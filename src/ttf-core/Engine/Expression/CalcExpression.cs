using System.Collections.Generic;
using System.Text.RegularExpressions;
using Ttf.Server.Core.Contracts;

namespace Ttf.Server.Core.Engine.Expression
{
    public class CalcExpression : ParseableExpression<double>
    {
        private const string CalcRegexPattern = @"[Xx] ?= ?(?<OutputType>[SRTsrt]) ?=\> ?[Yy] ?= ?(?<Expression>.*?)\Z";

        public CalcExpression(string expr, int prio) : base(expr, prio)
        {
        }

        protected override Regex GetRegex() => new Regex(CalcRegexPattern);

        protected override Dictionary<string, object> GetParameters(Input input)
        {
            return input.GetCalcParameters();
        }

        public static bool IsMatch(string expr)
        {
            return Regex.IsMatch(expr, CalcRegexPattern);
        }
    }
}