using UnityEngine;
using UnityEngine.UI;

namespace Pigeon
{
    public class RadialGraphic : Graphic
    {
        [Min(3)] public int sections;

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
                vertex.position = center;
                vh.AddVert(vertex);

                vertex.position = center + new Vector2(radius * Mathf.Sin(angle * i), radius * Mathf.Cos(angle * i));
                vh.AddVert(vertex);

                vertex.position = center + new Vector2(radius * Mathf.Sin(angle * (i + 1)), radius * Mathf.Cos(angle * (i + 1)));
                vh.AddVert(vertex);

                vh.AddTriangle(vertIndex, vertIndex + 1, vertIndex + 2);
                vertIndex += 3;
            }
        }
    }
}