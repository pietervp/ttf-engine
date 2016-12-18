using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NCalc.Domain;
using Ttf.Server.Core.Contracts;

namespace Ttf.Server.Core.Engine.Expression
{
    public abstract class ParseableExpression<T>
    {
        private readonly LogicalExpression _compiledExpression;
        

        public OutputType OutputType { get; }
        public int Prio { get; }

        protected ParseableExpression(string expression, int prio)
        {
            Prio = prio;

            var match = Match(expression);

            var rawExpr = match.Groups["Expression"].Value?.ToUpperInvariant();
            var target = match.Groups["OutputType"].Value?.ToUpperInvariant();

            OutputType = (OutputType) Enum.Parse(typeof(OutputType), target);
            _compiledExpression = NCalc.Expression.Compile(rawExpr, false);
        }

        private Match Match(string expression)
        {
            var regex = GetRegex();
            var match = regex.Match(expression);

            if (!match.Success)
                throw new ArgumentException($"{nameof(expression)} is not a valid expression", nameof(expression));
            return match;
        }

        public T Execute(Input input)
        {
            var expr = new NCalc.Expression(_compiledExpression);

            foreach (var element in GetParameters(input))
                expr.Parameters.Add(element.Key, element.Value);

            return (T) expr.Evaluate();
        }

        protected abstract Regex GetRegex();

        protected abstract Dictionary<string, object> GetParameters(Input input);
    }
}