
#ifndef __BP_CUSTOM_SIG_TABLE_H
#define __BP_CUSTOM_SIG_TABLE_H

#include <bp_sig_str.h>

<LANGUAGE_SUPPORTED_INFO>
#define STD_LANGUAGE_SUPPORTED_NUM   <STD_LANG_NUM>
#define STD_LANGUAGE_SUPPORTED_MASK  <STD_LANG_MASK>
</LANGUAGE_SUPPORTED_INFO>

<MACRO_DEFINED>
#define <MACRO>            <SIGNAL_ID>
</MACRO_DEFINED>

extern BP_SigId2Val g_CusSigId2Val[];
extern const BP_WORD g_CusSigNum;
extern const BP_SigTable g_CusSigTable[];
extern const BP_UINT8 * g_CusSigNameLang[];
extern const BP_UINT8 * g_CusSigUnitLang[];
extern const BP_UINT8 * g_CusSigGroupLang[];
extern const BP_UINT8 * g_CusSigEnumLang[];
extern const BP_CusLangMap g_CusSigNameLangMap[];
extern const BP_WORD g_CusSigNameLangMapNum;
extern const BP_CusLangMap g_CusSigUnitLangMap[];
extern const BP_WORD g_CusSigUnitLangMapNum;
extern const BP_CusLangMap g_CusSigGroupLangMap[];
extern const BP_WORD g_CusSigGroupLangMapNum;
extern const BP_CusEnumLangMap g_CusSigEnumLangMap[];
extern const BP_WORD g_CusSigEnumLangMapNum;



#endif
