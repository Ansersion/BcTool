
#include <bp_sig_str.h>
#include <bp_custom_sig_table.h>
#include <bp_sig_table_tools.h>

<CUSTOM_SIGNAL_MIN_MAX_DEF_VAL>
const SigTypeU <MACRO>_MIN = {.<TYPE> = <MIN>};
const SigTypeU <MACRO>_MAX = {.<TYPE> = <MAX>};
const SigTypeU <MACRO>_DEF = {.<TYPE> = <DEF>};
</CUSTOM_SIGNAL_MIN_MAX_DEF_VAL>

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
<SIGNAL_ENUM_LANGUAGE>
    <SPANISH>,
    <ARABIC>,
    <RUSSIAN>,
    <FRENCH>,
    <ENGLISH>,
    <CHINESE>,
</SIGNAL_ENUM_LANGUAGE>
};

const BP_CusLangMap g_CusSigUnitLangMap[] = 
{
    /* 1 mean first language resource, 0 means no language resource */
<SIGNAL_UNIT_LANGUAGE_UNIT>
    {<MACRO>, <SIGNAL_UNIT_LANGUAGE_INDEX>},
</SIGNAL_UNIT_LANGUAGE_UNIT>
};
const BP_WORD g_CusSigUnitLangMapNum = sizeof(g_CusSigUnitLangMap) / sizeof(BP_CusLangMap);

const BP_CusLangMap g_CusSigGroupLangMap[] = 
{
    /* 1 mean first language resource, 0 means no language resource */
<SIGNAL_GROUP_LANGUAGE_UNIT>
    {<MACRO>, <SIGNAL_GROUP_LANGUAGE_INDEX>},
</SIGNAL_GROUP_LANGUAGE_UNIT>
};
const BP_WORD g_CusSigGroupLangMapNum = sizeof(g_CusSigGroupLangMap) / sizeof(BP_CusLangMap);

<SIGNAL_ENUM_LANGUAGE_MAP>
BP_EnumSignalMap <MACRO>_ENUM_MAP[] = 
{
    {<ENUM_VAL>, <LANG_INDEX>}, 
};
</SIGNAL_ENUM_LANGUAGE_UNIT>

BP_SigId2EnumSignalMap g_CusSigId2EnumSigMap[] =
{
<SIGNAL_ENUM_LANGUAGE_MAP_UNIT>
    {<MACRO>, <MACRO>_ENUM_MAP, sizeof(<MACRO>_ENUM_MAP) / sizeof(BP_EnumSignalMap)},
</SIGNAL_ENUM_LANGUAGE_MAP_UNIT>
};

BP_WORD g_CusSigId2EnumSignalMapNum = sizeof(g_CusSigId2EnumSigMap) / sizeof(BP_SigId2EnumSignalMap);


