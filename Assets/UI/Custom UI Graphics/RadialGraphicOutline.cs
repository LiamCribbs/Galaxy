using UnityEngine;
using UnityEngine.UI;

namespace Pigeon
{
    public class RadialGraphicOutline : Graphic
    {
        [Min(3)] public int sections;
        public float thickness;
        public float thickness1;
        public float thickness2;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            Vector2 center = rectTransform.rect.center;
            float radius = rectTransform.rect.width * 0.5f;

            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = color;

            float angle = 360f / sections * Mathf.Deg2Rad;

            int vertIndex = 0;

            for (int i = 0; i < sections; i++)
            {
                vertex.position = center + new Vector2(thickness2 * Mathf.Sin(angle * (i + i + 1) * 0.5f), thickness2 * Mathf.Cos(angle * (i + i + 1) * 0.5f));
                vh.AddVert(vertex);

                vertex.position = center + new Vector2((radius - thickness1) * Mathf.Sin(angle * i + thickness * Mathf.Deg2Rad), (radius - thickness1) * Mathf.Cos(angle * i + thickness * Mathf.Deg2Rad));
                vh.AddVert(vertex);
                
                vertex.position = center + new Vector2((radius - thickness1) * Mathf.Sin(angle * (i + 1) - thickness * Mathf.Deg2Rad), (radius - thickness1) * Mathf.Cos(angle * (i + 1) - thickness * Mathf.Deg2Rad));
                vh.AddVert(vertex);

                vh.AddTriangle(vertIndex, vertIndex + 1, vertIndex + 2);
                vertIndex += 3;
            }
        }
    }
}