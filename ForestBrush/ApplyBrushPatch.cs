using ColossalFramework;
using ColossalFramework.Math;
using Harmony;
using System;
using UnityEngine;
using static RenderManager;

namespace ForestBrush
{
    [HarmonyPatch(typeof(TreeTool), "ApplyBrush")]
    public class ApplyBrushPatch
    {
        static bool Prefix(TreeTool __instance, Randomizer ___m_randomizer, Vector3 ___m_mousePosition, bool ___m_mouseLeftDown, bool ___m_mouseRightDown, ToolController ___m_toolController)
        {
            if(ForestBrushes.instance.BrushPanel != null)
            {

                ___m_toolController.SetBrush(null, Vector3.zero, 1f);
                if (___m_mouseLeftDown && __instance.m_prefab != null)
                {
                    for (var i = 0; i < 1024; i++)
                    {
                        var xz = UnityEngine.Random.insideUnitCircle;
                        var _x = xz.x;
                        var _y = xz.y;
                        var position = ___m_mousePosition + new Vector3(_x, 0f, _y) * (__instance.m_brushSize / 2);
                        if (UserMod.Settings.SquareBrush)
                        {
                            var angle = ToolsModifierControl.cameraController.m_currentAngle.x;
                            angle += 180f;
                            var radians = angle * Mathf.Deg2Rad;
                            var c = Mathf.Cos(radians);
                            var s = Mathf.Sin(radians);
                            var brushRadius = new Vector2(__instance.m_brushSize, __instance.m_brushSize) * 0.5f;                            
                            var center = new Vector2(UnityEngine.Random.value, UnityEngine.Random.value);
                            var corner = center * __instance.m_brushSize - brushRadius;
                            var translated  = corner - center;
                            xz.x = translated.x * c - translated.y * s;
                            xz.y = translated.y * c + translated.x * s;
                            xz += center;
                            _x = xz.x;
                            _y = xz.y;
                            position = ___m_mousePosition + new Vector3(_x, 0f, _y);
                        }
                        TreeInfo ___m_treeInfo;
                        if ((Singleton<ToolManager>.instance.m_properties.m_mode & ItemClass.Availability.AssetEditor) != ItemClass.Availability.None)
                        {
                            ___m_treeInfo = __instance.m_prefab;
                        }
                        else
                        {
                            ___m_treeInfo = __instance.m_prefab.GetVariation(ref ___m_randomizer);
                        }
                        
                        position.y = Singleton<TerrainManager>.instance.SampleDetailHeight(position, out float f, out float f2);
                        var spacing = UserMod.Settings.UseTreeSize ? ___m_treeInfo.m_generatedInfo.m_size.x / 2 : UserMod.Settings.Spacing;
                        if (Mathf.Max(Mathf.Abs(f), Mathf.Abs(f2)) < (float)___m_randomizer.Int32(10000u) * 5E-05f)
                        {
                            Randomizer randomizer = ___m_randomizer;
                            uint seed = Singleton<TreeManager>.instance.m_trees.NextFreeItem(ref randomizer);
                            Randomizer randomizer2 = new Randomizer(seed);
                            float num22 = ___m_treeInfo.m_minScale + (float)randomizer2.Int32(10000u) * (___m_treeInfo.m_maxScale - ___m_treeInfo.m_minScale) * 0.0001f;
                            float num23 = ___m_treeInfo.m_generatedInfo.m_size.y * num22;
                            float num24 = 4.5f;
                            Vector2 vector2 = VectorUtils.XZ(position);
                            Quad2 quad = default(Quad2);
                            quad.a = vector2 + new Vector2(-num24, -num24);
                            quad.b = vector2 + new Vector2(-num24, num24);
                            quad.c = vector2 + new Vector2(num24, num24);
                            quad.d = vector2 + new Vector2(num24, -num24);
                            Quad2 quad2 = default(Quad2);
                            quad2.a = vector2 + new Vector2(-spacing, -spacing);
                            quad2.b = vector2 + new Vector2(-spacing, spacing);
                            quad2.c = vector2 + new Vector2(spacing, spacing);
                            quad2.d = vector2 + new Vector2(spacing, -spacing);
                            float y = ___m_mousePosition.y;
                            float maxY = ___m_mousePosition.y + num23;
                            ItemClass.CollisionType collisionType = ItemClass.CollisionType.Terrain;
                            if (!Singleton<PropManager>.instance.OverlapQuad(quad, y, maxY, collisionType, 0, 0))
                            {
                                if (!Singleton<TreeManager>.instance.OverlapQuad(quad2, y, maxY, collisionType, 0, 0u))
                                {
                                    if (!Singleton<NetManager>.instance.OverlapQuad(quad, y, maxY, collisionType, ___m_treeInfo.m_class.m_layer, 0, 0, 0))
                                    {
                                        if (!Singleton<BuildingManager>.instance.OverlapQuad(quad, y, maxY, collisionType, ___m_treeInfo.m_class.m_layer, 0, 0, 0))
                                        {
                                            if (!Singleton<TerrainManager>.instance.HasWater(vector2))
                                            {
                                                var scale = randomizer.Int32(160) / 10;
                                                if (Mathf.PerlinNoise(position.x * scale, position.y * scale) > 0.5)
                                                {
                                                    uint num25;
                                                    if (Singleton<TreeManager>.instance.CreateTree(out num25, ref ___m_randomizer, ___m_treeInfo, position, false))
                                                    {
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (___m_mouseRightDown || __instance.m_prefab == null)
                {
                    float[] brushData = ___m_toolController.BrushData;
                    float num = __instance.m_brushSize * 0.5f;
                    float num2 = 32f;
                    int num3 = 540;
                    global::TreeInstance[] buffer = Singleton<TreeManager>.instance.m_trees.m_buffer;
                    uint[] treeGrid = Singleton<TreeManager>.instance.m_treeGrid;
                    float strength = __instance.m_strength;
                    Vector3 mousePosition = ___m_mousePosition;
                    int num4 = Mathf.Max((int)((mousePosition.x - num) / num2 + (float)num3 * 0.5f), 0);
                    int num5 = Mathf.Max((int)((mousePosition.z - num) / num2 + (float)num3 * 0.5f), 0);
                    int num6 = Mathf.Min((int)((mousePosition.x + num) / num2 + (float)num3 * 0.5f), num3 - 1);
                    int num7 = Mathf.Min((int)((mousePosition.z + num) / num2 + (float)num3 * 0.5f), num3 - 1);
                    for (int i = num5; i <= num7; i++)
                    {
                        float num8 = (((float)i - (float)num3 * 0.5f + 0.5f) * num2 - mousePosition.z + num) / __instance.m_brushSize * 64f - 0.5f;
                        int num9 = Mathf.Clamp(Mathf.FloorToInt(num8), 0, 63);
                        int num10 = Mathf.Clamp(Mathf.CeilToInt(num8), 0, 63);
                        for (int j = num4; j <= num6; j++)
                        {
                            float num11 = (((float)j - (float)num3 * 0.5f + 0.5f) * num2 - mousePosition.x + num) / __instance.m_brushSize * 64f - 0.5f;
                            int num12 = Mathf.Clamp(Mathf.FloorToInt(num11), 0, 63);
                            int num13 = Mathf.Clamp(Mathf.CeilToInt(num11), 0, 63);
                            float num14 = brushData[num9 * 64 + num12];
                            float num15 = brushData[num9 * 64 + num13];
                            float num16 = brushData[num10 * 64 + num12];
                            float num17 = brushData[num10 * 64 + num13];
                            float num18 = num14 + (num15 - num14) * (num11 - (float)num12);
                            float num19 = num16 + (num17 - num16) * (num11 - (float)num12);
                            float num20 = num18 + (num19 - num18) * (num8 - (float)num9);
                            int num21 = (int)(strength * (num20 * 1.2f - 0.2f) * 10000f);

                            uint num26 = treeGrid[i * num3 + j];
                            int num27 = 0;
                            while (num26 != 0u)
                            {
                                uint nextGridTree = buffer[(int)((UIntPtr)num26)].m_nextGridTree;
                                if (___m_randomizer.Int32(10000u) < num21)
                                {
                                    Singleton<TreeManager>.instance.ReleaseTree(num26);
                                }
                                num26 = nextGridTree;
                                if (++num27 >= 262144)
                                {
                                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                                    break;
                                }
                            }

                        }
                    }
                }
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(TreeTool), "RenderOverlay", new Type[] { typeof(CameraInfo) })]
    class RenderOverlayPatch
    {
        static bool Prefix(TreeTool __instance, ToolController ___m_toolController, ToolBase.ToolErrors ___m_placementErrors, Vector3 ___m_mousePosition, Randomizer ___m_randomizer, CameraInfo cameraInfo)
        {
            if(ForestBrushes.instance.BrushPanel != null)
            {
                TreeInfo treeInfo = __instance.m_prefab;
                if (__instance.m_mode == TreeTool.Mode.Brush && treeInfo != null && !___m_toolController.IsInsideUI && Cursor.visible)
                {
                    var size = __instance.m_brushSize / 2;
                    Color toolColor = ToolsModifierControl.toolController.m_validColorInfo;
                    ___m_toolController.RenderColliding(cameraInfo, toolColor, toolColor, toolColor, toolColor, 0, 0);
                    ToolManager instance = Singleton<ToolManager>.instance;
                    instance.m_drawCallData.m_overlayCalls = instance.m_drawCallData.m_overlayCalls + 1;
                    if(UserMod.Settings.SquareBrush)
                    {
                        var cameraAngleX = ToolsModifierControl.cameraController.m_targetAngle.x;
                        cameraAngleX += 180f;
                        var radians = cameraAngleX * Mathf.Deg2Rad;
                        var cos = Mathf.Cos(radians);
                        var sin = Mathf.Sin(radians);                       

                        Quad3 quad = default(Quad3);
                        Vector2 xz = VectorUtils.XZ(___m_mousePosition);
                        
                        var a = xz + new Vector2(-size, -size);
                        var aT = a - xz;
                        a.x = aT.x * cos - aT.y * sin;
                        a.y = aT.y * cos + aT.x * sin;
                        a += xz;


                        var b = xz + new Vector2(-size, size);
                        var bT = b - xz;
                        b.x = bT.x * cos - bT.y * sin;
                        b.y = bT.y * cos + bT.x * sin;
                        b += xz;

                        var c = xz + new Vector2(size, size);
                        var cT = c - xz;
                        c.x = cT.x * cos - cT.y * sin;
                        c.y = cT.y * cos + cT.x * sin;
                        c += xz;

                        var d = xz + new Vector2(size, -size);
                        var dT = d - xz;
                        d.x = dT.x * cos - dT.y * sin;
                        d.y = dT.y * cos + dT.x * sin;
                        d += xz;

                        quad.a = new Vector3(a.x, 0f, a.y);
                        quad.b = new Vector3(b.x, 0f, b.y);
                        quad.c = new Vector3(c.x, 0f, c.y);
                        quad.d = new Vector3(d.x, 0f, d.y);
                        Singleton<RenderManager>.instance.OverlayEffect.DrawQuad(cameraInfo, toolColor, quad, ___m_mousePosition.y - 100f, ___m_mousePosition.y + 100f, false, true);
                    }                        
                    else Singleton<RenderManager>.instance.OverlayEffect.DrawCircle(cameraInfo, toolColor, ___m_mousePosition, size * 2, ___m_mousePosition.y - 100f, ___m_mousePosition.y + 100f, false, true);
                }
                return false;
            }
            return true;
        }

        private static Color GetToolColor(bool warning, bool error)
        {
            if (Singleton<InfoManager>.instance.CurrentMode != InfoManager.InfoMode.None)
            {
                if (error)
                {
                    return ToolsModifierControl.toolController.m_errorColorInfo;
                }
                if (warning)
                {
                    return ToolsModifierControl.toolController.m_warningColorInfo;
                }
                return ToolsModifierControl.toolController.m_validColorInfo;
            }
            else
            {
                if (error)
                {
                    return ToolsModifierControl.toolController.m_errorColor;
                }
                if (warning)
                {
                    return ToolsModifierControl.toolController.m_warningColor;
                }
                return ToolsModifierControl.toolController.m_validColor;
            }
        }
    }
}
