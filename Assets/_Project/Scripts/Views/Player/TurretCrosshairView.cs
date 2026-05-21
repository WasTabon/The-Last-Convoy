using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TurretCrosshairView : MonoBehaviour
{
    [SerializeField] private Transform _aimPoint;

    private TurretWeaponConfig _config;
    private Camera _camera;
    private bool _isInjected;

    private Canvas _canvas;
    private RectTransform _crosshairContainer;
    private Image _topLine;
    private Image _bottomLine;
    private Image _leftLine;
    private Image _rightLine;

    [Inject]
    public void Construct(TurretWeaponConfig config, Camera camera)
    {
        _config = config;
        _camera = camera;
        _isInjected = true;
    }

    private void Start()
    {
        if (!_isInjected) return;

        SetupCanvas();
        CreateCrosshair();
    }

    private void LateUpdate()
    {
        if (!_isInjected) return;

        UpdateCrosshairPosition();
    }

    private void SetupCanvas()
    {
        GameObject canvasObj = new GameObject("TurretCrosshairCanvas");
        canvasObj.transform.SetParent(transform);

        _canvas = canvasObj.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<GraphicRaycaster>();
    }

    private void CreateCrosshair()
    {
        RectTransform canvasRect = _canvas.GetComponent<RectTransform>();

        GameObject container = new GameObject("Crosshair");
        _crosshairContainer = container.AddComponent<RectTransform>();
        _crosshairContainer.SetParent(canvasRect);
        _crosshairContainer.anchorMin = Vector2.zero;
        _crosshairContainer.anchorMax = Vector2.zero;
        _crosshairContainer.pivot = new Vector2(0.5f, 0.5f);
        _crosshairContainer.sizeDelta = new Vector2(100, 100);

        _topLine = CreateLine("TopLine", _crosshairContainer);
        _bottomLine = CreateLine("BottomLine", _crosshairContainer);
        _leftLine = CreateLine("LeftLine", _crosshairContainer);
        _rightLine = CreateLine("RightLine", _crosshairContainer);

        UpdateCrosshairAppearance();
    }

    private Image CreateLine(string name, RectTransform parent)
    {
        GameObject lineObj = new GameObject(name);
        RectTransform rect = lineObj.AddComponent<RectTransform>();
        rect.SetParent(parent);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        Image image = lineObj.AddComponent<Image>();
        image.color = _config.CrosshairColor;

        return image;
    }

    private void UpdateCrosshairPosition()
    {
        if (_camera == null) return;
        if (_crosshairContainer == null) return;

        Vector3 aimWorldPosition;

        if (_aimPoint != null)
        {
            Ray ray = new Ray(_aimPoint.position, _aimPoint.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, _config.RaycastRange))
            {
                aimWorldPosition = hit.point;
            }
            else
            {
                aimWorldPosition = _aimPoint.position + _aimPoint.forward * _config.RaycastRange;
            }
        }
        else
        {
            aimWorldPosition = _camera.transform.position + _camera.transform.forward * 100f;
        }

        Vector3 screenPos = _camera.WorldToScreenPoint(aimWorldPosition);

        if (screenPos.z > 0)
        {
            _crosshairContainer.anchoredPosition = new Vector2(screenPos.x, screenPos.y);
            SetCrosshairVisible(true);
        }
        else
        {
            SetCrosshairVisible(false);
        }
    }

    private void SetCrosshairVisible(bool visible)
    {
        if (_topLine != null) _topLine.enabled = visible;
        if (_bottomLine != null) _bottomLine.enabled = visible;
        if (_leftLine != null) _leftLine.enabled = visible;
        if (_rightLine != null) _rightLine.enabled = visible;
    }

    private void UpdateCrosshairAppearance()
    {
        float size = _config.CrosshairSize;
        float thickness = _config.CrosshairThickness;
        float gap = _config.CrosshairGap;

        _topLine.rectTransform.sizeDelta = new Vector2(thickness, size);
        _topLine.rectTransform.anchoredPosition = new Vector2(0, gap + size / 2f);

        _bottomLine.rectTransform.sizeDelta = new Vector2(thickness, size);
        _bottomLine.rectTransform.anchoredPosition = new Vector2(0, -(gap + size / 2f));

        _leftLine.rectTransform.sizeDelta = new Vector2(size, thickness);
        _leftLine.rectTransform.anchoredPosition = new Vector2(-(gap + size / 2f), 0);

        _rightLine.rectTransform.sizeDelta = new Vector2(size, thickness);
        _rightLine.rectTransform.anchoredPosition = new Vector2(gap + size / 2f, 0);

        _topLine.color = _config.CrosshairColor;
        _bottomLine.color = _config.CrosshairColor;
        _leftLine.color = _config.CrosshairColor;
        _rightLine.color = _config.CrosshairColor;
    }

    public void SetVisible(bool visible)
    {
        if (_canvas != null)
        {
            _canvas.enabled = visible;
        }
    }
}
