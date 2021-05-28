using UnityEngine;
using UnityEditor;

namespace Pigeon
{
    public static class EditorSpawnUtility
    {
        public static T CreateObject<T>(MenuCommand menuCommand, string name) where T : Component
        {
            GameObject go = new GameObject(name);
            go.AddComponent<CanvasRenderer>();
            T component = go.AddComponent<T>();
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + name);
            Selection.activeObject = go;
            return component;
        }
    }
}