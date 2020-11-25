using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.ootii.Geometry
{
    public static class RectExt 
    {
        /// <summary>
        /// Add padding to a Rect 
        /// </summary>
        /// <param name="rRect"></param>
        /// <param name="rHorizontal"></param>
        /// <param name="rVertical"></param>
        /// <returns></returns>
        public static Rect Pad(this Rect rRect, int rHorizontal, int rVertical)
        {
            return new Rect(
                rRect.x + rHorizontal,
                rRect.y + rVertical,
                rRect.width - (rHorizontal * 2),
                rRect.height - (rVertical * 2));
        }

#if UNITY_EDITOR
        /// <summary>
        /// Build an array of Rect columns within the parent Rect
        /// </summary>
        /// <param name="rRect"></param>
        /// <param name="rColumnWidths"></param>
        /// <param name="rColumnPadding"></param>
        /// <returns></returns>
        public static Rect[] GetColumnRects(this Rect rRect, float[] rColumnWidths, float rColumnPadding)
        {
            var lColumnRects = new List<Rect>();
            float xPosition = rRect.x;
            for (int i = 0; i < rColumnWidths.Length; i++)
            {
                lColumnRects.Add(new Rect(xPosition, rRect.y, rColumnWidths[i], EditorGUIUtility.singleLineHeight));
                xPosition += (i < rColumnWidths.Length - 1) 
                    ? rColumnWidths[i] + rColumnPadding 
                    : rColumnWidths[i];
            }

            return lColumnRects.ToArray();
        }
#endif
    }
}

