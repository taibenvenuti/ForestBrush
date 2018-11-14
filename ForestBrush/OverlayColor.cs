using System;
using UnityEngine;

namespace ForestBrush
{
    [Serializable]
    public struct OverlayColor
    {
        public byte r;
        public byte g; 
        public byte b;
        public byte a;

        public OverlayColor(Color32 color)
        {
            r = color.r;
            g = color.g;
            b = color.b;
            a = color.a;
        }

        public static implicit operator Color32(OverlayColor color)
        {
            return new Color32(color.r, color.g, color.b, color.a);
        }

        public static implicit operator OverlayColor(Color32 color)
        {
            return new OverlayColor(color);
        }

        public static implicit operator Color(OverlayColor color)
        {
            return new Color32(color.r, color.g, color.b, color.a);
        }

        public static implicit operator OverlayColor(Color color)
        {
            return new OverlayColor(color);
        }
    }
}
