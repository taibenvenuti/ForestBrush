using Harmony;

namespace ForestBrush
{
    [HarmonyPatch(typeof(TreeInstance), "GrowState", MethodType.Setter)]
    public static class SetGrowStatePatch
    {
        static void Prefix(ref TreeInstance __instance, ref int value) {
            value = 1;
        }
    }

    [HarmonyPatch(typeof(TreeInstance), "CheckOverlap")]
    public static class CheckOverlapPatch
    {
        static bool Prefix(ref TreeInstance __instance) {
            if (__instance.GrowState == 0) {
                DistrictManager districtManager = DistrictManager.instance;
                byte park = districtManager.GetPark(__instance.Position);
                districtManager.m_parks.m_buffer[park].m_treeCount++;
            }
            __instance.GrowState = 1;
            return false;
        }
    }
}
