using UnityEngine;
using TMPro;
using UnityEngine.Events;
using Pigeon;

[AddComponentMenu("UI/Custom/Easy Slider", 0)]
public class EasySlider : MonoBehaviour
{
    public RectTransform sliderGraphic;
    protected Button button;
    public float maxWidth;

    public TMP_InputField inputText;

    [Space(10)]
    public float minValue;
    public float maxValue;
    public float defaultValue;
    public float Range
    {
        get => maxValue - minValue;
    }
    public string inputFormat = "0.0";

    public UnityEvent<float> OnValueChanged;

    public virtual float Value
    {
        get
        {
            return sliderGraphic.rect.width / maxWidth;
        }
        set
        {
            sliderGraphic.sizeDelta = new Vector2((value - minValue) / Range * maxWidth, sliderGraphic.rect.height);
            SetInputField();
        }
    }

    public float RandomFloat()
    {
        return Random.Range(minValue, maxValue);
    }

    public int RandomInt()
    {
        return Random.Range((int)minValue, (int)maxValue);
    }

    public virtual float GetMappedValue()
    {
        return (sliderGraphic.rect.width / maxWidth * Range) + minValue;
    }

    protected virtual void Awake()
    {
        button = GetComponent<Button>();
        //maxWidth = sliderGraphic.rect.width;
        ///range = maxValue - minValue;

        //Value = defaultValue;
        //if (inputText)
        //{
        //    inputText.text = defaultValue.ToString(inputFormat);
        //}

        SetToDefault();
    }

    //protected virtual void Start()
    //{
    //    SetToDefault();
    //    print(0);
    //}

    protected virtual void Update()
    {
        if (button.clicking)
        {
            Vector2 mousePosition = Input.mousePosition;
            if (mousePosition.x < 0f)
            {
                mousePosition.x = 0f;
            }
            else if (mousePosition.x > maxWidth)
            {
                mousePosition.x = maxWidth;
            }

            //if (inputText)
            //{
            //    SetInputField();
            //}
            SetInputField();

            sliderGraphic.sizeDelta = new Vector2(mousePosition.x, sliderGraphic.rect.height);
            OnValueChanged?.Invoke(Value);
        }
    }

    public virtual void SetInputField()
    {
        inputText.text = GetMappedValue().ToString(inputFormat);
    }

    public virtual void OnInputFieldSubmit()
    {
        if (!float.TryParse(inputText.text, out float value))
        {
            inputText.text = Value.ToString(inputFormat);
            return;
        }

        Value = value < minValue ? minValue : value > maxValue ? maxValue : value;

        //SetInputField();
        OnValueChanged?.Invoke(Value);
    }

    public virtual void SetToDefault()
    {
        Value = defaultValue;
        OnValueChanged?.Invoke(Value);
    }
}