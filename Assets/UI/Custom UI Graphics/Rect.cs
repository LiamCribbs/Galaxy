using UnityEngine;
using UnityEngine.UI;

namespace Pigeon
{
    public class Rect : Graphic
    {
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            float minX = rectTransform.rect.xMin;
            float maxX = rectTransform.rect.xMax;
            float minY = rectTransform.rect.yMin;
            float maxY = rectTransform.rect.yMax;

            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = color;

            vertex.position = new Vector3(minX, minY);
            vh.AddVert(vertex);

            vertex.position = new Vector3(minX, maxY);
            vh.AddVert(vertex);

            vertex.position = new Vector3(maxX, maxY);
            vh.AddVert(vertex);

            vertex.position = new Vector3(maxX, minY);
            vh.AddVert(vertex);

            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 3, 0);
        }
    }
}