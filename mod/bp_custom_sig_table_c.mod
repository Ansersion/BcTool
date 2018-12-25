
#include <bp_sig_str.h>
#include <bp_custom_sig_table.h>
#include <bp_sig_table_tools.h>

BP_SigId2Val g_CusSigId2Val[] = 
{
<CUSTOM_SIGNAL_ID_2_VAL>
	{<MACRO>, 0}, 
</CUSTOM_SIGNAL_ID_2_VAL>
};

const BP_SigTable g_CusSigTable[] = 
{
<CUSTOM_SIGNAL_TABLE>
	{<MACRO>, <TYPE_DEFINED>, <STATISTICS>, <DISPLAY>, <ACCURACY>, <ALARM>, <PERMISSION>, <ALARM_CLASS>, <CUSTOM_INFO>, RESERVED_FIELD, (SigTypeU *)&<MIN>, (SigTypeU *)&<MAX>, (SigTypeU *)&<DEF>, <ALARM_DELAY_BEF>, <ALARM_DELAY_AFT>},
</CUSTOM_SIGNAL_TABLE>
};

const BP_WORD g_CusSigNum = sizeof(g_CusSigId2Val) / sizeof(BP_SigId2Val);

const BP_UINT8 * g_CusSigNameLang[] = 
{
<SIGNAL_NAME_LANGUAGE>
    <SPANISH>,
    <ARABIC>,
    <RUSSIAN>,
    <FRENCH>,
    <ENGLISH>,
    <CHINESE>,
</SIGNAL_NAME_LANGUAGE>
};

const BP_UINT8 * g_CusSigUnitLang[] = 
{
<SIGNAL_UNIT_LANGUAGE>
    <SPANISH>,
    <ARABIC>,
    <RUSSIAN>,
    <FRENCH>,
    <ENGLISH>,
    <CHINESE>,
</SIGNAL_UNIT_LANGUAGE>
};

const BP_UINT8 * g_CusSigGroupLang[] = 
{
<SIGNAL_GROUP_LANGUAGE>
    <SPANISH>,
    <ARABIC>,
    <RUSSIAN>,
    <FRENCH>,
    <ENGLISH>,
    <CHINESE>,
</SIGNAL_GROUP_LANGUAGE>
};

const BP_UINT8 * g_CusSigEnumLang[] = 
{
    /* TODO: must be ordered */
};

const BP_CusLangMap g_CusSigNameLangMap[] = 
{
    /* 1 mean first language resource, 0 means no language resource */
<SIGNAL_NAME_LANGUAGE_UNIT>
    {<MACRO>, <SIGNAL_NAME_LANGUAGE_INDEX>},
</SIGNAL_NAME_LANGUAGE_UNIT>
};

const BP_WORD g_CusSigNameLangMapNum = sizeof(g_CusSigNameLangMap) / sizeof(BP_CusLangMap);

const BP_CusLangMap g_CusSigUnitLangMap[] = 
{
    /* 1 mean first language resource, 0 means no language resource */
    // {SIG_CUS_DEVICE_NAME, 0},
<SIGNAL_UNIT_LANGUAGE_UNIT>
    {<MACRO>, <SIGNAL_UNIT_LANGUAGE_INDEX>},
</SIGNAL_UNIT_LANGUAGE_UNIT>
};
const BP_WORD g_CusSigUnitLangMapNum = sizeof(g_CusSigUnitLangMap) / sizeof(BP_CusLangMap);

const BP_CusLangMap g_CusSigGroupLangMap[] = 
{
    /* 1 mean first language resource, 0 means no language resource */
    // {SIG_CUS_DEVICE_NAME, 0},
<SIGNAL_GROUP_LANGUAGE_UNIT>
    {<MACRO>, <SIGNAL_GROUP_LANGUAGE_INDEX>},
</SIGNAL_GROUP_LANGUAGE_UNIT>
};
const BP_WORD g_CusSigGroupLangMapNum = sizeof(g_CusSigGroupLangMap) / sizeof(BP_CusLangMap);

const BP_CusEnumLangMap g_CusSigEnumLangMap[] = 
{
    /* 1 mean first language resource, 0 means no language resource */
    // {SIG_CUS_DEVICE_NAME, 0},
};
const BP_WORD g_CusSigEnumLangMapNum = sizeof(g_CusSigEnumLangMap) / sizeof(BP_CusEnumLangMap);

