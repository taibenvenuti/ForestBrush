using ColossalFramework;
using ColossalFramework.Math;
using Harmony;
using System;
using System.Collections;
using UnityEngine;
using static RenderManager;

namespace ForestBrush
{
    [HarmonyPatch(typeof(ToolController), "SetBrush")]
    public class SetBrushPatch
    {
        static bool Prefix(ref Texture2D ___m_brush, ref float[] ___m_brushData, ref Vector3 ___m_brushPosition, ref float ___m_brushSize, Texture2D brush, Vector3 brushPosition, float brushSize)
        {
            if (ForestBrush.Instance.Active)
            {
                if (___m_brush != brush)
                {
                    ___m_brush = brush;

                    for (int i = 0; i < 64; i++)
                    {
                        for (int j = 0; j < 64; j++)
                        {
                            try
                            {
                                if (___m_brush != null) ___m_brushData[i * 64 + j] = ___m_brush.GetPixel(j, i).a;
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }

                }
                ___m_brushPosition = brushPosition;
                ___m_brushSize = brushSize;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(TreeTool), "ApplyBrush")]
    public class ApplyBrushPatch
    {
        internal static float Angle = ToolsModifierControl.cameraController.m_currentAngle.x;

        internal static bool AngleChanged;

        internal static bool RotationTogglePressed()
        {
            return (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand));
        }

        internal static bool DeleteForestTogglePressed()
        {
            return (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
        }

        static bool Prefix(TreeTool __instance, Randomizer ___m_randomizer, Vector3 ___m_mousePosition, bool ___m_mouseLeftDown, bool ___m_mouseRightDown, ToolController ___m_toolController)
        {
            if (!ForestBrush.Instance.Active) return true;
            else if ((___m_mouseLeftDown && ForestBrush.Instance.Container.m_variations.Length == 0) || (___m_mouseRightDown && RotationTogglePressed())) return false;
            else if (___m_mouseLeftDown || (___m_mouseRightDown && DeleteForestTogglePressed()))
            {
                if (__instance.m_prefab != null)
                {
                    int batchSize = (int)__instance.m_brushSize * ForestBrush.Instance.BrushTweaker.SizeMultiplier + ForestBrush.Instance.BrushTweaker.SizeAddend;
                    if (___m_mouseRightDown) batchSize *= 2;
                    ToolBase.RaycastInput input = new ToolBase.RaycastInput();
                    for (int i = 0; i < batchSize; i++)
                    {
                        var xz = UnityEngine.Random.insideUnitCircle;
                        var xzx = xz.x;
                        var xzy = xz.y;
                        var position = ___m_mousePosition + new Vector3(xzx, 0f, xzy) * (__instance.m_brushSize / 2);
                        if (UserMod.Settings.SelectedBrush.Options.IsSquare)
                        {
                            var radians = Angle * Mathf.Deg2Rad;
                            var c = Mathf.Cos(radians);
                            var s = Mathf.Sin(radians);
                            var brushRadius = new Vector2(__instance.m_brushSize, __instance.m_brushSize) * 0.5f;
                            var center = new Vector2(UnityEngine.Random.value, UnityEngine.Random.value);
                            var corner = center * __instance.m_brushSize - brushRadius;
                            var translated = corner - center;
                            xz.x = translated.x * c - translated.y * s;
                            xz.y = translated.y * c + translated.x * s;
                            xz += center;
                            xzx = xz.x;
                            xzy = xz.y;
                            position = ___m_mousePosition + new Vector3(xzx, 0f, xzy);
                        }
                        TreeInfo ___m_treeInfo = __instance.m_prefab.GetVariation(ref ___m_randomizer);

                        position.y = Singleton<TerrainManager>.instance.SampleDetailHeight(position, out float f, out float f2);
                        var spacing = UserMod.Settings.SelectedBrush.Options.AutoDensity ? ___m_treeInfo.m_generatedInfo.m_size.x / 2 : UserMod.Settings.SelectedBrush.Options.Density;

                        Randomizer randomizer = ___m_randomizer;
                        uint seed = Singleton<TreeManager>.instance.m_trees.NextFreeItem(ref randomizer);
                        Randomizer randomizer2 = new Randomizer(seed);
                        float num22 = ___m_treeInfo.m_minScale + (float)randomizer2.Int32(10000u) * (___m_treeInfo.m_maxScale - ___m_treeInfo.m_minScale) * 0.0001f;
                        float num23 = ___m_treeInfo.m_generatedInfo.m_size.y * num22;
                        float num24 = ForestBrush.Instance.BrushTweaker.Clearance;
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

                        if (___m_mouseLeftDown
                            && (Singleton<PropManager>.instance.OverlapQuad(quad, y, maxY, collisionType, 0, 0)
                                || Singleton<TreeManager>.instance.OverlapQuad(quad2, y, maxY, collisionType, 0, 0u)
                                || Singleton<NetManager>.instance.OverlapQuad(quad, y, maxY, collisionType, ___m_treeInfo.m_class.m_layer, 0, 0, 0)
                                || Singleton<BuildingManager>.instance.OverlapQuad(quad, y, maxY, collisionType, ___m_treeInfo.m_class.m_layer, 0, 0, 0)
                                || (Singleton<TerrainManager>.instance.HasWater(vector2)
                                    && !Input.GetKey(KeyCode.LeftAlt)
                                    && !Input.GetKey(KeyCode.RightAlt))))
                            continue;
                        var scale = ___m_randomizer.Int32(16);
                        var str2Rnd = UnityEngine.Random.Range(0.0f, ForestBrush.Instance.BrushTweaker.MaxRandomRange);
                        if (Mathf.PerlinNoise(position.x * scale, position.y * scale) > 0.5 && str2Rnd < UserMod.Settings.SelectedBrush.Options.Strength)
                        {
                            if (___m_mouseLeftDown )
                            {
                                if (Singleton<TreeManager>.instance.CreateTree(out uint num25, ref ___m_randomizer, ___m_treeInfo, position, false))
                                {
                                    // Tree created
                                }
                            }
                            else if (___m_mouseRightDown)
                            {
                                Vector3 screenPoint = Camera.main.WorldToScreenPoint(position);
                                Ray ray = Camera.main.ScreenPointToRay(screenPoint);
                                input.m_ray = ray;
                                input.m_length = Camera.main.farClipPlane;
                                input.m_rayRight = Camera.main.transform.TransformDirection(Vector3.right);
                                if (RayCast(input, out ToolBase.RaycastOutput output))
                                {
                                    uint tree = output.m_treeInstance;
                                    if (tree == 0) continue;
                                    TreeInfo treeInfo = TreeManager.instance.m_trees.m_buffer[tree].Info;
                                    if (ForestBrush.Instance.BrushTool.TreeInfos.Contains(treeInfo))
                                        TreeManager.instance.ReleaseTree(tree);
                                }
                            }
                        }
                    }
                }
                return false;
            }
            return true;
        }

        static bool RayCast(ToolBase.RaycastInput input, out ToolBase.RaycastOutput output)
        {
            
            object[] parameters = new object[] { input, null };
            object result = ForestBrush.Instance.RayCastMethod.Invoke(null, parameters);
            bool boolResult = (bool)result;
            if (boolResult)
                output = (ToolBase.RaycastOutput)parameters[1];
            else output = default(ToolBase.RaycastOutput);
            return boolResult;
        }
    }

    [HarmonyPatch(typeof(TreeTool), "RenderOverlay", new Type[] { typeof(CameraInfo) })]
    class RenderOverlayPatch
    {
        static bool Prefix(TreeTool __instance, ToolController ___m_toolController, ToolBase.ToolErrors ___m_placementErrors, Vector3 ___m_mousePosition, bool ___m_mouseRightDown, Randomizer ___m_randomizer, CameraInfo cameraInfo)
        {
            if (!ForestBrush.Instance.Active || ___m_mouseRightDown && !ApplyBrushPatch.DeleteForestTogglePressed() && !ApplyBrushPatch.RotationTogglePressed())
            {
                return true;
            }
            else
            {
                TreeInfo treeInfo = __instance.m_prefab;
                if (__instance.m_mode == TreeTool.Mode.Brush && treeInfo != null && !___m_toolController.IsInsideUI && Cursor.visible)
                {
                    var size = __instance.m_brushSize / 2;
                    Color toolColor = UserMod.Settings.SelectedBrush.Options.OverlayColor;
                    ___m_toolController.RenderColliding(cameraInfo, toolColor, toolColor, toolColor, toolColor, 0, 0);
                    ToolManager instance = Singleton<ToolManager>.instance;
                    instance.m_drawCallData.m_overlayCalls = instance.m_drawCallData.m_overlayCalls + 1;
                    if (UserMod.Settings.SelectedBrush.Options.IsSquare)
                    {
                        var Angle = ApplyBrushPatch.Angle;
                        var radians = Angle * Mathf.Deg2Rad;
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
                        Singleton<RenderManager>.instance.OverlayEffect.DrawQuad(cameraInfo, toolColor, quad, ___m_mousePosition.y - 100f, ___m_mousePosition.y + 100f, false, false);
                    }
                    else Singleton<RenderManager>.instance.OverlayEffect.DrawCircle(cameraInfo, toolColor, ___m_mousePosition, size * 2, ___m_mousePosition.y - 100f, ___m_mousePosition.y + 100f, false, true);
                }
                return false;
            }
        }
    }

    [HarmonyPatch(typeof(TreeTool), "OnToolGUI")]
    public class OnToolGUIPatch
    {
        static bool Prefix(TreeTool __instance, ToolController ___m_toolController, ref bool ___m_mouseLeftDown, ref bool ___m_mouseRightDown, Event e)
        {
            if (ForestBrush.Instance.Active)
            {
                if (!___m_toolController.IsInsideUI && e.type == EventType.MouseDown)
                {
                    if (e.button == 0)
                    {
                        ___m_mouseLeftDown = true;
                        if (__instance.m_mode == TreeTool.Mode.Single)
                        {
                            Singleton<SimulationManager>.instance.AddAction((IEnumerator)typeof(TreeTool).GetMethod("CreateTree", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(__instance, null));
                        }
                    }
                    else if (e.button == 1)
                    {
                        ___m_mouseRightDown = true;
                        ApplyBrushPatch.AngleChanged = false;
                    }
                }
                else if (e.type == EventType.MouseUp)
                {
                    if (e.button == 0)
                    {
                        ___m_mouseLeftDown = false;
                    }
                    else if (e.button == 1)
                    {
                        ___m_mouseRightDown = false;
                        if (__instance.m_mode == TreeTool.Mode.Brush && !ApplyBrushPatch.AngleChanged)
                        {
                            Singleton<SimulationManager>.instance.AddAction(ToggleAngle());
                        }
                    }
                }
                return false;
            }
            else return true;
        }

        private static IEnumerator ToggleAngle()
        {
            ApplyBrushPatch.Angle = Mathf.Round(ApplyBrushPatch.Angle / 45f - 1f) * 45f;
            if (ApplyBrushPatch.Angle < 0f)
            {
                ApplyBrushPatch.Angle += 360f;
            }
            if (ApplyBrushPatch.Angle >= 360f)
            {
                ApplyBrushPatch.Angle -= 360f;
            }
            yield return 0;
            yield break;
        }
    }

    [HarmonyPatch(typeof(TreeTool), "OnToolUpdate")]
    public class OnToolUpdatePatch
    {
        static void Prefix(ref TreeTool __instance, ToolController ___m_toolController, ref float ___m_brushSize)
        {
            if (ForestBrush.Instance.Active)
            {
                __instance.m_mode = TreeTool.Mode.Brush;
                ___m_brushSize = UserMod.Settings.SelectedBrush.Options.Size;
            }
        }

        static void Postfix(TreeTool __instance, ToolController ___m_toolController)
        {
            if (ForestBrush.Instance.Active)
            {
                if (__instance.m_mode == TreeTool.Mode.Brush && Input.GetKey(KeyCode.Mouse1) && ApplyBrushPatch.RotationTogglePressed())
                {
                    float axis = Input.GetAxis("Mouse X");
                    if (axis != 0f)
                    {
                        ApplyBrushPatch.AngleChanged = true;
                        Singleton<SimulationManager>.instance.AddAction(DeltaAngle(axis * 10f));
                    }
                }
            }
        }

        private static IEnumerator DeltaAngle(float delta)
        {
            ApplyBrushPatch.Angle += delta;
            yield return 0;
            yield break;
        }
    }
}
