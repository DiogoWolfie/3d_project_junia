using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonHoverEffect : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    [Header("Target Text")]
    public TextMeshProUGUI buttonText;

    [Header("Audio (optional)")]
    public UIAudioManager uiAudio;

    [Header("Scale Settings")]
    public float hoverScale = 1.1f;
    public float clickScale = 1.2f;
    public float smoothSpeed = 8f;

    [Header("Color Settings")]
    public Color normalColor = new Color32(235, 235, 235, 255);
    public Color hoverColor  = new Color32(212, 175, 55, 255);
    public Color clickColor  = new Color32(234, 197, 77, 255);
    public float colorLerpSpeed = 10f;

    [Header("Hover Pulse Effect")]
    public bool pulseOnHover = true;
    public float pulseAmplitude = 0.05f;
    public float pulseSpeed = 2f;

    private Vector3 originalScale;
    private Vector3 targetScale;
    private Color targetColor;

    private bool isHovered = false;
    private bool isPressed = false;

    void Start()
    {
        if (buttonText == null)
            buttonText = GetComponentInChildren<TextMeshProUGUI>();

        if (uiAudio == null)
            uiAudio = FindObjectOfType<UIAudioManager>();

        originalScale = buttonText.rectTransform.localScale;
        targetScale = originalScale;
        targetColor = normalColor;
        buttonText.color = normalColor;
    }

    void Update()
    {
        Vector3 finalTarget = targetScale;

        if (isHovered && !isPressed && pulseOnHover)
        {
            float pulse = Mathf.Sin(Time.unscaledTime * pulseSpeed) * pulseAmplitude;
            finalTarget = originalScale * (hoverScale + pulse);
        }

        buttonText.rectTransform.localScale = Vector3.Lerp(
            buttonText.rectTransform.localScale,
            finalTarget,
            Time.deltaTime * smoothSpeed
        );

        buttonText.color = Color.Lerp(
            buttonText.color,
            targetColor,
            Time.deltaTime * colorLerpSpeed
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        targetColor = hoverColor;
        targetScale = originalScale * hoverScale;

        uiAudio?.PlayHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        targetColor = normalColor;
        targetScale = originalScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        targetColor = clickColor;
        targetScale = originalScale * clickScale;

        uiAudio?.PlayClick();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        targetColor = hoverColor;
        targetScale = originalScale * hoverScale;
    }
}
