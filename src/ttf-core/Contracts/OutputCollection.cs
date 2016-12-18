using System;
using System.Collections.Generic;
using NetJSON;

namespace Ttf.Server.Core.Contracts
{
    public class OutputCollection
    {
        [NetJSONProperty("version")]
        public string Version { get; set; }

        [NetJSONProperty("timestamp")]
        public DateTime TimeStamp { get; set; }

        [NetJSONProperty("resultCount")]
        public int ResultCount { get; set; }

        [NetJSONProperty("results")]
        public List<Output> Results { get; set; }

        public override string ToString()
        {
            return $"{nameof(Version)}: {Version}, {nameof(TimeStamp)}: {TimeStamp}, {nameof(ResultCount)}: {ResultCount}";
        }
    }
}