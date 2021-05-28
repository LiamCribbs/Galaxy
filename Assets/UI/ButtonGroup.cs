using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pigeon
{
    public class ButtonGroup : MonoBehaviour
    {
        public int selectedIndex;

        public OutlineThicknessButton[] buttons;

        public OutlineGraphic selectionGraphic;
        public float selectionMoveSpeed = 2f;
        const float SelectionGraphicOutlineThickness = 12f;
        IEnumerator moveSelectionGraphic;

        void Awake()
        {
            SelectButton(selectedIndex);
        }

        public void SelectButton(int index)
        {
            selectedIndex = index;

            if (moveSelectionGraphic != null)
            {
                StopCoroutine(moveSelectionGraphic);
            }

            moveSelectionGraphic = MoveSelectionGraphic();
            StartCoroutine(moveSelectionGraphic);
        }

        IEnumerator MoveSelectionGraphic()
        {
            Vector2 startPosition = selectionGraphic.rectTransform.anchoredPosition;
            Vector2 targetPosition = buttons[selectedIndex].mainGraphic.rectTransform.anchoredPosition;
            targetPosition.x -= SelectionGraphicOutlineThickness;
            targetPosition.y += SelectionGraphicOutlineThickness;

            float time = 0f;

            while (time < 1f)
            {
                time += selectionMoveSpeed * Time.unscaledDeltaTime;
                if (time > 1f)
                {
                    time = 1f;
                }

                selectionGraphic.rectTransform.anchoredPosition = Vector2.LerpUnclamped(startPosition, targetPosition, EaseFunctions.EaseOutQuintic(time));
                selectionGraphic.SetValue(Mathf.LerpUnclamped(SelectionGraphicOutlineThickness, SelectionGraphicOutlineThickness * 0.5f, EaseFunctions.BellCurveQuadratic(time)));

                yield return null;
            }

            moveSelectionGraphic = null;
        }

        [ContextMenu("Add Child Buttons")]
        public void AddChildButtons()
        {
            buttons = transform.GetComponentsInChildren<OutlineThicknessButton>();
        }
    }
}
