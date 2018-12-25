using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BcTool
{
    public partial class BcTool
    {
        private Regex REGEX_CODE_BLOCK_MACRO_DEFINED = new Regex(@"(\s*#define\s+)(<MACRO>)\s+(<SIGNAL_ID>)");

        public string constructCodeBlock(string codBlockTag, string codeBlock)
        {
            string ret = "";

            try
            {
                string input = "AB$3C";
                input = input.Replace("$", "$$");
                
                ret = REGEX_CODE_BLOCK_MACRO_DEFINED.Replace(codeBlock, "$1" + input + " " + "0xE000");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                ret = "";
            }

            return ret;
        }
    }
}
