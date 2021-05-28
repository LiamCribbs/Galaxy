using UnityEngine;
using UnityEngine.UI;

namespace Pigeon
{
    public class Line : Graphic
    {
        public Vector2 leftPosition;
        public Vector2 rightPosition;
        public float thickness;

        public void SetPositions(Vector3 leftPosition, Vector3 rightPosition)
        {
            this.leftPosition = leftPosition;
            this.rightPosition = rightPosition;
            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            //float minX = rectTransform.rect.xMin;
            //float maxX = rectTransform.rect.xMax;
            //float minY = rectTransform.rect.yMin;
            //float maxY = rectTransform.rect.yMax;

            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = color;

            vertex.position = leftPosition;
            vh.AddVert(vertex);

            vertex.position = new Vector2(leftPosition.x, leftPosition.y + thickness);
            vh.AddVert(vertex);

            vertex.position = new Vector2(rightPosition.x, rightPosition.y + thickness);
            vh.AddVert(vertex);

            vertex.position = rightPosition;
            vh.AddVert(vertex);

            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 3, 0);
        }
    }
}