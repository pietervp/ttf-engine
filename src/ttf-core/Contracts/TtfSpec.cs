using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ttf.Server.Core.Contracts
{
    public class TtfSpec
    {
        private const string InheritsKeyword = "INHERITS";

        public TtfSpec(string filename)
        {
            var lines = File.ReadAllLines(filename).Where(line => !string.IsNullOrWhiteSpace(line)).Select(line => line.ToUpperInvariant()).ToList();
            SourceFile = filename;

            var firstLine = lines.FirstOrDefault();

            Inherits = firstLine != null && firstLine.StartsWith(InheritsKeyword)
                ? firstLine.Replace(InheritsKeyword, string.Empty).Trim()
                : null;

            Lines = Inherits == null ? lines : lines.Skip(1).ToList();
            Name = Path.GetFileNameWithoutExtension(filename).ToUpperInvariant();
        }

        public List<string> Lines { get; set; }
        public string SourceFile { get; set; }
        public string Inherits { get; set; }
        public string Name { get; set; }
    }
}