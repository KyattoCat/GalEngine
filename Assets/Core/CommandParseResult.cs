using System.Collections.Generic;

namespace GalEngine.Core
{
    public class CommandParseResult
    {
        public List<Command> commands;
        public Dictionary<string, int> jumpTable;
    }
}