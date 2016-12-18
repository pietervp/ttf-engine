using System.Collections.Generic;
using System.Text.RegularExpressions;
using Ttf.Server.Core.Contracts;

namespace Ttf.Server.Core.Engine.Expression
{
    public class RuleExpression : ParseableExpression<bool>
    {
        private const string RuleRegexPattern = @"(?<Expression>.*?) ?=\> X ?= ?(?<OutputType>[SRTsrt])\Z";

        public RuleExpression(string expr, int prio) : base(expr, prio)
        {
        }

        protected override Regex GetRegex() => new Regex(RuleRegexPattern);

        protected override Dictionary<string, object> GetParameters(Input input)
        {
            return input.GetRuleParameters();
        }
        public static bool IsMatch(string expr)
        {
            return Regex.IsMatch(expr, RuleRegexPattern);
        }
    }
}