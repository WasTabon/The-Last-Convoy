using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEditor;

public class SlideshowSetup
{
    [MenuItem("Tools/Slideshow/Setup Scene")]
    public static void SetupScene()
    {
        GameObject existingController = GameObject.Find("SlideshowController");
        if (existingController != null)
        {
            if (!EditorUtility.DisplayDialog("Slideshow Setup",
                "SlideshowController already exists in scene. Recreate?", "Yes", "Cancel"))
            {
                return;
            }
            Object.DestroyImmediate(existingController);

            GameObject existingCanvas = GameObject.Find("SlideshowCanvas");
            if (existingCanvas != null) Object.DestroyImmediate(existingCanvas);

            GameObject existingES = GameObject.Find("EventSystem");
            if (existingES != null) Object.DestroyImmediate(existingES);
        }

        GameObject canvasGO = new GameObject("SlideshowCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(canvasGO.transform, false);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = Color.black;
        StretchFull(bg.GetComponent<RectTransform>());

        Image imageA = CreateFullscreenImage(canvasGO.transform, "ImageA");
        Image imageB = CreateFullscreenImage(canvasGO.transform, "ImageB");
        Image finalImage = CreateFullscreenImage(canvasGO.transform, "FinalImage");
        finalImage.gameObject.SetActive(false);

        Image blackOverlay = CreateFullscreenImage(canvasGO.transform, "BlackOverlay");
        blackOverlay.color = new Color(0f, 0f, 0f, 0f);

        GameObject skipHintGO = new GameObject("SkipHint");
        skipHintGO.transform.SetParent(canvasGO.transform, false);
        TextMeshProUGUI skipHint = skipHintGO.AddComponent<TextMeshProUGUI>();
        skipHint.text = "Press Space To Skip";
        skipHint.fontSize = 28;
        skipHint.color = new Color(1f, 1f, 1f, 0.7f);
        skipHint.alignment = TextAlignmentOptions.BottomRight;
        skipHint.fontStyle = FontStyles.Normal;
        RectTransform skipRT = skipHintGO.GetComponent<RectTransform>();
        skipRT.anchorMin = new Vector2(1f, 0f);
        skipRT.anchorMax = new Vector2(1f, 0f);
        skipRT.pivot = new Vector2(1f, 0f);
        skipRT.anchoredPosition = new Vector2(-40f, 30f);
        skipRT.sizeDelta = new Vector2(400f, 60f);

        GameObject subtitleGO = new GameObject("SubtitleText");
        subtitleGO.transform.SetParent(canvasGO.transform, false);
        TextMeshProUGUI subtitle = subtitleGO.AddComponent<TextMeshProUGUI>();
        subtitle.text = "";
        subtitle.fontSize = 42;
        subtitle.color = new Color(1f, 1f, 1f, 1f);
        subtitle.alignment = TextAlignmentOptions.Bottom;
        subtitle.fontStyle = FontStyles.Normal;
        subtitle.enableWordWrapping = true;
        subtitle.outlineWidth = 0.2f;
        subtitle.outlineColor = Color.black;
        RectTransform subRT = subtitleGO.GetComponent<RectTransform>();
        subRT.anchorMin = new Vector2(0.5f, 0f);
        subRT.anchorMax = new Vector2(0.5f, 0f);
        subRT.pivot = new Vector2(0.5f, 0f);
        subRT.anchoredPosition = new Vector2(0f, 80f);
        subRT.sizeDelta = new Vector2(1400f, 200f);

        if (GameObject.FindObjectOfType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }

        GameObject controllerGO = new GameObject("SlideshowController");
        SlideshowController controller = controllerGO.AddComponent<SlideshowController>();

        GameObject mainAudioGO = new GameObject("MainAudioSource");
        mainAudioGO.transform.SetParent(controllerGO.transform, false);
        AudioSource mainAudio = mainAudioGO.AddComponent<AudioSource>();
        mainAudio.playOnAwake = false;

        GameObject finalAudioGO = new GameObject("FinalAudioSource");
        finalAudioGO.transform.SetParent(controllerGO.transform, false);
        AudioSource finalAudio = finalAudioGO.AddComponent<AudioSource>();
        finalAudio.playOnAwake = false;

        GameObject voAudioGO = new GameObject("VoiceoverAudioSource");
        voAudioGO.transform.SetParent(controllerGO.transform, false);
        AudioSource voAudio = voAudioGO.AddComponent<AudioSource>();
        voAudio.playOnAwake = false;

        controller.canvas = canvas;
        controller.imageA = imageA;
        controller.imageB = imageB;
        controller.finalImageDisplay = finalImage;
        controller.blackOverlay = blackOverlay;
        controller.mainAudioSource = mainAudio;
        controller.finalAudioSource = finalAudio;
        controller.voiceoverAudioSource = voAudio;
        controller.skipHintText = skipHint;
        controller.subtitleText = subtitle;

        Selection.activeGameObject = controllerGO;
        EditorUtility.SetDirty(controllerGO);

        Debug.Log("Slideshow scene setup complete. Assign 10 slide images, final image, and 2 audio clips on SlideshowController.");
    }

    private static Image CreateFullscreenImage(Transform parent, string name)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image img = go.AddComponent<Image>();
        img.color = new Color(1f, 1f, 1f, 0f);
        img.preserveAspect = true;
        RectTransform rt = go.GetComponent<RectTransform>();
        StretchFull(rt);
        return img;
    }

    private static void StretchFull(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}
