using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Ttf.Server.Core.Contracts;
using Ttf.Server.Core.Engine;

namespace Ttf.Server.Core
{
    public class TtfStartup
    {
        public static IConfigurationRoot AppConfig{ get; set; }

        public static void ConfigureMapper(IConfigurationRoot configuration)
        {
            EngineConfig.Mode = EngineOutputMode.AllMatchingRules;
            AppConfig = configuration;

            ParseTtfSpecFiles();

            var watcher = new FileSystemWatcher(AppConfig["ttf-folder"],
                $"*{AppConfig["ttf-extension"]}")
            {
                EnableRaisingEvents = true
            };

            watcher.Changed += (sender, args) =>
            {
                ((FileSystemWatcher)sender).EnableRaisingEvents = false;
                Logger.Info($"CHANGED {args.ChangeType} :: {args.Name}");
                ParseTtfSpecFiles();
                ((FileSystemWatcher)sender).EnableRaisingEvents = true;
            };

            watcher.Deleted += (sender, args) =>
            {
                Logger.Info($"DELETED {args.ChangeType} :: {args.Name}");
                ParseTtfSpecFiles();
            };
        }

        private static void ParseTtfSpecFiles()
        {
            // parse *ttfspec* files and create engines based on them
            var specs = Directory
                            .EnumerateFiles(
                                AppConfig["ttf-folder"],
                                $"*{AppConfig["ttf-extension"]}")
                            .Select(ttfspec => new TtfSpec(ttfspec))
                            .ToList();

            var engines = specs
                            .Where(x => x.Inherits == null)
                            .SelectMany(root => ResolveSpec(root, null, specs));

            CalculationEngine.Engines = engines
                                        .ToImmutableDictionary(x => x.Version, x => x);
        }

        private static IEnumerable<CalculationEngine> ResolveSpec(TtfSpec spec, CalculationEngine parent, List<TtfSpec> allSpecs)
        {
            Logger.Info($"Parsing spec {spec.SourceFile} (parent: {parent?.Version ?? "none"})");

            var engine = parent == null
                            ? new CalculationEngine(spec.Name)
                            : new CalculationEngine(parent, spec.Name);

            IEnumerable<CalculationEngine> engines = new List<CalculationEngine> { engine };

            foreach (var specLine in spec.Lines)
            {
                Logger.Debug($"Inspecting specline '{specLine}'");
                engine.AddSpec(specLine);
            }

            // get child engines
            foreach (var source in allSpecs.Where(x => x.Inherits == spec.Name))
            {
                engines = engines.Concat(ResolveSpec(source, engine, allSpecs));
            }

            return engines;
        }
    }
}
