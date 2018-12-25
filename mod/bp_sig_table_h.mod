
#ifndef __BP_SIG_TABLE_H
#define __BP_SIG_TABLE_H

#include <bp_sig_str.h>

<MACRO_DEFINED>
#define <MACRO>            <SIGNAL_ID>
</MACRO_DEFINED>

extern BP_SigId2Val g_SysSigId2Val[];
extern const BP_SigTable g_SysSigTable[];
extern const BP_WORD g_SysSigNum;
extern const BP_SysCustomUnit g_SysCustomUnit[];
extern const BP_WORD g_SysCustomUnitNum;
extern const BP_SysCustomUnit g_SysCustomUnitTable[];
extern const BP_WORD g_SysCustomUnitNum;

<SYSTEM_SIGNAL_MAP_DIST>
extern const BP_UINT8 g_SysMapDis_<DIST_N>[];
</SYSTEM_SIGNAL_MAP_DIST>

extern const BP_SysSigMap g_SysSigMap[];
extern const BP_WORD g_SysSigMapSize;


#endif

