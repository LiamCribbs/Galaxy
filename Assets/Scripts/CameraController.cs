using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    public UnityEngine.EventSystems.EventSystem eventSystem;
    public UnityEngine.EventSystems.StandaloneInputModule input;

    Vector3 prevMousePosition;
    Vector3 mouseVelocity;
    public float scrollVelocity;

    public new Camera camera;
    public float moveSpeed;
    public float moveDecelerationSpeed = 9.81f;
    public AnimationCurve zoomPanCurve;

    bool canPan;

    [Space(10)]
    public float minZoom;
    public float maxZoom;
    public float zoomSpeed;
    public float zoomDecelerationSpeed = 6f;
    public AnimationCurve zoomSpeedCurve;
    public Pigeon.Slider zoomSlider;

    float bounds;

    [Space(10)]
    public Pigeon.Button settingsButton;
    public Transform settingsOpenGraphic;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        SetBounds();
    }

    public void SetBounds()
    {
        bounds = Galaxy.instance.galaxySize * Galaxy.instance.armLength * 3.75f;
    }

    public void ToggleSettings()
    {
        settingsOpenGraphic.localScale = new Vector3(settingsButton.hovering ? 0.9f : -0.9f, settingsOpenGraphic.localScale.y, 1f);
        settingsButton.SetHover(!settingsButton.hovering);
    }

    void Update()
    {
        // Zoom
        float scrollDelta = -Input.mouseScrollDelta.y;
        if (scrollDelta != 0f)
        {
            scrollVelocity = scrollDelta;
        }
        else
        {
            scrollVelocity = Mathf.Lerp(scrollVelocity, 0f, zoomDecelerationSpeed * Time.unscaledDeltaTime);
            if (Mathf.Abs(scrollVelocity) < 0.01f)
            {
                scrollVelocity = 0f;
            }
        }

        float maxZoom = this.maxZoom * bounds;
        float sizePercentage = Mathf.InverseLerp(minZoom, maxZoom, camera.orthographicSize);
        float speed = zoomSpeed * zoomSpeedCurve.Evaluate(sizePercentage);
        float size = camera.orthographicSize + scrollVelocity * speed;
        if (size > maxZoom)
        {
            size = maxZoom;
        }
        else if (size < minZoom)
        {
            size = minZoom;
        }

        zoomSlider.Value = sizePercentage;

        camera.orthographicSize = size;

        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1))
        {
            canPan = !eventSystem.IsPointerOverGameObject();
        }

        // Pan
        if (canPan && (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1)))
        {
            mouseVelocity = prevMousePosition - Input.mousePosition;
        }
        else
        {
            mouseVelocity = Vector3.Lerp(mouseVelocity, Vector3.zero, moveDecelerationSpeed * Time.unscaledDeltaTime);
            if (mouseVelocity.sqrMagnitude < 0.1f)
            {
                mouseVelocity = Vector3.zero;
            }
        }

        Vector3 position = camera.transform.position + mouseVelocity * moveSpeed * (size * zoomPanCurve.Evaluate(sizePercentage));
        size *= 1f - sizePercentage;
        if (position.y > bounds - size)
        {
            position.y = bounds - size;
        }
        else if (position.y < -bounds + size)
        {
            position.y = -bounds + size;
        }

        float horizontalSize = size * camera.aspect;
        if (position.x > bounds - horizontalSize)
        {
            position.x = bounds - horizontalSize;
        }
        else if (position.x < -bounds + horizontalSize)
        {
            position.x = -bounds + horizontalSize;
        }

        camera.transform.position = position;

        prevMousePosition = Input.mousePosition;
    }

    public void SetZoom(float value)
    {
        camera.orthographicSize = Mathf.Lerp(minZoom, maxZoom * bounds, value);
    }
}