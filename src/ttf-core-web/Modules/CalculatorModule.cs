using Nancy;
using Ttf.Server.Core.Contracts;
using Ttf.Server.Core.Engine;
//using NLog;

namespace Ttf.Server.Core.Web.Modules
{
    public sealed class CalculatorModule : NancyModule
    {
        public CalculatorModule() : base("api/calculator")
        {
            Get("/{engine}/(?<options>[01]{3})", parameters =>
            {
                //Logger.Debug("Start parsing request");

                Input input;
                CalculationEngine calculationEngine = GetEngine(parameters);

                if (calculationEngine == null)
                    return Error("No matching calculationEngine found!");

                var success = Input.TryParse(
                    parameters.options.ToString(),
                    Request.Query.d.ToString(),
                    Request.Query.e.ToString(),
                    Request.Query.f.ToString(),
                    out input);

                if (!success)
                    return Error("Could not parse input");

                //Logger.Debug($"Parsed input: {input}");

                var result = calculationEngine.GetCollectionFor(input);

                //Logger.Debug($"Calculated result: {result}");

                return Response.AsJson(result);
            });
        }

        private CalculationEngine GetEngine(dynamic parameters)
        {
            CalculationEngine calculationEngine = null;
            string engineName = parameters.engine.ToString()?.ToUpperInvariant();

            if (CalculationEngine.Engines.ContainsKey(engineName))
                calculationEngine = CalculationEngine.Engines[engineName];

            return calculationEngine;
        }

        private Response Error(string msg)
        {
            //Logger.Warn(msg);
            return Response.AsJson(new {Error = msg});
        }
    }
}