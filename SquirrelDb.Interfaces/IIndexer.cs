using System;
using System.Collections.Generic;

namespace SquirrelDb.Interfaces
{
    public interface IIndexer
    {
        string Name { get; }

        Func<string, bool> IndexDocument { get; }

        List<string> PostProcessing(List<string> documents, Dictionary<string,string> arguments);

    }
}
