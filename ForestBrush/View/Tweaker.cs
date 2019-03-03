using ColossalFramework;
using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForestBrush
{
    public partial class ForestTool : ToolBase
    {
        public class Tweaks
        {
            public int SizeAddend;
            public int SizeMultiplier;
            public uint NoiseScale;
            public float NoiseThreshold;
            public float MaxRandomRange;
            public float Clearance;
            public float StrengthMultiplier;
            public float _maxSize;
            public float MaxSize
            {
                get
                {
                    return _maxSize;
                }
                set
                {
                    _maxSize = value;
                    ForestBrush.Instance.ForestBrushPanel.BrushOptionsSection.sizeSlider.maxValue = value;
                }
            }
        }

        public Tweaks Tweaker = new Tweaks()
        {
            SizeAddend = 10,
            SizeMultiplier = 7,
            NoiseScale = 16,
            NoiseThreshold = 0.5f,
            MaxRandomRange = 4f,
            Clearance = 4.5f,
            StrengthMultiplier = 1,
            _maxSize = 1000f
        };
    }
}
