using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BcTool
{
    class DataGridViewChecker
    {
        Boolean checkAll()
        {
            Boolean ret = false;
            try
            {
                ret = true;
            } 
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return ret;
        }
    }
}
