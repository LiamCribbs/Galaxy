using UnityEngine;
using UnityEngine.Events;

namespace Pigeon
{
    public class Slider : Button
    {
        public RectTransform rectTransform;
        public Canvas canvas;
        public RectTransform handle;

        public float defaultValue;

        public float minPosition, maxPosition;
        public float minValue, maxValue;

        public TMPro.TextMeshProUGUI valueText;
        public bool roundTextToInt;

        [System.Serializable]
        public class ValueChangedEvent : UnityEvent<float> { }
        public ValueChangedEvent OnValueChanged;

        public float Value
        {
            get
            {
                return Mathf.Lerp(minValue, maxValue, Mathf.InverseLerp(minPosition, maxPosition, handle.anchoredPosition.x));
            }
            set
            {
                handle.anchoredPosition = new Vector2(Mathf.Lerp(minPosition, maxPosition, Mathf.InverseLerp(minValue, maxValue, value)), handle.anchoredPosition.y);
                SetValueText();
            }
        }

        /// <summary>
        /// Set Value from a 0 to 1 value
        /// </summary>
        public void SetValueNormalized(float value)
        {
            handle.anchoredPosition = new Vector2(Mathf.Lerp(minPosition, maxPosition, value), handle.anchoredPosition.y);
            SetValueText();
        }

        public override void Awake()
        {
            base.Awake();

            Value = defaultValue;
        }

        public void SetValueText()
        {
            valueText.text = (roundTextToInt ? Mathf.RoundToInt(Value) : Value).ToString();
        }

        void Update()
        {
            if (clicking)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, canvas.worldCamera, out Vector2 mousePosition);

                if (mousePosition.x < minPosition)
                {
                    mousePosition.x = minPosition;
                }
                else if (mousePosition.x > maxPosition)
                {
                    mousePosition.x = maxPosition;
                }

                if (mousePosition.x == handle.anchoredPosition.x)
                {
                    return;
                }

                handle.anchoredPosition = new Vector2(mousePosition.x, handle.anchoredPosition.y);

                OnValueChanged?.Invoke(Value);
                SetValueText();
            }
        }
    } 
}