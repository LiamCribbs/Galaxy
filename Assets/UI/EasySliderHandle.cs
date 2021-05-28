using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasySliderHandle : EasySlider
{
    public RectTransform handle;
    public bool lockY = true;
    public float defaultValueY;

    public override float Value
    {
        get
        {
            return handle.anchoredPosition.x / sliderGraphic.sizeDelta.x;
        }
        set
        {
            handle.anchoredPosition = new Vector2((value - minValue) / Range * sliderGraphic.sizeDelta.x, handle.anchoredPosition.y);
            //SetInputField();
        }
    }

    public float ValueY
    {
        get
        {
            return handle.anchoredPosition.y / sliderGraphic.sizeDelta.y;
        }
        set
        {
            handle.anchoredPosition = new Vector2(handle.anchoredPosition.x, (value - minValue) / Range * sliderGraphic.sizeDelta.y);
            //SetInputField();
        }
    }

    protected override void Update()
    {
        if (button.clicking)
        {
            Vector2 mousePosition = Input.mousePosition;
            if (mousePosition.x < 0f)
            {
                mousePosition.x = 0f;
            }
            else if (mousePosition.x > sliderGraphic.sizeDelta.x)
            {
                mousePosition.x = sliderGraphic.sizeDelta.x;
            }
            if (!lockY)
            {
                if (mousePosition.y < 0f)
                {
                    mousePosition.y = 0f;
                }
                else if (mousePosition.y > sliderGraphic.sizeDelta.y)
                {
                    mousePosition.y = sliderGraphic.sizeDelta.y;
                }
            }

            handle.anchoredPosition = new Vector2(mousePosition.x, lockY ? handle.anchoredPosition.y : mousePosition.y);
            OnValueChanged?.Invoke(Value);
        }
    }

    public override void SetToDefault()
    {
        Value = defaultValue;
        if (!lockY)
        {
            ValueY = defaultValueY;
        }
        OnValueChanged?.Invoke(Value);
    }
}