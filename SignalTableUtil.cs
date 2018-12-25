using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BcTool
{
    class SignalTableUtil
    {
        public static Regex REGEX_SIGNAL_TABLE_BLOCK_START = new Regex(@"<(\w+)>");

        public static Regex makeSignalTableBlockEndRegex(string tag)
        {
            return new Regex(@"</" + tag + @">");
        }
    }
}
