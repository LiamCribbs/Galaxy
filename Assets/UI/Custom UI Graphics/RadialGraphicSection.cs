using UnityEngine;
using UnityEngine.UI;

namespace Pigeon
{
    public class RadialGraphicSection : Graphic
    {
        [Min(3)] public float angle;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            Vector2 center = rectTransform.rect.center;
            float radius = rectTransform.rect.width * 0.5f;

            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = color;

            float angle = this.angle * Mathf.Deg2Rad;

            vertex.position = center;
            vh.AddVert(vertex);

            vertex.position = center + new Vector2(0f, radius);
            vh.AddVert(vertex);

            vertex.position = center + new Vector2(radius * Mathf.Sin(angle), radius * Mathf.Cos(angle));
            vh.AddVert(vertex);

            vh.AddTriangle(0, 1, 2);
        }
    }
}