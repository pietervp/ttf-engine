using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Ttf.Server.Core.Contracts;
using Ttf.Server.Core.Engine.Expression;
using Ttf.Server.Core.Engine.Strategy;

namespace Ttf.Server.Core.Engine
{
    public class CalculationEngine : ICloneable
    {
        public static ImmutableDictionary<string, CalculationEngine> Engines = ImmutableDictionary<string, CalculationEngine>.Empty;

        public string Version { get; }
        public int Level { get; }

        public ImmutableList<CalcExpression> Calcs { get; private set; }
        public ImmutableList<RuleExpression> Rules { get; private set; }

        private readonly IEngineStrategy _strategy;
        private int _rulePrio = 20, _calcPrio = 2;

        public CalculationEngine(string version)
        {
            Level = 1;
            Version = version;
            Calcs = ImmutableList<CalcExpression>.Empty;
            Rules = ImmutableList<RuleExpression>.Empty;

            _strategy = EngineStrategyFactory.GetStrategy();
        }

        public CalculationEngine(CalculationEngine parent, string version) : this(version)
        {
            Rules = existing.Rules.ToImmutableList();
            Calcs = existing.Calcs.ToImmutableList();
            _strategy = existing._strategy;
            Level = existing.Level + 1;
        }

        public object Clone()
        {
            return new CalculationEngine(this, Version);
        }

        public OutputCollection GetCollectionFor(Input input)
        {
            var results = GetResultsFor(input).ToList();

            return new OutputCollection
            {
                Results = results,
                ResultCount = results.Count,
                TimeStamp = DateTime.UtcNow,
                Version = Version
            };
        }

        public IEnumerable<Output> GetResultsFor(Input input)
        {
            var q = from r in Rules.Where(a => a.Execute(input))
                    join c in Calcs on r.OutputType equals c.OutputType
                    select new Output
                    {
                        X = r.OutputType,
                        Y = c.Execute(input),
                        CalcPrio = c.Prio,
                        RulePrio = r.Prio
                    };

            return _strategy.SelectOuputs(q);
        }

        public void AddSpec(string specLine)
        {
            if (CalcExpression.IsMatch(specLine))
                AddCalc(specLine);
            else if (RuleExpression.IsMatch(specLine))
                AddRule(specLine);
            else
                throw new ArgumentException($"Could not parse '{specLine}' as either a rule or a calculation!", nameof(specLine));
        }

        public void AddRule(params string[] rule)
        {
            Rules = Rules.AddRange(rule.Select(r => new RuleExpression(r, Interlocked.Increment(ref _rulePrio) * Level)));
        }

        public void AddCalc(params string[] rule)
        {
            Calcs = Calcs.AddRange(rule.Select(r => new CalcExpression(r, Interlocked.Increment(ref _calcPrio) * Level)));
        }
    }
}
