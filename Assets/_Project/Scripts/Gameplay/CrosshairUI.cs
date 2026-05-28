using UnityEngine;
using UnityEngine.UI;

public class CrosshairUI : MonoBehaviour
{
    [Header("Crosshair Settings")]
    [SerializeField] private Color crosshairColor = Color.white;
    [SerializeField] private float lineThickness = 2f;
    [SerializeField] private float lineLength = 20f;
    [SerializeField] private float centerGap = 10f;
    
    [Header("Dynamic Spread")]
    [SerializeField] private bool enableDynamicSpread = true;
    [SerializeField] private float spreadAmount = 0f;
    [SerializeField] private float maxSpread = 30f;
    [SerializeField] private float spreadSpeed = 5f;
    
    [Header("References")]
    [SerializeField] private MinigunController minigunController;
    
    private Canvas canvas;
    private RectTransform canvasRect;
    
    private Image topLine;
    private Image bottomLine;
    private Image leftLine;
    private Image rightLine;
    
    private float currentSpread = 0f;
    private float targetSpread = 0f;

    void Awake()
    {
        SetupCanvas();
        CreateCrosshair();
    }

    void Update()
    {
        UpdateDynamicSpread();
        UpdateCrosshairPosition();
    }

    void SetupCanvas()
    {
        GameObject canvasObj = new GameObject("CrosshairCanvas");
        canvasObj.transform.SetParent(transform);
        
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        canvasRect = canvasObj.GetComponent<RectTransform>();
    }

    void CreateCrosshair()
    {
        GameObject crosshairContainer = new GameObject("Crosshair");
        RectTransform containerRect = crosshairContainer.AddComponent<RectTransform>();
        containerRect.SetParent(canvasRect);
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.pivot = new Vector2(0.5f, 0.5f);
        containerRect.anchoredPosition = Vector2.zero;
        containerRect.sizeDelta = new Vector2(100, 100);
        
        topLine = CreateLine("TopLine", containerRect);
        bottomLine = CreateLine("BottomLine", containerRect);
        leftLine = CreateLine("LeftLine", containerRect);
        rightLine = CreateLine("RightLine", containerRect);
        
        UpdateCrosshairAppearance();
    }

    Image CreateLine(string name, RectTransform parent)
    {
        GameObject lineObj = new GameObject(name);
        RectTransform rect = lineObj.AddComponent<RectTransform>();
        rect.SetParent(parent);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        
        Image image = lineObj.AddComponent<Image>();
        image.color = crosshairColor;
        
        return image;
    }

    void UpdateCrosshairAppearance()
    {
        topLine.rectTransform.sizeDelta = new Vector2(lineThickness, lineLength);
        topLine.rectTransform.anchoredPosition = new Vector2(0, centerGap + lineLength / 2f + currentSpread);
        
        bottomLine.rectTransform.sizeDelta = new Vector2(lineThickness, lineLength);
        bottomLine.rectTransform.anchoredPosition = new Vector2(0, -(centerGap + lineLength / 2f + currentSpread));
        
        leftLine.rectTransform.sizeDelta = new Vector2(lineLength, lineThickness);
        leftLine.rectTransform.anchoredPosition = new Vector2(-(centerGap + lineLength / 2f + currentSpread), 0);
        
        rightLine.rectTransform.sizeDelta = new Vector2(lineLength, lineThickness);
        rightLine.rectTransform.anchoredPosition = new Vector2(centerGap + lineLength / 2f + currentSpread, 0);
        
        topLine.color = crosshairColor;
        bottomLine.color = crosshairColor;
        leftLine.color = crosshairColor;
        rightLine.color = crosshairColor;
    }

    void UpdateDynamicSpread()
    {
        if (!enableDynamicSpread) return;
        
        bool isFiring = false;
        if (minigunController != null)
        {
            isFiring = minigunController.IsFiring();
        }
        
        targetSpread = isFiring ? maxSpread : 0f;
        
        currentSpread = Mathf.Lerp(currentSpread, targetSpread, Time.deltaTime * spreadSpeed);
        
        UpdateCrosshairAppearance();
    }

    void UpdateCrosshairPosition()
    {
        if (!enableDynamicSpread)
        {
            currentSpread = spreadAmount;
            UpdateCrosshairAppearance();
        }
    }

    public void SetColor(Color color)
    {
        crosshairColor = color;
        UpdateCrosshairAppearance();
    }

    public void SetThickness(float thickness)
    {
        lineThickness = thickness;
        UpdateCrosshairAppearance();
    }

    public void SetLength(float length)
    {
        lineLength = length;
        UpdateCrosshairAppearance();
    }

    public void SetGap(float gap)
    {
        centerGap = gap;
        UpdateCrosshairAppearance();
    }

    public void SetSpread(float spread)
    {
        spreadAmount = Mathf.Clamp(spread, 0f, maxSpread);
        if (!enableDynamicSpread)
        {
            currentSpread = spreadAmount;
            UpdateCrosshairAppearance();
        }
    }

    public void SetVisible(bool visible)
    {
        if (canvas != null)
        {
            canvas.enabled = visible;
        }
    }

    void OnValidate()
    {
        lineThickness = Mathf.Max(1f, lineThickness);
        lineLength = Mathf.Max(5f, lineLength);
        centerGap = Mathf.Max(0f, centerGap);
        spreadAmount = Mathf.Clamp(spreadAmount, 0f, maxSpread);
        maxSpread = Mathf.Max(0f, maxSpread);
    }
}