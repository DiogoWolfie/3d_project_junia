using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonHoverEffect : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    [Header("Target Text")]
    public TextMeshProUGUI buttonText;   // Drag your TMP text here

    [Header("Scale Settings")]
    public float hoverScale = 1.1f;
    public float clickScale = 1.2f;
    public float smoothSpeed = 8f;

    [Header("Color Settings")]
    public Color normalColor = new Color32(235, 235, 235, 255); // light gray
    public Color hoverColor  = new Color32(212, 175, 55, 255);  // gold
    public Color clickColor  = new Color32(234, 197, 77, 255);  // brighter gold
    public float colorLerpSpeed = 10f;

    [Header("Hover Pulse Effect")]
    public bool pulseOnHover = true;
    public float pulseAmplitude = 0.05f;   // how much it scales up/down
    public float pulseSpeed = 2f;          // how fast it pulses

    private Vector3 originalScale;
    private Vector3 targetScale;
    private Color targetColor;

    private bool isHovered = false;
    private bool isPressed = false;

    void Start()
    {
        if (buttonText == null)
            buttonText = GetComponentInChildren<TextMeshProUGUI>();

        originalScale = buttonText.rectTransform.localScale;
        targetScale = originalScale;
        targetColor = normalColor;
        buttonText.color = normalColor;
    }

    void Update()
    {
        // --- Smooth scaling ---
        Vector3 finalTarget = targetScale;

        // Pulse if hovered (and not pressed)
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

        // --- Smooth color transition ---
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

        // 🔊 Play hover sound
        UIAudioManager.Instance?.PlayHover();
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

        // 🔊 Play click sound
        UIAudioManager.Instance?.PlayClick();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        targetColor = hoverColor;
        targetScale = originalScale * hoverScale;
    }
}
