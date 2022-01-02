using ForestBrush.Persistence;
using HarmonyLib;

namespace ForestBrush
{
    [HarmonyPatch(typeof(TreeInstance), "CheckOverlap")]
    public static class CheckOverlapPatch
    {
        static bool Prefix(ref TreeInstance __instance, uint treeID) {
            if (!GrowState.data.Contains(treeID)) return true;
            if (__instance.GrowState == 0) {
                DistrictManager districtManager = DistrictManager.instance;
                byte park = districtManager.GetPark(__instance.Position);
                districtManager.m_parks.m_buffer[park].m_treeCount++;
            }
            __instance.GrowState = 1;
            return false;
        }
    }

    [HarmonyPatch(typeof(TreeManager), "ReleaseTree")]
    public static class ReleaseTreePatch
    {
        static void Postfix(uint tree) {
            if (GrowState.data.Contains(tree)) GrowState.data.Remove(tree);
        }
    }
}
