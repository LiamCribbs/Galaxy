using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HoverInfo : MonoBehaviour
{
    public static HoverInfo instance;

    new RectTransform transform;
    public Pigeon.RectOutline outline;
    float defaultOutlineThickness;

    public RectTransform canvas;

    public Vector3 positionOffset;
    Vector2 defaultPosition;
    Vector2 targetPosition;
    float defaultHeight;
    public float moveSpeed;
    public float sizeSpeed;

    [Space(20)]
    public TextMeshProUGUI starNameText;
    public TextMeshProUGUI starMassText;
    public TextMeshProUGUI starRadiusText;
    public TextMeshProUGUI starClassText;

    public Star star;

    IEnumerator resizeCoroutine;

    public void SetHoverInfo(Star star)
    {
        this.star = star;

        starNameText.text = star.name;
        starMassText.text = star.mass.ToString("0.0") + " M";
        starRadiusText.text = star.radius.ToString("0.0") + "R";
        starClassText.text = star.spectralClass.type.ToString();
    }

    void Awake()
    {
        instance = this;

        transform = (RectTransform)(base.transform);
        defaultPosition = transform.anchoredPosition;
        targetPosition = defaultPosition;
        defaultHeight = transform.sizeDelta.y;
        defaultOutlineThickness = outline.thickness;

        resizeCoroutine = Resize(false);
        StartCoroutine(resizeCoroutine);
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(CameraController.instance.camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0f);
        if (hit.collider && hit.collider.TryGetComponent(out Star hitStar))
        {
            if (!star)
            {
                if (resizeCoroutine != null)
                {
                    StopCoroutine(resizeCoroutine);
                }

                resizeCoroutine = Resize(true);
                StartCoroutine(resizeCoroutine);
            }

            SetHoverInfo(hitStar);
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            star = null;
            if (resizeCoroutine != null)
            {
                StopCoroutine(resizeCoroutine);
            }

            resizeCoroutine = Resize(false);
            StartCoroutine(resizeCoroutine);
        }

        if (star)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, RectTransformUtility.WorldToScreenPoint(CameraController.instance.camera, star.transform.position + positionOffset), CameraController.instance.camera, out targetPosition);
        }

        transform.anchoredPosition = Vector2.Lerp(transform.anchoredPosition, targetPosition, moveSpeed * Time.unscaledDeltaTime);
    }

    IEnumerator Resize(bool active)
    {
        float startHeight = transform.sizeDelta.y;
        float startOutlineThickness = outline.thickness;
        float time = 0f;

        while (time < 1f)
        {
            time += sizeSpeed * Time.unscaledDeltaTime;
            if (time > 1f)
            {
                time = 1f;
            }

            float t = Pigeon.EaseFunctions.EaseOutQuintic(time);
            transform.sizeDelta = new Vector2(transform.sizeDelta.x, Mathf.LerpUnclamped(startHeight, active ? defaultHeight : 0f, t));
            outline.SetValue(Mathf.LerpUnclamped(startOutlineThickness, active ? defaultOutlineThickness : 0f, t));

            yield return null;
        }

        resizeCoroutine = null;
    }
}