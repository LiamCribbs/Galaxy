using UnityEngine;
using UnityEditor;
using static Pigeon.EditorSpawnUtility;

namespace Pigeon
{
    public static class CustomGraphicsSpawnUtility
    {
        [MenuItem("GameObject/UI/Graphics/Polygon")]
        public static void CreatePolygon(MenuCommand menuCommand)
        {
            CreateObject<Polygon>(menuCommand, "Polygon");
        }

        [MenuItem("GameObject/UI/Graphics/Polygon Outline")]
        public static void CreatePolygonOutline(MenuCommand menuCommand)
        {
            CreateObject<PolygonOutline>(menuCommand, "Polygon Outline");
        }

        [MenuItem("GameObject/UI/Graphics/Rect")]
        public static void CreateRect(MenuCommand menuCommand)
        {
            CreateObject<Rect>(menuCommand, "Rect");
        }

        [MenuItem("GameObject/UI/Graphics/Rect Gradient")]
        public static void CreateRectGradient(MenuCommand menuCommand)
        {
            CreateObject<RectGradient>(menuCommand, "Rect Gradient");
        }

        [MenuItem("GameObject/UI/Graphics/Rect Outline")]
        public static void CreateRectOutline(MenuCommand menuCommand)
        {
            CreateObject<RectOutline>(menuCommand, "Rect Outline");
        }
    }
}