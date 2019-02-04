//using ColossalFramework;
//using ColossalFramework.Math;
//using ICities;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//namespace ForestBrush
//{
//    public class ForestTool : ToolBase
//    {
//        private TreeInfo Prefab { get; set; }

//        public ForestBrush Brush { get; private set; }

//        private Color32 OverlayColor { get; set; }

//        private float Desity { get; set; }

//        private float Scale { get; set; }

//        private bool Square { get; set; }

//        private Randomizer Randomizer { get; set; }

//        private bool Rotating { get; set; }

//        private bool MouseRightDown { get; set; }

//        private bool MouseLeftDown { get; set; }

//        private float Angle { get; set; }

//        private bool AngleChanged { get; set; }

//        private Vector3 MousePosition { get; set; }

//        protected override void Awake()
//        {
//            base.Awake();
//            Randomizer = new Randomizer(DateTime.Now.Ticks);
//        }

//        protected override void OnEnable()
//        {
//            base.OnEnable();
//        }

//        protected override void OnDisable()
//        {
//            base.OnDisable();
//        }

//        protected override void OnToolGUI(Event e)
//        {
//            base.OnToolGUI(e);
//        }

//        protected override void OnToolUpdate()
//        {
//            base.OnToolUpdate();
//        }

//        protected override void OnToolLateUpdate()
//        {
//            base.OnToolLateUpdate();
//        }

//        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
//        {
//            if (Prefab != null && !m_toolController.IsInsideUI && Cursor.visible)
//            {
//                var size = Scale / 2;
//                Color toolColor = ForestBrushMod.instance.BrushSettings.OverlayColor;
//                m_toolController.RenderColliding(cameraInfo, toolColor, toolColor, toolColor, toolColor, 0, 0);
//                ToolManager instance = Singleton<ToolManager>.instance;
//                instance.m_drawCallData.m_overlayCalls = instance.m_drawCallData.m_overlayCalls + 1;
//                if (ForestBrushMod.instance.Settings.SquareBrush)
//                {
//                    var Angle = ApplyBrushPatch.Angle;
//                    var radians = Angle * Mathf.Deg2Rad;
//                    var cos = Mathf.Cos(radians);
//                    var sin = Mathf.Sin(radians);

//                    Quad3 quad = default(Quad3);
//                    Vector2 xz = VectorUtils.XZ(MousePosition);

//                    var a = xz + new Vector2(-size, -size);
//                    var aT = a - xz;
//                    a.x = aT.x * cos - aT.y * sin;
//                    a.y = aT.y * cos + aT.x * sin;
//                    a += xz;


//                    var b = xz + new Vector2(-size, size);
//                    var bT = b - xz;
//                    b.x = bT.x * cos - bT.y * sin;
//                    b.y = bT.y * cos + bT.x * sin;
//                    b += xz;

//                    var c = xz + new Vector2(size, size);
//                    var cT = c - xz;
//                    c.x = cT.x * cos - cT.y * sin;
//                    c.y = cT.y * cos + cT.x * sin;
//                    c += xz;

//                    var d = xz + new Vector2(size, -size);
//                    var dT = d - xz;
//                    d.x = dT.x * cos - dT.y * sin;
//                    d.y = dT.y * cos + dT.x * sin;
//                    d += xz;

//                    quad.a = new Vector3(a.x, 0f, a.y);
//                    quad.b = new Vector3(b.x, 0f, b.y);
//                    quad.c = new Vector3(c.x, 0f, c.y);
//                    quad.d = new Vector3(d.x, 0f, d.y);
//                    Singleton<RenderManager>.instance.OverlayEffect.DrawQuad(cameraInfo, toolColor, quad, MousePosition.y - 100f, MousePosition.y + 100f, false, false);
//                }
//                else Singleton<RenderManager>.instance.OverlayEffect.DrawCircle(cameraInfo, toolColor, MousePosition, size * 2, MousePosition.y - 100f, MousePosition.y + 100f, false, true);
//            }
//        }

//        public override void SimulationStep()
//        {
//            base.SimulationStep();
//        }
//    }
//}
