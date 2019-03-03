using ColossalFramework;
using ColossalFramework.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ForestBrush
{
    public partial class ForestTool : ToolBase
    {
        public enum Mode
        {
            Bitmap,
            Geometric
        }

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
        private Color DeleteColor = new Color32(128, 0, 0, 128);
        private Color PaintColor => Options.OverlayColor;
        public Color Color => Deleting ? DeleteColor : PaintColor; 
        private Randomizer Randomizer;
        public TreeInfo Container = ForestBrush.Instance.Container;

        private Mode ToolMode => Options.ToolMode;
        private float Size => Options.Size;
        private float Strength => Options.Strength;
        private float Density => Options.Density;
        private bool SquareBrush => Options.IsSquare;
        private Brush.BrushOptions Options => UserMod.Settings.SelectedBrush.Options;

        private bool ShiftDown => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        private bool AltDown => Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        private bool CtrlDown => Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)
                              || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand);

        private bool Painting => MouseLeftDown && !CtrlDown && !ShiftDown && !MouseRightDown;
        private bool Deleting => MouseRightDown&& !AltDown && !CtrlDown && !MouseLeftDown;
        private bool SizeOrRotation => MouseRightDown && CtrlDown && !ShiftDown && !AltDown && !MouseLeftDown;
        private bool SelectiveDelete => MouseRightDown && ShiftDown && !CtrlDown && !AltDown && !MouseLeftDown;
        private bool StrengthOrDensity => MouseRightDown && AltDown && !AltDown && !ShiftDown && !MouseLeftDown;


        private BoxCollider2D BoxCollider = new BoxCollider2D();
        private Texture2D Rotated { get; set; }
        private float[] BrushData;
        public int ID_Angle { get; private set; }
        public int ID_BrushTex { get; private set; }
        public int ID_BrushWS { get; private set; }
        public Material BrushMaterial { get; private set; }
        private Mesh BoxMesh { get; set; }
        public Texture2D[] Brushes { get; private set; }
        private Shader Shader { get; } = Resources.ResourceLoader.LoadCustomShaderFromBundle();

        private Texture2D _original;
        public Texture2D BrushTexture
        {
            get
            {
                if (_original == null) _original = Brushes[36];
                SetBrush(_original);
                return _original;
            }
            set
            {
                _original = value;
                SetBrush(_original);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            BoxMesh = RenderManager.instance.OverlayEffect.GetType().GetField("m_boxMesh", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(RenderManager.instance.OverlayEffect) as Mesh;
            BrushData = new float[128 * 128];
            Brushes = Resources.ResourceLoader.LoadBrushTextures();
            ID_BrushTex = Shader.PropertyToID("_BrushTex");
            ID_BrushWS = Shader.PropertyToID("_BrushWS");
            ID_Angle = Shader.PropertyToID("_Angle");
            BrushMaterial = new Material(ToolsModifierControl.toolController.m_brushMaterial) { shader = Shader };
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
                if (BrushMaterial != null)
                {
                    Destroy(BrushMaterial);
                    BrushMaterial = null;
                }
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Exception caught: {exception}");
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.m_toolController.ClearColliding();
            BrushTexture = Brushes[36];
            ClampAngle();
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
            if (brush != null)
            {
                for (int i = 0; i < 128; i++)
                {
                    for (int j = 0; j < 128; j++)
                    {
                        BrushData[i * 128 + j] = brush.GetPixel(j, i).a;
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
                    if(!AngleChanged) Rotate45();
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
            if (MouseRightDown)
            {
                float axisX = Input.GetAxis("Mouse X");
                float axisY = Input.GetAxis("Mouse Y");
                if (axisX != 0)
                {
                    if (SizeOrRotation)
                    {
                        AngleChanged = true;
                        DeltaAngle(axisX * 10.0f);
                    }
                    else if (StrengthOrDensity)
                    {
                        Options.Strength = Mathf.Clamp(Options.Strength + axisX * 10.0f, 0.01f, 1.0f);
                    }
                }
                if (axisY != 0)
                {
                    if (SizeOrRotation)
                    {
                        Options.Size = Mathf.Clamp(Options.Size + axisY * 10.0f, 1.0f, 500.0f);
                    }
                    else if (StrengthOrDensity)
                    {
                        Options.Density = Mathf.Clamp(Options.Density + axisY * 10.0f, 0.0f, 16.0f);
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
            switch (ToolMode)
            {
                case Mode.Bitmap: if (Painting) AddTreesBitmapImpl(); else if(Deleting) RemoveTreesBitmapImpl(); break;
                case Mode.Geometric: if (Painting) AddTreesImpl(); else if (Deleting) RemoveTreesImpl(); break;
            }
        }

        private void AddTreesImpl()
        {
            int batchSize = (int)Size * Tweaker.SizeMultiplier + Tweaker.SizeAddend;
            for (int i = 0; i < batchSize; i++)
            {
                Vector2 treePosition = UnityEngine.Random.insideUnitCircle;
                Vector3 position = MousePosition + new Vector3(treePosition.x, 0f, treePosition.y) * (Size / 2);

                if (SquareBrush)
                {
                    float radians = Angle * Mathf.Deg2Rad;
                    float cosine = Mathf.Cos(radians);
                    float sine = Mathf.Sin(radians);
                    Vector2 radiusVector = new Vector2(Size, Size) * 0.5f;
                    Vector2 center = new Vector2(UnityEngine.Random.value, UnityEngine.Random.value);
                    Vector2 corner = center * Size - radiusVector;
                    Vector2 translated = corner - center;
                    treePosition.x = translated.x * cosine - translated.y * sine;
                    treePosition.y = translated.y * cosine + translated.x * sine;
                    treePosition += center;
                    position = MousePosition + new Vector3(treePosition.x, 0f, treePosition.y);
                }
                TreeInfo treeInfo = Container.GetVariation(ref Randomizer);

                position.y = Singleton<TerrainManager>.instance.SampleDetailHeight(position, out float f, out float f2);
                float spacing = Options.AutoDensity ? treeInfo.m_generatedInfo.m_size.x / 2 : Density;
                Randomizer tempRandomizer = Randomizer;
                uint item = TreeManager.instance.m_trees.NextFreeItem(ref tempRandomizer);
                Randomizer treeRandomizer = new Randomizer(item);
                float scale = treeInfo.m_minScale + (float)treeRandomizer.Int32(10000u) * (treeInfo.m_maxScale - treeInfo.m_minScale) * 0.0001f;
                float height = treeInfo.m_generatedInfo.m_size.y * scale;
                float clearance = Tweaker.Clearance;
                Vector2 treePosition2 = VectorUtils.XZ(position);
                Quad2 clearanceQuad = default(Quad2);
                clearanceQuad.a = treePosition2 + new Vector2(-clearance, -clearance);
                clearanceQuad.b = treePosition2 + new Vector2(-clearance, clearance);
                clearanceQuad.c = treePosition2 + new Vector2(clearance, clearance);
                clearanceQuad.d = treePosition2 + new Vector2(clearance, -clearance);
                Quad2 spacingQuad = default(Quad2);
                spacingQuad.a = treePosition2 + new Vector2(-spacing, -spacing);
                spacingQuad.b = treePosition2 + new Vector2(-spacing, spacing);
                spacingQuad.c = treePosition2 + new Vector2(spacing, spacing);
                spacingQuad.d = treePosition2 + new Vector2(spacing, -spacing);
                float minY = MousePosition.y;
                float maxY = MousePosition.y + height;
                ItemClass.CollisionType collisionType = ItemClass.CollisionType.Terrain;

                if (PropManager.instance.OverlapQuad(clearanceQuad, minY, maxY, collisionType, 0, 0) && !AltDown) continue;
                if (TreeManager.instance.OverlapQuad(spacingQuad, minY, maxY, collisionType, 0, 0) && !AltDown) continue;
                if (NetManager.instance.OverlapQuad(clearanceQuad, minY, maxY, collisionType, treeInfo.m_class.m_layer, 0, 0, 0) && !AltDown) continue;
                if (BuildingManager.instance.OverlapQuad(clearanceQuad, minY, maxY, collisionType, treeInfo.m_class.m_layer, 0, 0, 0) && !AltDown) continue;
                if (TerrainManager.instance.HasWater(position) && !AltDown) continue;
                int noiseScale = Randomizer.Int32(16);
                float str2Rnd = UnityEngine.Random.Range(0.0f, Tweaker.MaxRandomRange);
                if (Mathf.PerlinNoise(position.x * noiseScale, position.y * noiseScale) > 0.5 && str2Rnd < Strength * Tweaker.StrengthMultiplier)
                {
                    if (Singleton<TreeManager>.instance.CreateTree(out uint num25, ref Randomizer, treeInfo, position, false)) { }
                }
            }
        }

        private void AddTreesBitmapImpl()
        {
            int batchSize = (int)Size * Tweaker.SizeMultiplier + Tweaker.SizeAddend;
            for (int i = 0; i < batchSize; i++)
            {
                Vector2 randomPosition = UnityEngine.Random.insideUnitCircle;
                Vector3 treePosition = MousePosition + new Vector3(randomPosition.x, 0f, randomPosition.y) * (Size / 2);

                float brushRadius = Size / 2;

                var distance = treePosition - MousePosition;
                var distanceRotated = Quaternion.Euler(0, Angle, 0) * distance;

                float brushZ = (distanceRotated.z + brushRadius) / Size * 128.0f - 0.5f;
                int z0 = Mathf.Clamp(Mathf.FloorToInt(brushZ), 0, 127);
                int z1 = Mathf.Clamp(Mathf.CeilToInt(brushZ), 0, 127);

                float brushX = (distanceRotated.x + brushRadius) / Size * 128.0f - 0.5f;
                int x0 = Mathf.Clamp(Mathf.FloorToInt(brushX), 0, 127);
                int x1 = Mathf.Clamp(Mathf.CeilToInt(brushX), 0, 127);

                float brush00 = BrushData[z0 * 128 + x0];
                float brush10 = BrushData[z0 * 128 + x1];
                float brush01 = BrushData[z1 * 128 + x0];
                float brush11 = BrushData[z1 * 128 + x1];

                float brush0 = brush00 + (brush10 - brush00) * (brushX - x0);
                float brush1 = brush01 + (brush11 - brush01) * (brushX - x0);
                float brush = brush0 + (brush1 - brush0) * (brushZ - z0);

                int change = (int)(Strength * (brush * 1.2f - 0.2f) * 10000.0f);

                if (Randomizer.Int32(10000) < change)
                {
                    TreeInfo treeInfo = Container.GetVariation(ref Randomizer);

                    treePosition.y = Singleton<TerrainManager>.instance.SampleDetailHeight(treePosition, out float f, out float f2);
                    float spacing = Options.AutoDensity ? treeInfo.m_generatedInfo.m_size.x / 2 : Density;
                    Randomizer tempRandomizer = Randomizer;
                    uint item = TreeManager.instance.m_trees.NextFreeItem(ref tempRandomizer);
                    Randomizer treeRandomizer = new Randomizer(item);
                    float scale = treeInfo.m_minScale + (float)treeRandomizer.Int32(10000u) * (treeInfo.m_maxScale - treeInfo.m_minScale) * 0.0001f;
                    float height = treeInfo.m_generatedInfo.m_size.y * scale;
                    float clearance = Tweaker.Clearance;
                    Vector2 treePosition2 = VectorUtils.XZ(treePosition);
                    Quad2 clearanceQuad = default(Quad2);
                    clearanceQuad.a = treePosition2 + new Vector2(-clearance, -clearance);
                    clearanceQuad.b = treePosition2 + new Vector2(-clearance, clearance);
                    clearanceQuad.c = treePosition2 + new Vector2(clearance, clearance);
                    clearanceQuad.d = treePosition2 + new Vector2(clearance, -clearance);
                    Quad2 spacingQuad = default(Quad2);
                    spacingQuad.a = treePosition2 + new Vector2(-spacing, -spacing);
                    spacingQuad.b = treePosition2 + new Vector2(-spacing, spacing);
                    spacingQuad.c = treePosition2 + new Vector2(spacing, spacing);
                    spacingQuad.d = treePosition2 + new Vector2(spacing, -spacing);
                    float minY = MousePosition.y;
                    float maxY = MousePosition.y + height;
                    ItemClass.CollisionType collisionType = ItemClass.CollisionType.Terrain;

                    if (PropManager.instance.OverlapQuad(clearanceQuad, minY, maxY, collisionType, 0, 0) && !AltDown) continue;
                    if (TreeManager.instance.OverlapQuad(spacingQuad, minY, maxY, collisionType, 0, 0) && !AltDown) continue;
                    if (NetManager.instance.OverlapQuad(clearanceQuad, minY, maxY, collisionType, treeInfo.m_class.m_layer, 0, 0, 0) && !AltDown) continue;
                    if (BuildingManager.instance.OverlapQuad(clearanceQuad, minY, maxY, collisionType, treeInfo.m_class.m_layer, 0, 0, 0) && !AltDown) continue;
                    if (TerrainManager.instance.HasWater(treePosition) && !AltDown) continue;
                    int noiseScale = Randomizer.Int32(16);
                    float str2Rnd = UnityEngine.Random.Range(0.0f, Tweaker.MaxRandomRange);
                    if (Mathf.PerlinNoise(treePosition.x * noiseScale, treePosition.y * noiseScale) > 0.5 && str2Rnd < Strength * Tweaker.StrengthMultiplier)
                    {
                        if (Singleton<TreeManager>.instance.CreateTree(out uint num25, ref Randomizer, treeInfo, treePosition, false)) { }
                    }
                }
            }
        }

        private void RemoveTreesImpl()
        {
            float size = Size;
            float brushRadius = size / 2;
            float cellSize = TreeManager.TREEGRID_CELL_SIZE;
            int resolution = TreeManager.TREEGRID_RESOLUTION;
            TreeInstance[] trees = TreeManager.instance.m_trees.m_buffer;
            uint[] treeGrid = TreeManager.instance.m_treeGrid;
            float strength = Strength;
            Vector3 position = MousePosition;

            int minX = Mathf.Max((int)((position.x - Size) / cellSize + resolution * 0.5f), 0);
            int minZ = Mathf.Max((int)((position.z - Size) / cellSize + resolution * 0.5f), 0);
            int maxX = Mathf.Min((int)((position.x + Size) / cellSize + resolution * 0.5f), resolution - 1);
            int maxZ = Mathf.Min((int)((position.z + Size) / cellSize + resolution * 0.5f), resolution - 1);

            for (int z = minZ; z <= maxZ; ++z)
            {
                for (int x = minX; x <= maxX; ++x)
                {
                    uint treeIndex = treeGrid[z * resolution + x];
                    while (treeIndex != 0)
                    {
                        uint next = trees[treeIndex].m_nextGridTree;
                        var noiseScale = Randomizer.Int32(Tweaker.NoiseScale);
                        var strengthToRandom = UnityEngine.Random.Range(0.0f, Tweaker.MaxRandomRange);
                        if ((SelectiveDelete &&  Mathf.PerlinNoise(position.x * noiseScale, position.y * noiseScale) > Tweaker.NoiseThreshold && strengthToRandom < UserMod.Settings.SelectedBrush.Options.Strength)
                        || !SelectiveDelete)
                        {
                            TreeInfo treeInfo = TreeManager.instance.m_trees.m_buffer[treeIndex].Info;
                            if ((ShiftDown && ForestBrush.Instance.BrushTool.TreeInfos.Contains(treeInfo)) || !ShiftDown)
                            {
                                Vector3 treePosition = TreeManager.instance.m_trees.m_buffer[treeIndex].Position;
                                Vector2 xzTreePosition = VectorUtils.XZ(treePosition);
                                Vector2 xzMousePosition = VectorUtils.XZ(MousePosition);
                                Quad3 quad = GetQuad();
                                Quad2 xzQuad = Quad2.XZ(quad);
                                if ((!SquareBrush && (xzMousePosition - xzTreePosition).sqrMagnitude <= brushRadius * brushRadius) || (SquareBrush && xzQuad.Intersect(xzTreePosition)))
                                {
                                    TreeManager.instance.ReleaseTree(treeIndex);
                                }
                            }
                        }
                        treeIndex = next;
                    }
                }
            }
        }

        private void RemoveTreesBitmapImpl()
        {
            float brushRadius = Size / 2;
            float cellSize = TreeManager.TREEGRID_CELL_SIZE;
            int resolution = TreeManager.TREEGRID_RESOLUTION;
            TreeInstance[] trees = TreeManager.instance.m_trees.m_buffer;
            uint[] treeGrid = TreeManager.instance.m_treeGrid;
            float strength = Strength;
            Vector3 position = MousePosition;

            int minX = Mathf.Max((int)((position.x - Size) / cellSize + resolution * 0.5f), 0);
            int minZ = Mathf.Max((int)((position.z - Size) / cellSize + resolution * 0.5f), 0);
            int maxX = Mathf.Min((int)((position.x + Size) / cellSize + resolution * 0.5f), resolution - 1);
            int maxZ = Mathf.Min((int)((position.z + Size) / cellSize + resolution * 0.5f), resolution - 1);

            for (int z = minZ; z <= maxZ; ++z)
            {
                for (int x = minX; x <= maxX; ++x)
                {
                    uint treeIndex = treeGrid[z * resolution + x];

                    while (treeIndex != 0)
                    {
                        uint next = trees[treeIndex].m_nextGridTree;

                        Vector3 treePosition = TreeManager.instance.m_trees.m_buffer[treeIndex].Position;

                        var distance = treePosition - position;
                        var distanceRotated = Quaternion.Euler(0, Angle, 0) * distance;

                        float brushZ = (distanceRotated.z + brushRadius) / Size * 128.0f - 0.5f;
                        int z0 = Mathf.Clamp(Mathf.FloorToInt(brushZ), 0, 127);
                        int z1 = Mathf.Clamp(Mathf.CeilToInt(brushZ), 0, 127);

                        float brushX = (distanceRotated.x + brushRadius) / Size * 128.0f - 0.5f;
                        int x0 = Mathf.Clamp(Mathf.FloorToInt(brushX), 0, 127);
                        int x1 = Mathf.Clamp(Mathf.CeilToInt(brushX), 0, 127);

                        float brush00 = BrushData[z0 * 128 + x0];
                        float brush10 = BrushData[z0 * 128 + x1];
                        float brush01 = BrushData[z1 * 128 + x0];
                        float brush11 = BrushData[z1 * 128 + x1];

                        float brush0 = brush00 + (brush10 - brush00) * (brushX - x0);
                        float brush1 = brush01 + (brush11 - brush01) * (brushX - x0);
                        float brush = brush0 + (brush1 - brush0) * (brushZ - z0);

                        int change = (int)(strength * (brush * 1.2f - 0.2f) * 10000.0f);

                        if (Randomizer.Int32(10000) < change)
                        {
                            var noiseScale = Randomizer.Int32(Tweaker.NoiseScale);
                            var strengthToRandom = UnityEngine.Random.Range(0.0f, Tweaker.MaxRandomRange);
                            TreeInfo treeInfo = TreeManager.instance.m_trees.m_buffer[treeIndex].Info;
                            if ((SelectiveDelete && ForestBrush.Instance.BrushTool.TreeInfos.Contains(treeInfo) && Mathf.PerlinNoise(position.x * noiseScale, position.y * noiseScale) > Tweaker.NoiseThreshold && strengthToRandom < UserMod.Settings.SelectedBrush.Options.Strength)
                            || !SelectiveDelete)
                            {
                                Vector2 xzTreePosition = VectorUtils.XZ(treePosition);
                                Vector2 xzMousePosition = VectorUtils.XZ(MousePosition);
                                if ((xzMousePosition - xzTreePosition).sqrMagnitude <= brushRadius * brushRadius) TreeManager.instance.ReleaseTree(treeIndex);
                            }
                        }
                        treeIndex = next;
                    }
                }
            }
        }

        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            if (!MouseRayValid || Container is null) return;

            switch (ToolMode)
            {
                case Mode.Bitmap: RenderBitmap(cameraInfo); break;
                case Mode.Geometric: RenderGeometric(cameraInfo); break;
            }
        }

        private void RenderBitmap(RenderManager.CameraInfo cameraInfo)
        {
            if (BrushTexture != null)
            {
                BrushMaterial.SetTexture(ID_BrushTex, BrushTexture);
                Vector4 position = MousePosition;
                position.w = Size;
                BrushMaterial.SetVector(this.ID_BrushWS, position);
                BrushMaterial.SetFloat(ID_Angle, Angle);
                Vector3 center = new Vector3(MousePosition.x, 512f, MousePosition.z);
                Vector3 size = new Vector3(Size, 1224f, Size);
                Bounds bounds = new Bounds(center, size);
                ToolManager instance = Singleton<ToolManager>.instance;
                instance.m_drawCallData.m_overlayCalls = instance.m_drawCallData.m_overlayCalls + 1;
                RenderManager.instance.OverlayEffect.DrawEffect(cameraInfo, BrushMaterial, 0, bounds);
            }
        }

        private void RenderGeometric(RenderManager.CameraInfo cameraInfo)
        {
            m_toolController.RenderColliding(cameraInfo, Color, Color, Color, Color, 0, 0);
            ToolManager instance = Singleton<ToolManager>.instance;
            instance.m_drawCallData.m_overlayCalls = instance.m_drawCallData.m_overlayCalls + 1;
            if (UserMod.Settings.SelectedBrush.Options.IsSquare)
                Singleton<RenderManager>.instance.OverlayEffect.DrawQuad(cameraInfo, Color, GetQuad(), MousePosition.y - 100f, MousePosition.y + 100f, false, true);
            else Singleton<RenderManager>.instance.OverlayEffect.DrawCircle(cameraInfo, Color, MousePosition, Size, MousePosition.y - 100f, MousePosition.y + 100f, false, true);
        }

        private Quad3 GetQuad()
        {
            float size = Size / 2;
            var radians = Angle * Mathf.Deg2Rad;
            var cos = Mathf.Cos(radians);
            var sin = Mathf.Sin(radians);

            Quad3 quad = default(Quad3);
            Vector2 xz = VectorUtils.XZ(MousePosition);

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
            return quad;
        }

        private void Rotate45()
        {
            Angle = Mathf.Round(Angle / 45f - 1f) * 45f;
            ClampAngle();
        }

        private void ClampAngle()
        {
            if (Angle < 0f)
            {
                Angle += 360f;
            }
            if (Angle >= 360f)
            {
                Angle -= 360f;
            }
        }

        private  void DeltaAngle(float delta)
        {
            Angle += delta;
            ClampAngle();
        }
    }
}
