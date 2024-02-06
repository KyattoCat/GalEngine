using System.Collections.Generic;
using System.Text;

namespace GalEngine.Core
{
    public class Command
    {
        private static StringBuilder stringBuilder = new StringBuilder();

        public int index;
        public CommandType type;
        public Dictionary<string, string> paramDic;

        public override string ToString()
        {
            stringBuilder.Clear();
            if (paramDic != null ) 
            {
                foreach (var kvp in paramDic)
                {
                    stringBuilder.Append(kvp.Key);
                    stringBuilder.Append("=");
                    stringBuilder.Append(kvp.Value);
                    stringBuilder.Append(" ");
                }
            }

            return $"[{index}] ({type}) {{{stringBuilder.ToString()}}}";
        }
    }
}
