using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BcTool
{
    public class LanguageResourceItem
    {
        public const int CHINESE_KEY = 7;
        public const int ENGLISH_KEY = 6;
        public const int FRENCH_KEY = 5;
        public const int RUSSIAN_KEY = 4;
        public const int ARABIC_KEY = 3;
        public const int SPANISH_KEY = 2;

        private int indexId;
        private Dictionary<UInt16, string> languageMap;

        public int IndexId { get => indexId; set => indexId = value; }
        public Dictionary<ushort, string> LanguageMap { get => languageMap; set => languageMap = value; }

        public static LanguageResourceItem parseLanguageResourceItem(System.Windows.Forms.DataGridViewCellCollection row, ref string err)
        {
            LanguageResourceItem languageResourceItemRet = null;
            if (null == row)
            {
                return languageResourceItemRet;
            }

            try
            {
                String tmp;

                tmp = row["LanguageID"].Value.ToString().Trim();
                int languageId = -1;
                try
                {
                    languageId = Convert.ToInt32(tmp, 16);
                }
                catch (Exception e)
                {
                    languageId = -1;
                }
                if (languageId < 0)
                {
                    if (null != err)
                    {
                        err = "<LanguageID> error: \"" + tmp + "\"";
                    }
                    return languageResourceItemRet;
                }

                Dictionary<UInt16, string> languageMap = new Dictionary<ushort, string>();

                tmp = row["Chinese"].Value.ToString().Trim();
                languageMap.Add(CHINESE_KEY, tmp);
                tmp = row["English"].Value.ToString().Trim();
                languageMap.Add(ENGLISH_KEY, tmp);
                tmp = row["French"].Value.ToString().Trim();
                languageMap.Add(FRENCH_KEY, tmp);
                tmp = row["Russian"].Value.ToString().Trim();
                languageMap.Add(RUSSIAN_KEY, tmp);
                tmp = row["Arabic"].Value.ToString().Trim();
                languageMap.Add(ARABIC_KEY, tmp);
                tmp = row["Spanish"].Value.ToString().Trim();
                languageMap.Add(SPANISH_KEY, tmp);

                languageResourceItemRet = new LanguageResourceItem(languageId, languageMap);
            }
            catch (Exception e)
            {
                languageResourceItemRet = null;
                if (err != null)
                {
                    err += "(Catch exception)";
                }
            }

            return languageResourceItemRet;
        }

        public LanguageResourceItem(int indexId, Dictionary<ushort, string> languageMap)
        {
            this.indexId = indexId;
            this.languageMap = languageMap;
        }
    }
}
