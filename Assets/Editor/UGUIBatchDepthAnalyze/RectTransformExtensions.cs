using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StaticUITool {
    //扩展工具  对比两个UI是否相交，重叠
    public static class RectTransformExtensions
    {
        public static bool Overlaps(this RectTransform a, RectTransform b)
        {
            //return a.rect.Overlaps(b.rect);
            return IsRectTransformOverlap(a, b);
        }
        public static bool Overlaps(this RectTransform a, RectTransform b, bool allowInverse)
        {
            //return a.rect.Overlaps(b.rect, allowInverse);
            return IsRectTransformOverlap(a, b);
        }
        public static bool IsRectTransformOverlap(RectTransform rect1, RectTransform rect2)
        {
            float rect1MinX = rect1.position.x - rect1.rect.width / 2;
            float rect1MaxX = rect1.position.x + rect1.rect.width / 2;
            float rect1MinY = rect1.position.y - rect1.rect.height / 2;
            float rect1MaxY = rect1.position.y + rect1.rect.height / 2;

            float rect2MinX = rect2.position.x - rect2.rect.width / 2;
            float rect2MaxX = rect2.position.x + rect2.rect.width / 2;
            float rect2MinY = rect2.position.y - rect2.rect.height / 2;
            float rect2MaxY = rect2.position.y + rect2.rect.height / 2;

            bool xNotOverlap = rect1MaxX <= rect2MinX || rect2MaxX <= rect1MinX;
            bool yNotOverlap = rect1MaxY <= rect2MinY || rect2MaxY <= rect1MinY;

            bool notOverlap = xNotOverlap || yNotOverlap;
            return !notOverlap;
        }
    }
}

