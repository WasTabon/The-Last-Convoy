using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

[System.Serializable]
public class Subtitle
{
    public float startTime;
    [TextArea(1, 4)]
    public string text;
}

public class SlideshowController : MonoBehaviour
{
    [Header("Images")]
    public Sprite[] slideImages = new Sprite[10];
    public Sprite finalImage;

    [Header("Audio")]
    public AudioClip mainAudio;
    public AudioClip finalAudio;
    public AudioClip voiceoverAudio;

    [Header("Refs")]
    public Canvas canvas;
    public Image imageA;
    public Image imageB;
    public Image blackOverlay;
    public Image finalImageDisplay;
    public AudioSource mainAudioSource;
    public AudioSource finalAudioSource;
    public AudioSource voiceoverAudioSource;
    public TextMeshProUGUI skipHintText;
    public TextMeshProUGUI subtitleText;

    [Header("Timing")]
    public float perSlideDuration = 14f;
    public float crossfadeDuration = 1.5f;
    public float audioFadeInDuration = 2f;
    public float audioFadeOutDuration = 2f;
    public float blackTransitionDuration = 1.5f;
    public float pauseBeforeFinalImage = 5f;
    public float finalImageFlyDuration = 1.2f;
    public float finalSceneHoldDuration = 15f;
    public float finalFadeOutDuration = 2f;

    [Header("Final Image Animation")]
    public float finalImageStartScale = 0.05f;
    public float finalImageEndScale = 1f;
    public float finalImageStartYOffset = 800f;

    [Header("Audio Volumes")]
    [Range(0f, 1f)] public float mainAudioMaxVolume = 0.5f;
    [Range(0f, 1f)] public float finalAudioMaxVolume = 1f;
    [Range(0f, 1f)] public float voiceoverMaxVolume = 1f;

    [Header("Subtitles")]
    public float subtitleFadeDuration = 0.3f;
    public Subtitle[] subtitles;

    [Header("Next Scene")]
    public string nextSceneName = "";

    private int currentIndex = 0;
    private Image currentImage;
    private Image nextImage;
    private bool skipped = false;
    private float skipFadeDuration = 1f;
    private float subtitleStartTime;
    private int currentSubtitleIndex = 0;
    private Coroutine subtitleCoroutine;

    private void Reset()
    {
        subtitles = new Subtitle[]
        {
            new Subtitle { startTime = 0.08f, text = "In this world, the sun doesn't shine to give life. It shines so you can clearly see how badly things are about to go wrong." },
            new Subtitle { startTime = 10.16f, text = "The desert isn't a place. It's just what's left after everyone stopped caring." },
            new Subtitle { startTime = 17.06f, text = "And we are flying." },
            new Subtitle { startTime = 19.30f, text = "The helicopter screams like it already regrets being part of this story. Inside it — a man who decided that stealing an armored war machine was a \"quick job.\"" },
            new Subtitle { startTime = 31.70f, text = "Optimism. The most lethal substance in the wasteland." },
            new Subtitle { startTime = 36.41f, text = "They spot us immediately. Of course they do. Even the dust here has early warning systems." },
            new Subtitle { startTime = 44.50f, text = "The first helicopter goes down fast. Like it was just waiting for an excuse to retire." },
            new Subtitle { startTime = 50.50f, text = "The second follows right after — dramatically, like a failed career trajectory." },
            new Subtitle { startTime = 56.78f, text = "No one mourns. Emotional capacity is limited. Ammunition is not." },
            new Subtitle { startTime = 62.20f, text = "I grab the machine gun. It's warm. Like a handshake from someone who will absolutely try to kill you later." },
            new Subtitle { startTime = 70.52f, text = "Down below, raiders run around like no one told them this isn't a training simulation." },
            new Subtitle { startTime = 77.50f, text = "We don't explain." },
            new Subtitle { startTime = 78.30f, text = "We answer in bursts of fire." },
            new Subtitle { startTime = 80.28f, text = "One by one, they stop being problems and become scenery." },
            new Subtitle { startTime = 84.78f, text = "And then it hits me — the first rule of this desert: if it moves, it's either an enemy… or a bad idea. Either way, it gets the same solution." },
            new Subtitle { startTime = 97.00f, text = "Now it's just us." },
            new Subtitle { startTime = 98.50f, text = "One helicopter. One pilot. And a plan that wouldn't survive basic reasoning." },
            new Subtitle { startTime = 105.50f, text = "Ahead — the base." },
            new Subtitle { startTime = 106.83f, text = "It looks like a scrapyard someone decided to defend out of pure spite." },
            new Subtitle { startTime = 110.50f, text = "\"Maybe we should fall back?\" logic asks." },
            new Subtitle { startTime = 113.50f, text = "But logic already fell out somewhere between the second explosion and the first bullet." },
            new Subtitle { startTime = 120.56f, text = "So we keep flying." },
            new Subtitle { startTime = 122.20f, text = "Because in this world, survival doesn't belong to the strongest." },
            new Subtitle { startTime = 127.18f, text = "Or the smartest." },
            new Subtitle { startTime = 129.64f, text = "It belongs to whoever still has ammo… and no better options left." }
        };
    }

