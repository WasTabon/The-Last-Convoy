using UnityEngine;
using UnityEngine.UI;
using Zenject;
using LastConvoy.Models;
using LastConvoy.Configs;

namespace LastConvoy.Views.UI
{
    public class CrosshairView : MonoBehaviour
    {
        private CrosshairModel _model;
        private CrosshairConfig _config;

        private Canvas _canvas;
        private Image _topLine;
        private Image _bottomLine;
        private Image _leftLine;
        private Image _rightLine;

        [Inject]
        public void Construct(CrosshairModel model, CrosshairConfig config)
        {
            _model = model;
            _config = config;
        }

        private void Awake()
        {
            SetupCanvas();
            CreateCrosshair();
        }

        private void OnEnable()
        {
            _model.OnSpreadChanged += HandleSpreadChanged;
        }

        private void OnDisable()
        {
            _model.OnSpreadChanged -= HandleSpreadChanged;
        }

        private void SetupCanvas()
        {
            GameObject canvasObj = new GameObject("CrosshairCanvas");
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

            GameObject crosshairContainer = new GameObject("Crosshair");
            RectTransform containerRect = crosshairContainer.AddComponent<RectTransform>();
            containerRect.SetParent(canvasRect);
            containerRect.anchorMin = new Vector2(0.5f, 0.5f);
            containerRect.anchorMax = new Vector2(0.5f, 0.5f);
            containerRect.pivot = new Vector2(0.5f, 0.5f);
            containerRect.anchoredPosition = Vector2.zero;
            containerRect.sizeDelta = new Vector2(100, 100);

            _topLine = CreateLine("TopLine", containerRect);
            _bottomLine = CreateLine("BottomLine", containerRect);
            _leftLine = CreateLine("LeftLine", containerRect);
            _rightLine = CreateLine("RightLine", containerRect);

            UpdateCrosshairAppearance(0f);
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

        private void HandleSpreadChanged(float spread)
        {
            UpdateCrosshairAppearance(spread);
        }

        private void UpdateCrosshairAppearance(float currentSpread)
        {
            float thickness = _config.LineThickness;
            float length = _config.LineLength;
            float gap = _config.CenterGap;

            _topLine.rectTransform.sizeDelta = new Vector2(thickness, length);
            _topLine.rectTransform.anchoredPosition = new Vector2(0, gap + length / 2f + currentSpread);

            _bottomLine.rectTransform.sizeDelta = new Vector2(thickness, length);
            _bottomLine.rectTransform.anchoredPosition = new Vector2(0, -(gap + length / 2f + currentSpread));

            _leftLine.rectTransform.sizeDelta = new Vector2(length, thickness);
            _leftLine.rectTransform.anchoredPosition = new Vector2(-(gap + length / 2f + currentSpread), 0);

            _rightLine.rectTransform.sizeDelta = new Vector2(length, thickness);
            _rightLine.rectTransform.anchoredPosition = new Vector2(gap + length / 2f + currentSpread, 0);

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
}
