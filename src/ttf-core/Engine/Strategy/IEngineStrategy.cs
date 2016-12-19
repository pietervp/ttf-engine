using System.Collections.Generic;
using Ttf.Server.Core.Contracts;

namespace Ttf.Server.Core.Engine.Strategy
{
    public interface IEngineStrategy
    {
        IEnumerable<Output> SelectOuputs(IEnumerable<Output> outputs);
    }
}