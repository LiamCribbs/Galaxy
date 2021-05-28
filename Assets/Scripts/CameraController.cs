using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 prevMousePosition;
    Vector3 mouseVelocity;
    public float scrollVelocity;

    public new Camera camera;
    public float moveSpeed;
    public float moveDecelerationSpeed = 9.81f;
    public AnimationCurve zoomPanCurve;

    [Space(10)]
    public float minZoom;
    public float maxZoom;
    public float zoomSpeed;
    public float zoomDecelerationSpeed = 6f;
    public AnimationCurve zoomSpeedCurve;

    float bounds;

    void Start()
    {
        bounds = Galaxy.instance.galaxySize * Galaxy.instance.armLength * 3.75f;
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

        camera.orthographicSize = size;

        // Pan
        if (Input.GetKey(KeyCode.Mouse1) || Input.GetKey(KeyCode.Mouse2))
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

        RaycastHit2D hit = Physics2D.Raycast(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0f);
        if (hit.collider && hit.collider.TryGetComponent(out Star star))
        {
            Galaxy.instance.starNameText.text = star.name;
        }

        prevMousePosition = Input.mousePosition;
    }
}