    private void Start()
    {
        InitializeState();
        StartCoroutine(RunSlideshow());
    }

    private void Update()
    {
        if (skipped) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            skipped = true;
            StopAllCoroutines();
            DOTween.KillAll();
            StartCoroutine(SkipToNextScene());
            return;
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            StopAllCoroutines();
            DOTween.KillAll();
            StartCoroutine(SkipToOutro());
            return;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            SkipToNextSubtitle();
        }
    }

    private void SkipToNextSubtitle()
    {
        if (subtitles == null || subtitles.Length == 0) return;
        if (currentSubtitleIndex >= subtitles.Length) return;

        float targetTime = subtitles[currentSubtitleIndex].startTime;
        float currentTime = Time.time - subtitleStartTime;
        float jump = targetTime - currentTime;
        if (jump <= 0f) return;

        subtitleStartTime -= jump;

        if (voiceoverAudioSource.isPlaying && voiceoverAudioSource.clip != null)
        {
            float newTime = voiceoverAudioSource.time + jump;
            if (newTime < voiceoverAudioSource.clip.length)
            {
                voiceoverAudioSource.time = newTime;
            }
        }
    }

    private IEnumerator SkipToOutro()
    {
        float fastFade = 0.5f;

        if (mainAudioSource.isPlaying)
            mainAudioSource.DOFade(0f, fastFade).SetEase(Ease.InOutQuad).OnComplete(() => mainAudioSource.Stop());
        if (voiceoverAudioSource.isPlaying)
            voiceoverAudioSource.DOFade(0f, fastFade).SetEase(Ease.InOutQuad).OnComplete(() => voiceoverAudioSource.Stop());

        subtitleText.DOFade(0f, fastFade).SetEase(Ease.InOutQuad);
        imageA.DOFade(0f, fastFade).SetEase(Ease.InOutQuad);
        imageB.DOFade(0f, fastFade).SetEase(Ease.InOutQuad);

        blackOverlay.transform.SetAsLastSibling();
        subtitleText.transform.SetAsLastSibling();
        skipHintText.transform.SetAsLastSibling();
        blackOverlay.DOFade(1f, fastFade).SetEase(Ease.InOutQuad);

        yield return new WaitForSeconds(fastFade);

        subtitleText.text = "";

        finalAudioSource.Play();
        finalAudioSource.DOFade(finalAudioMaxVolume, audioFadeInDuration).SetEase(Ease.InOutQuad);

        yield return new WaitForSeconds(pauseBeforeFinalImage);

        finalImageDisplay.sprite = finalImage;
        finalImageDisplay.gameObject.SetActive(true);
        finalImageDisplay.transform.SetAsLastSibling();
        subtitleText.transform.SetAsLastSibling();
        skipHintText.transform.SetAsLastSibling();

        RectTransform finalRT = finalImageDisplay.rectTransform;
        finalRT.localScale = Vector3.one * finalImageStartScale;
        finalRT.anchoredPosition = new Vector2(0f, finalImageStartYOffset);
        SetImageAlpha(finalImageDisplay, 0f);

        Sequence flySeq = DOTween.Sequence();
        flySeq.Append(finalImageDisplay.DOFade(1f, finalImageFlyDuration * 0.4f).SetEase(Ease.OutQuad));
        flySeq.Join(finalRT.DOAnchorPos(Vector2.zero, finalImageFlyDuration).SetEase(Ease.InQuad));
        flySeq.Join(finalRT.DOScale(finalImageEndScale, finalImageFlyDuration).SetEase(Ease.OutBack));

        yield return new WaitForSeconds(finalSceneHoldDuration);

        blackOverlay.transform.SetAsLastSibling();
        blackOverlay.DOFade(1f, finalFadeOutDuration).SetEase(Ease.InOutQuad);
        finalAudioSource.DOFade(0f, finalFadeOutDuration).SetEase(Ease.InOutQuad).OnComplete(() => finalAudioSource.Stop());

        yield return new WaitForSeconds(finalFadeOutDuration);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("Next scene name is empty");
        }
    }

    private void LateUpdate()
    {
        if (skipped) return;
        Transform parent = skipHintText.transform.parent;
        int childCount = parent.childCount;

        if (subtitleText.transform.GetSiblingIndex() != childCount - 2 || skipHintText.transform.GetSiblingIndex() != childCount - 1)
        {
            subtitleText.transform.SetAsLastSibling();
            skipHintText.transform.SetAsLastSibling();
        }
    }

    private IEnumerator SkipToNextScene()
    {
        blackOverlay.transform.SetAsLastSibling();
        blackOverlay.DOFade(1f, skipFadeDuration).SetEase(Ease.InOutQuad);
        skipHintText.DOFade(0f, skipFadeDuration * 0.5f).SetEase(Ease.InOutQuad);
        subtitleText.DOFade(0f, skipFadeDuration * 0.5f).SetEase(Ease.InOutQuad);

        if (mainAudioSource.isPlaying)
            mainAudioSource.DOFade(0f, skipFadeDuration).SetEase(Ease.InOutQuad).OnComplete(() => mainAudioSource.Stop());
        if (finalAudioSource.isPlaying)
            finalAudioSource.DOFade(0f, skipFadeDuration).SetEase(Ease.InOutQuad).OnComplete(() => finalAudioSource.Stop());
        if (voiceoverAudioSource.isPlaying)
            voiceoverAudioSource.DOFade(0f, skipFadeDuration).SetEase(Ease.InOutQuad).OnComplete(() => voiceoverAudioSource.Stop());

        yield return new WaitForSeconds(skipFadeDuration);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("Next scene name is empty");
        }
    }

    private void InitializeState()
    {
        currentImage = imageA;
        nextImage = imageB;

        SetImageAlpha(imageA, 0f);
        SetImageAlpha(imageB, 0f);
        SetImageAlpha(blackOverlay, 0f);
        SetImageAlpha(finalImageDisplay, 0f);

        finalImageDisplay.gameObject.SetActive(false);
        blackOverlay.transform.SetAsLastSibling();
        skipHintText.transform.SetAsLastSibling();

        mainAudioSource.clip = mainAudio;
        mainAudioSource.volume = 0f;
        mainAudioSource.loop = true;

        finalAudioSource.clip = finalAudio;
        finalAudioSource.volume = 0f;
        finalAudioSource.loop = true;

        voiceoverAudioSource.clip = voiceoverAudio;
        voiceoverAudioSource.volume = 0f;
        voiceoverAudioSource.loop = false;

        subtitleText.text = "";
        SetTextAlpha(subtitleText, 0f);
    }

    private IEnumerator RunSlideshow()
    {
        currentImage.sprite = slideImages[0];
        currentImage.DOFade(1f, crossfadeDuration).SetEase(Ease.InOutQuad);

        mainAudioSource.Play();
        mainAudioSource.DOFade(mainAudioMaxVolume, audioFadeInDuration).SetEase(Ease.InOutQuad);

        if (voiceoverAudio != null)
        {
            voiceoverAudioSource.Play();
            voiceoverAudioSource.DOFade(voiceoverMaxVolume, audioFadeInDuration).SetEase(Ease.InOutQuad);
        }

        StartCoroutine(RunSubtitles());

        yield return new WaitForSeconds(perSlideDuration);

        for (int i = 1; i < slideImages.Length; i++)
        {
            nextImage.sprite = slideImages[i];
            nextImage.transform.SetAsLastSibling();

            nextImage.DOFade(1f, crossfadeDuration).SetEase(Ease.InOutQuad);
            currentImage.DOFade(0f, crossfadeDuration).SetEase(Ease.InOutQuad);

            yield return new WaitForSeconds(perSlideDuration);

            Image temp = currentImage;
            currentImage = nextImage;
            nextImage = temp;
        }

        blackOverlay.transform.SetAsLastSibling();
        blackOverlay.DOFade(1f, blackTransitionDuration).SetEase(Ease.InOutQuad);
        mainAudioSource.DOFade(0f, audioFadeOutDuration).SetEase(Ease.InOutQuad).OnComplete(() => mainAudioSource.Stop());
        voiceoverAudioSource.DOFade(0f, audioFadeOutDuration).SetEase(Ease.InOutQuad).OnComplete(() => voiceoverAudioSource.Stop());
        subtitleText.DOFade(0f, audioFadeOutDuration * 0.5f).SetEase(Ease.InOutQuad);

        yield return new WaitForSeconds(blackTransitionDuration);

        finalAudioSource.Play();
        finalAudioSource.DOFade(finalAudioMaxVolume, audioFadeInDuration).SetEase(Ease.InOutQuad);

        yield return new WaitForSeconds(pauseBeforeFinalImage);

        finalImageDisplay.sprite = finalImage;
        finalImageDisplay.gameObject.SetActive(true);
        finalImageDisplay.transform.SetAsLastSibling();

        RectTransform finalRT = finalImageDisplay.rectTransform;
        finalRT.localScale = Vector3.one * finalImageStartScale;
        Vector2 endPos = Vector2.zero;
        finalRT.anchoredPosition = new Vector2(0f, finalImageStartYOffset);
        SetImageAlpha(finalImageDisplay, 0f);

        Sequence flySeq = DOTween.Sequence();
        flySeq.Append(finalImageDisplay.DOFade(1f, finalImageFlyDuration * 0.4f).SetEase(Ease.OutQuad));
        flySeq.Join(finalRT.DOAnchorPos(endPos, finalImageFlyDuration).SetEase(Ease.InQuad));
        flySeq.Join(finalRT.DOScale(finalImageEndScale, finalImageFlyDuration).SetEase(Ease.OutBack));

        yield return new WaitForSeconds(finalSceneHoldDuration);

        blackOverlay.transform.SetAsLastSibling();
        blackOverlay.DOFade(1f, finalFadeOutDuration).SetEase(Ease.InOutQuad);
        finalAudioSource.DOFade(0f, finalFadeOutDuration).SetEase(Ease.InOutQuad).OnComplete(() => finalAudioSource.Stop());

        yield return new WaitForSeconds(finalFadeOutDuration);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("Next scene name is empty");
        }
    }

    private IEnumerator RunSubtitles()
    {
        if (subtitles == null || subtitles.Length == 0) yield break;

        subtitleStartTime = Time.time;
        currentSubtitleIndex = 0;

        while (currentSubtitleIndex < subtitles.Length)
        {
            while (Time.time - subtitleStartTime < subtitles[currentSubtitleIndex].startTime)
            {
                yield return null;
            }

            ShowSubtitle(subtitles[currentSubtitleIndex].text);
            currentSubtitleIndex++;
        }
    }

    private void ShowSubtitle(string text)
    {
        subtitleText.DOKill();
        Sequence seq = DOTween.Sequence();
        seq.Append(subtitleText.DOFade(0f, subtitleFadeDuration).SetEase(Ease.InOutQuad));
        seq.AppendCallback(() => subtitleText.text = text);
        seq.Append(subtitleText.DOFade(1f, subtitleFadeDuration).SetEase(Ease.InOutQuad));
    }

    private void SetImageAlpha(Image img, float a)
    {
        Color c = img.color;
        c.a = a;
        img.color = c;
    }

    private void SetTextAlpha(TextMeshProUGUI t, float a)
    {
        Color c = t.color;
        c.a = a;
        t.color = c;
    }
}