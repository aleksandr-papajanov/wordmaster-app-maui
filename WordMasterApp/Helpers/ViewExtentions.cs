using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordMasterApp.Helpers
{
    public static class ViewExtentions
    {
        public static Color ColorLerp(this Color start, Color end, float t)
        {
            t = Math.Clamp(t, 0f, 1f);

            float r = start.Red + (end.Red - start.Red) * t;
            float g = start.Green + (end.Green - start.Green) * t;
            float b = start.Blue + (end.Blue - start.Blue) * t;
            float a = start.Alpha + (end.Alpha - start.Alpha) * t;

            return new Color(r, g, b, a);
        }
    }
}
