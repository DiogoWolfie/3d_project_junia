using UnityEngine;
using UnityEngine.EventSystems;
using TMPro; // only if you're using TextMeshPro

public class ButtonHoverEffect : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    public TextMeshProUGUI buttonText;   // drag your TMP text here
    public float hoverScale = 1.1f;
    public float clickScale = 1.2f;
    public float smoothSpeed = 8f;

    private Vector3 originalScale;
    private Vector3 targetScale;

    void Start()
    {
        if (buttonText == null)
            buttonText = GetComponentInChildren<TextMeshProUGUI>();

        originalScale = buttonText.rectTransform.localScale;
        targetScale = originalScale;
    }

    void Update()
    {
        buttonText.rectTransform.localScale = Vector3.Lerp(
            buttonText.rectTransform.localScale,
            targetScale,
            Time.deltaTime * smoothSpeed
        );
    }

    public void OnPointerEnter(PointerEventData eventData) => targetScale = originalScale * hoverScale;
    public void OnPointerExit(PointerEventData eventData) => targetScale = originalScale;
    public void OnPointerDown(PointerEventData eventData) => targetScale = originalScale * clickScale;
    public void OnPointerUp(PointerEventData eventData) => targetScale = originalScale * hoverScale;
}
