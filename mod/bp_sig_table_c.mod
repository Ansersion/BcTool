#include <bp_sig_table.h>
#include <bp_public.h>
#include <bp_strlen.h>
#include <bp_sig_table_tools.h>

<SIGNAL_MIN_MAX_DEF>
const SigTypeU <MACRO>_MIN = {.<TYPE> = <MIN>};
const SigTypeU <MACRO>_MAX = {.<TYPE> = <MAX>};
const SigTypeU <MACRO>_DEF = {.<TYPE> = <DEF>};
</SIGNAL_MIN_MAX_DEF>

/* system signal id to value array index*/
BP_SigId2Val g_SysSigId2Val[] = 
{
<SIGNAL_ID_2_VAL>
	{<MACRO>, 0}, 
</SIGNAL_ID_2_VAL>
};

/* system signal id to signal table */
const BP_SigTable g_SysSigTable[] = 
{
	<SYSTEM_SIGNAL_TABLE>
	{<MACRO>, <TYPE_DEFINED>, <STATISTICS>, <DISPLAY>, <ACCURACY>, <ALARM>, <PERMISSION>, <ALARM_CLASS>, <CUSTOM_INFO>, RESERVED_FIELD, (SigTypeU *)&<MIN>, (SigTypeU *)&<MAX>, (SigTypeU *)&<DEF>, <ALARM_DELAY_BEF>, <ALARM_DELAY_AFT>},
	</SYSTEM_SIGNAL_TABLE>
};

/* system signal number */
const BP_WORD g_SysSigNum = sizeof(g_SysSigId2Val) / sizeof(BP_SigId2Val);

/* system signal custom value */
<SYSTEM_SIGNAL_CUSTOM_VALUE>
const BP_UINT8 <MACRO>_IS_ALARM = <IS_ALARM>;
const BP_UINT16 <MACRO>_UNIT_ID = <UNIT_ID>;
const BP_UINT16 <MACRO>_PERMISSION = <PERMISSION>;
const BP_UINT8 <MACRO>_IS_DISPLAY = <IS_DISPLAY>;
const BP_UINT8 <MACRO>_ACCURACY = <ACCURACY>;
const <C_VALUE_TYPE> <MACRO>_MIN_VAL = <MIN_VAL>;
const <C_VALUE_TYPE> <MACRO>_MAX_VAL = <MAX_VAL>;
const <C_VALUE_TYPE> <MACRO>_DEF_VAL = <DEF_VAL>;
const BP_UINT16 <MACRO>_GROUP_ID = <GROUP_ID>;
const BP_EnumSignalMap <MACRO>_ENUM_MAP[] = 
{
	{<KEY>, <VALUE>},
};
const BP_SysCusEnumUnit <MACRO>_ENUM_LANG = {sizeof(<MACRO>_ENUM_MAP) / sizeof(BP_EnumSignalMap), <MACRO>_ENUM_MAP};
const BP_UINT8 <MACRO>_EN_STATISTICS = <EN_STATISTICS>;
const BP_UINT8 <MACRO>_ALARM_CLASS = <ALARM_CLASS>;
const BP_UINT8 <MACRO>_DELAY_BEFORE_ALARM = <DELAY_BEFORE_ALARM>;
const BP_UINT8 <MACRO>_DELAY_AFTER_ALARM = <DELAY_AFTER_ALARM>;
</SYSTEM_SIGNAL_CUSTOM_VALUE>


/* system signal custom value unit array */
const BP_SysCustomUnit g_SysCustomUnitTable[] = {
    /* must be sorted which same signal id get togather */
<CUSTOM_INFO_TABLE_UNIT>
    {<MACRO>, <CUSTOM_INFO_TYPE>, (void *)<CUSTOM_INFO>},
</CUSTOM_INFO_TABLE_UNIT>
};

/* system signal custom unit number */
const BP_WORD g_SysCustomUnitNum = sizeof(g_SysCustomUnitTable) / sizeof(BP_SysCustomUnit);


/* system signal enable map */
<SYSTEM_SIGNAL_ENABLE_DIST>
const BP_UINT8 g_SysMapDis_<DIST_N>[] = {<ENABLE_LIST>};
</SYSTEM_SIGNAL_ENABLE_DIST>

const BP_SysSigMap g_SysSigMap[] = 
{
<SYSTEM_SIGNAL_ENABLE_DIST_UNIT>
	{<DIST_N_MAP> | <DIST_CLASS> | <DIST_END_FLAG>, sizeof(g_SysMapDis_<DIST_N>) / sizeof(BP_UINT8), g_SysMapDis_<DIST_N>}, 
</SYSTEM_SIGNAL_ENABLE_DIST_UNIT>
};

const BP_WORD g_SysSigMapSize = sizeof(g_SysSigMap) / sizeof(BP_SysSigMap);


