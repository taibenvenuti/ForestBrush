using ColossalFramework;
using ColossalFramework.Math;
using ColossalFramework.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ForestBrush
{
    public class ForestTool : ToolBase
    {
        public class Tweaks
        {
            public uint NoiseScale;
            public float NoiseThreshold;
            public float StrengthMultiplier;
            public float MaxRandomRange;
            public float Clearance;
            public float maxSize;
            public float MaxSize
            {
                get
                {
                    return maxSize;
                }
                set
                {
                    maxSize = value;
                    ForestBrush.Instance.ForestBrushPanel.BrushOptionsSection.sizeSlider.maxValue = value;
                }
            }
        }

        public Tweaks Tweaker = new Tweaks()
        {
            NoiseThreshold = 0.5f,
            StrengthMultiplier = 100000,
            NoiseScale = 16,
            MaxRandomRange = 1.0f,
            Clearance = 4.5f,
            maxSize = 2000f
        };

        private float Angle = ToolsModifierControl.cameraController.m_currentAngle.x;
        private bool AngleChanged;
        private float MouseRayLength;
        private bool MouseLeftDown;
        private bool MouseRightDown;
        private bool MouseRayValid;
        private Ray MouseRay;
        private Vector3 MouseRayRight;
        private Vector3 CachedPosition;
        private Vector3 MousePosition;
        private Color DeleteColor = Color.red;
        private Color PaintColor = Color.green;
        private Randomizer Randomizer;
        public TreeInfo Container = ForestBrush.Instance.Container;
        private Texture2D BrushTexture;
        private float[] BrushData;

        public int ID_BrushTex { get; private set; }
        public int ID_BrushWS { get; private set; }

        private float Size => Options.Size;
        private float Strength => Options.Strength;
        private float Density => Options.Density;
        private Color BrushColor
        {
            set
            {
                BrushMaterial.SetVector("_TerrainBrushColor1", new Vector4(value.r, value.g, value.b, value.a));
                BrushMaterial.SetVector("_TerrainBrushColor2", new Vector4(value.gamma.r, value.gamma.g, value.gamma.b, value.gamma.a));
            }
        }
        private bool SquareBrush => Options.IsSquare;
        private Brush.BrushOptions Options => UserMod.Settings.SelectedBrush.Options;
        private bool ShiftDown => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        private bool AltDown => Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        private bool CtrlDown => Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)
                              || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand);

        private bool Painting => MouseLeftDown && !CtrlDown && !ShiftDown && !MouseRightDown;
        private bool Deleting => MouseRightDown && !MouseLeftDown && !AltDown && !CtrlDown;
        private bool Rotating => MouseRightDown && CtrlDown && !ShiftDown && !AltDown && !MouseLeftDown;
        private bool SelectiveDelete => MouseRightDown && ShiftDown && !CtrlDown && !AltDown && !MouseLeftDown;
        private bool ChangingStrength => MouseRightDown && AltDown && !CtrlDown && !ShiftDown && !MouseLeftDown;
        private bool ChangingSize => MouseRightDown && MouseLeftDown;
        private bool ChangingDensity => MouseRightDown && MouseLeftDown && AltDown;

        public Material BrushMaterial { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            BrushData = new float[4096];
            ID_BrushTex = Shader.PropertyToID("_BrushTex");
            ID_BrushWS = Shader.PropertyToID("_BrushWS");
            RenderManager renderManager = RenderManager.instance;
            OverlayEffect overlayEffect = renderManager.OverlayEffect;
            Material material = overlayEffect.GetType().GetField("m_shapeMaterialBlend", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(overlayEffect) as Material;
            BrushMaterial = new Material(material) { shader = ToolsModifierControl.toolController.m_brushMaterial.shader };
            Randomizer = new Randomizer((int)DateTime.Now.Ticks);
            try
            {
                FieldInfo fieldInfo = typeof(ToolController).GetField("m_tools", BindingFlags.Instance | BindingFlags.NonPublic);
                ToolBase[] tools = (ToolBase[])fieldInfo.GetValue(ToolsModifierControl.toolController);
                int initialLength = tools.Length;
                Array.Resize(ref tools, initialLength + 1);
                Dictionary<Type, ToolBase> dictionary = (Dictionary<Type, ToolBase>)typeof(ToolsModifierControl).GetField("m_Tools", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
                dictionary.Add(typeof(ForestTool), this);
                tools[initialLength] = this;
                fieldInfo.SetValue(ToolsModifierControl.toolController, tools);
                ToolsModifierControl.toolController.CurrentTool = ToolsModifierControl.toolController.GetComponent<DefaultTool>();
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Exception caught: {exception}");
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            try
            {
                FieldInfo fieldInfo = typeof(ToolController).GetField("m_tools", BindingFlags.Instance | BindingFlags.NonPublic);
                List<ToolBase> tools = ((ToolBase[])fieldInfo.GetValue(ToolsModifierControl.toolController)).ToList();
                tools.Remove(this);
                fieldInfo.SetValue(ToolsModifierControl.toolController, tools.ToArray());
                Dictionary<Type, ToolBase> dictionary = (Dictionary<Type, ToolBase>)typeof(ToolsModifierControl).GetField("m_Tools", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
                dictionary.Remove(typeof(ForestTool));
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Exception caught: {exception}");
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetBrush(ToolsModifierControl.toolController.m_brushes[3]);
            this.m_toolController.ClearColliding();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            SetBrush(null);
            MouseLeftDown = false;
            MouseRightDown = false;
            MouseRayValid = false;
        }

        private void SetBrush(Texture2D brush)
        {
            if (BrushTexture != brush)
            {
                BrushTexture = brush;
                if (BrushTexture != null)
                {
                    for (int i = 0; i < 64; i++)
                    {
                        for (int j = 0; j < 64; j++)
                        {
                            BrushData[i * 64 + j] = BrushTexture.GetPixel(j, i).a;
                        }
                    }
                }
            }
        }

        protected override void OnToolGUI(Event e)
        {
            if (!this.m_toolController.IsInsideUI && e.type == EventType.MouseDown)
            {
                if (e.button == 0)
                {
                    MouseLeftDown = true;
                }
                else if (e.button == 1)
                {
                    MouseRightDown = true;
                    AngleChanged = false;
                }
            }
            else if (e.type == EventType.MouseUp)
            {
                if (e.button == 0)
                {
                    MouseLeftDown = false;
                }
                else if (e.button == 1)
                {
                    MouseRightDown = false;
                    if(!AngleChanged) Singleton<SimulationManager>.instance.AddAction(ClampAngle());
                }
            }
        }

        protected override void OnToolUpdate()
        {
            if (Container is null) return;
            if (MouseRayValid)
            {
                if (UserMod.Settings.ShowInfoTooltip)
                {
                    string text = $"Trees: {Container.m_variations.Length}\nSize: {Options.Size}\nStrength: {Options.Strength}\nDensity: {16 - Options.Density}";
                    base.ShowToolInfo(true, text, CachedPosition);
                }
                else base.ShowToolInfo(false, null, CachedPosition);
            }
            else base.ShowToolInfo(false, null, CachedPosition);
            if (MouseRightDown || MouseLeftDown)
            {
                float axis = Input.GetAxis("MouseX");
                if (axis != 0)
                {
                    if (Rotating)
                    {
                        AngleChanged = true;
                        Singleton<SimulationManager>.instance.AddAction(DeltaAngle(Input.GetAxis("MouseX") * 10.0f));
                    }
                    else if (ChangingSize)
                    {
                        Options.Size = Mathf.Clamp(Mathf.RoundToInt(Options.Size + Input.GetAxis("MouseX") * 10.0f), 0.0f, 500.0f);
                    }
                    else if (ChangingStrength)
                    {
                        Options.Strength = Mathf.Clamp(Mathf.RoundToInt(Options.Strength + Input.GetAxis("MouseX")), 0.01f, 1.0f);
                    }
                    else if (ChangingDensity)
                    {
                        Options.Density = Mathf.Clamp(Mathf.RoundToInt(Options.Density + Input.GetAxis("MouseX")), 0.0f, 16.0f);
                    }
                }
            }
        }

        protected override void OnToolLateUpdate()
        {
            MouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            MouseRayLength = Camera.main.farClipPlane;
            MouseRayRight = Camera.main.transform.TransformDirection(Vector3.right);
            MouseRayValid = (!m_toolController.IsInsideUI && Cursor.visible);
            CachedPosition = MousePosition;
        }

        public override void SimulationStep()
        {
            if (Container is null) return;
            RaycastInput input = new RaycastInput(MouseRay, MouseRayLength);
            if (RayCast(input, out RaycastOutput output))
            {
                MousePosition = output.m_hitPos;
                if (MouseLeftDown != MouseRightDown) ApplyBrush();
            }
        }

        private void ApplyBrush()
        {
            if (Container is null) return;
            if (Painting) AddTreesImpl();
            else if (Deleting) RemoveTreesImpl();
        }

        private void AddTreesImpl()
        {
            float brushSize = Size * 1.5f;
            float brushRadius = brushSize / 2;
            float cellSize = TreeManager.TREEGRID_CELL_SIZE;
            int resolution = TreeManager.TREEGRID_RESOLUTION;
            TreeInstance[] trees = TreeManager.instance.m_trees.m_buffer;
            uint[] treeGrid = TreeManager.instance.m_treeGrid;
            float strength = Strength * Tweaker.StrengthMultiplier;
            Vector3 position = MousePosition;

            int minX = Mathf.Max((int)((position.x - brushRadius) / cellSize + resolution * 0.5f), 0);
            int minZ = Mathf.Max((int)((position.z - brushRadius) / cellSize + resolution * 0.5f), 0);
            int maxX = Mathf.Min((int)((position.x + brushRadius) / cellSize + resolution * 0.5f), resolution - 1);
            int maxZ = Mathf.Min((int)((position.z + brushRadius) / cellSize + resolution * 0.5f), resolution - 1);

            for (int z = minZ; z <= maxZ; ++z)
            {
                float brushZ = ((z - resolution * 0.5f + 0.5f) * cellSize - position.z + brushRadius) / brushSize * 64.0f - 0.5f;
                int z0 = Mathf.Clamp(Mathf.FloorToInt(brushZ), 0, 63);
                int z1 = Mathf.Clamp(Mathf.CeilToInt(brushZ), 0, 63);

                for (int x = minX; x <= maxX; ++x)
                {
                    float brushX = ((x - resolution * 0.5f + 0.5f) * cellSize - position.x + brushRadius) / brushSize * 64.0f - 0.5f;
                    int x0 = Mathf.Clamp(Mathf.FloorToInt(brushX), 0, 63);
                    int x1 = Mathf.Clamp(Mathf.CeilToInt(brushX), 0, 63);

                    float brush00 = BrushData[z0 * 64 + x0];
                    float brush10 = BrushData[z0 * 64 + x1];
                    float brush01 = BrushData[z1 * 64 + x0];
                    float brush11 = BrushData[z1 * 64 + x1];

                    float brush0 = brush00 + (brush10 - brush00) * (brushX - x0);
                    float brush1 = brush01 + (brush11 - brush01) * (brushX - x0);
                    float brush = brush0 + (brush1 - brush0) * (brushZ - z0);

                    int change = (int)(strength * (brush * 1.2f - 0.2f) * 10000.0f);
                    
                    if (Randomizer.Int32(10000) < change)
                    {
                        TreeInfo treeInfo = Container.GetVariation(ref Randomizer);

                        Vector3 treePos;
                        treePos.x = (x - resolution * 0.5f) * cellSize;
                        treePos.z = (z - resolution * 0.5f) * cellSize;
                        treePos.x += (Randomizer.Int32(10000) + 0.5f) * (cellSize / 10000.0f);
                        treePos.z += (Randomizer.Int32(10000) + 0.5f) * (cellSize / 10000.0f);
                        treePos.y = 0.0f;
                        treePos.y = TerrainManager.instance.SampleDetailHeight(treePos, out float slopeX, out float slopeZ);

                        Randomizer tempRandomizer = Randomizer;
                        uint item = TreeManager.instance.m_trees.NextFreeItem(ref tempRandomizer);
                        Randomizer treeRandomizer = new Randomizer(item);
                        float scale = treeInfo.m_minScale + treeRandomizer.Int32(10000) * (treeInfo.m_maxScale - treeInfo.m_minScale) * 0.0001f;
                        float height = treeInfo.m_generatedInfo.m_size.y * scale;
                        float clearance = Tweaker.Clearance;
                        float spacing = Options.AutoDensity ? treeInfo.m_generatedInfo.m_size.x / 2 : Density;
                        Vector2 treePos2 = VectorUtils.XZ(treePos);
                        Quad2 clearanceQuad = new Quad2();
                        clearanceQuad.a = treePos2 + new Vector2(-clearance, -clearance);
                        clearanceQuad.b = treePos2 + new Vector2(-clearance, clearance);
                        clearanceQuad.c = treePos2 + new Vector2(clearance, clearance);
                        clearanceQuad.d = treePos2 + new Vector2(clearance, -clearance);
                        Quad2 spacingQuad = new Quad2();
                        spacingQuad.a = treePos2 + new Vector2(-spacing, -spacing);
                        spacingQuad.b = treePos2 + new Vector2(-spacing, spacing);
                        spacingQuad.c = treePos2 + new Vector2(spacing, spacing);
                        spacingQuad.d = treePos2 + new Vector2(spacing, -spacing);
                        float minY = position.y;
                        float maxY = position.y + height;
                        ItemClass.CollisionType collisionType = ItemClass.CollisionType.Terrain;

                        if (PropManager.instance.OverlapQuad(clearanceQuad, minY, maxY, collisionType, 0, 0) && !AltDown) continue;
                        if (TreeManager.instance.OverlapQuad(spacingQuad, minY, maxY, collisionType, 0, 0) && !AltDown) continue;
                        if (NetManager.instance.OverlapQuad(clearanceQuad, minY, maxY, collisionType, treeInfo.m_class.m_layer, 0, 0, 0) && !AltDown) continue;
                        if (BuildingManager.instance.OverlapQuad(clearanceQuad, minY, maxY, collisionType, treeInfo.m_class.m_layer, 0, 0, 0) && !AltDown) continue;
                        if (TerrainManager.instance.HasWater(treePos2) && !AltDown) continue;

                        var noiseScale = treeRandomizer.Int32(Tweaker.NoiseScale);
                        var strengthToRandom = UnityEngine.Random.Range(0.0f, Tweaker.MaxRandomRange);
                        if (Mathf.PerlinNoise(treePos2.x * noiseScale, treePos2.y * noiseScale) > Tweaker.NoiseThreshold && strengthToRandom < Stren    gth)
                        {
                            float actualRadius = Size / 2;
                            Vector2 actualPosition = treePos2;
                            Vector2 mousePos = new Vector2(MousePosition.x, MousePosition.z);
                            if ((mousePos - actualPosition).sqrMagnitude <= actualRadius * actualRadius)
                                if (TreeManager.instance.CreateTree(out uint treeIndex, ref Randomizer, treeInfo, treePos, false))
                                {
                                }
                        }
                    }
                }
            }
        }

        private void RemoveTreesImpl()
        {
            float size = Size * 2;
            float brushRadius = size / 2;
            float cellSize = TreeManager.TREEGRID_CELL_SIZE;
            int resolution = TreeManager.TREEGRID_RESOLUTION;
            TreeInstance[] trees = TreeManager.instance.m_trees.m_buffer;
            uint[] treeGrid = TreeManager.instance.m_treeGrid;
            float strength = Strength;
            Vector3 position = MousePosition;

            int minX = Mathf.Max((int)((position.x - brushRadius) / cellSize + resolution * 0.5f), 0);
            int minZ = Mathf.Max((int)((position.z - brushRadius) / cellSize + resolution * 0.5f), 0);
            int maxX = Mathf.Min((int)((position.x + brushRadius) / cellSize + resolution * 0.5f), resolution - 1);
            int maxZ = Mathf.Min((int)((position.z + brushRadius) / cellSize + resolution * 0.5f), resolution - 1);

            for (int z = minZ; z <= maxZ; ++z)
            {
                float brushZ = ((z - resolution * 0.5f + 0.5f) * cellSize - position.z + brushRadius) / size * 64.0f - 0.5f;
                int z0 = Mathf.Clamp(Mathf.FloorToInt(brushZ), 0, 63);
                int z1 = Mathf.Clamp(Mathf.CeilToInt(brushZ), 0, 63);

                for (int x = minX; x <= maxX; ++x)
                {
                    float brushX = ((x - resolution * 0.5f + 0.5f) * cellSize - position.x + brushRadius) / size * 64.0f - 0.5f;
                    int x0 = Mathf.Clamp(Mathf.FloorToInt(brushX), 0, 63);
                    int x1 = Mathf.Clamp(Mathf.CeilToInt(brushX), 0, 63);

                    float brush00 = BrushData[z0 * 64 + x0];
                    float brush10 = BrushData[z0 * 64 + x1];
                    float brush01 = BrushData[z1 * 64 + x0];
                    float brush11 = BrushData[z1 * 64 + x1];

                    float brush0 = brush00 + (brush10 - brush00) * (brushX - x0);
                    float brush1 = brush01 + (brush11 - brush01) * (brushX - x0);
                    float brush = brush0 + (brush1 - brush0) * (brushZ - z0);

                    int change = (int)(strength * (brush * 1.2f - 0.2f) * 10000.0f);

                    uint treeIndex = treeGrid[z * resolution + x];
                        
                    while (treeIndex != 0)
                    {
                        uint next = trees[treeIndex].m_nextGridTree;
                        if (Randomizer.Int32(10000) < change)
                        {
                            TreeInfo treeInfo = TreeManager.instance.m_trees.m_buffer[treeIndex].Info;
                            if ((ShiftDown && ForestBrush.Instance.BrushTool.TreeInfos.Contains(treeInfo)) || !ShiftDown)
                            {
                                float actualRadius = Size / 2;
                                Vector3 treePosition = TreeManager.instance.m_trees.m_buffer[treeIndex].Position;
                                Vector2 actualPosition = new Vector2(treePosition.x, treePosition.z);
                                Vector2 mousePos = new Vector2(MousePosition.x, MousePosition.z);
                                if ((mousePos - actualPosition).sqrMagnitude <= actualRadius * actualRadius) TreeManager.instance.ReleaseTree(treeIndex);
                            }
                        }
                        treeIndex = next;
                    }
                }
            }
        }

        public void RenderBrush(RenderManager.CameraInfo cameraInfo)
        {
            if (BrushTexture != null)
            {
                BrushMaterial.SetTexture(ID_BrushTex, BrushTexture);
                Vector4 value = MousePosition;
                value.w = Size;
                BrushMaterial.SetVector(this.ID_BrushWS, value);
                Color toolColor = MouseRightDown && !CtrlDown ? DeleteColor : PaintColor;
                BrushColor = toolColor;
                Vector3 center = new Vector3(MousePosition.x, 512f, MousePosition.z);
                Vector3 size = new Vector3(Size, 1224f, Size);
                Bounds bounds = new Bounds(center, size);
                ToolManager instance = Singleton<ToolManager>.instance;
                instance.m_drawCallData.m_overlayCalls = instance.m_drawCallData.m_overlayCalls + 1;
                Singleton<RenderManager>.instance.OverlayEffect.DrawEffect(cameraInfo, BrushMaterial, 0, bounds);
            }
        }

        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            if (MouseRayValid)
                RenderBrush(cameraInfo);
        }

        private IEnumerator ClampAngle()
        {
            Angle = Mathf.Round(Angle / 45f - 1f) * 45f;
            if (Angle < 0f)
            {
                Angle += 360f;
            }
            if (Angle >= 360f)
            {
                Angle -= 360f;
            }
            yield return 0;
            yield break;
        }

        private  IEnumerator DeltaAngle(float delta)
        {
            Angle += delta;
            yield return 0;
            yield break;
        }
    }
}
