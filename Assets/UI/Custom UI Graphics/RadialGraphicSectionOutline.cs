using UnityEngine;
using UnityEngine.UI;

namespace Pigeon
{
    public class RadialGraphicSectionOutline : Graphic
    {
        [Min(3)] public float angle;
        public float thickness;
        public Color[] colors;

        public float sx, sy, sx1, sy1;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            Vector2 center = rectTransform.rect.center;
            float radius = rectTransform.rect.width * 0.5f;

            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = color;

            float angle = this.angle * Mathf.Deg2Rad;

            float x = Mathf.Sin(angle);
            float y = Mathf.Cos(angle);

            float x1 = Mathf.Sin(angle * 0.5f);
            float y1 = Mathf.Cos(angle * 0.5f);

            sx = x;
            sy = y;
            sx1 = x1;
            sy1 = y1;

            vertex.color = colors[0];
            vertex.position = center;
            vh.AddVert(vertex);

            vertex.color = colors[1];
            vertex.position = center + new Vector2(0f, radius);
            vh.AddVert(vertex);

            vertex.color = colors[2];
            vertex.position = center + new Vector2(radius * x, radius * y);
            vh.AddVert(vertex);

            float slopeX = radius * x;
            float slopeY = radius * y - radius;

            vertex.color = colors[3];
            vertex.position = center + new Vector2(thickness * x1, thickness * y1);
            vh.AddVert(vertex);

            vertex.color = colors[4];
            vertex.position = center + new Vector2(thickness * x, radius + (thickness * y) - (thickness * y1));
            vh.AddVert(vertex);

            vertex.color = colors[5];
            vertex.position = center + new Vector2((radius - thickness) * x, radius * y);
            vh.AddVert(vertex);

            //vh.AddTriangle(0, 1, 4);
            //vh.AddTriangle(4, 3, 0);
            //vh.AddTriangle(1, 2, 5);
            //vh.AddTriangle(5, 4, 1);
            //vh.AddTriangle(2, 0, 3);
            //vh.AddTriangle(3, 5, 2);
            vh.AddTriangle(3, 4, 5);
        }
    }
}