using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FancyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public AudioSource audioSource;
    public AudioClip hoverSound;
    public AudioClip clickSound;
    public Image glowOverlay; // parlama efekti (opsiyonel)
    
    private Image image;
    private Vector3 originalScale;
    private Color originalColor;

    void Start()
    {
        image = GetComponent<Image>();
        originalScale = transform.localScale;
        originalColor = image.color;
        
        if (glowOverlay != null)
            glowOverlay.enabled = false;
    }

    public void OnPointerEnter(PointerEventData e)
    {
        transform.localScale = originalScale * 1.05f;
        image.color = new Color(1f, 0.95f, 0.8f, 1f); // hafif sarı parıltı
        if (glowOverlay != null)
            glowOverlay.enabled = true;
        if (hoverSound != null && audioSource != null)
            audioSource.PlayOneShot(hoverSound);
    }

    public void OnPointerExit(PointerEventData e)
    {
        transform.localScale = originalScale;
        image.color = originalColor;
        if (glowOverlay != null)
            glowOverlay.enabled = false;
    }

    public void OnPointerDown(PointerEventData e)
    {
        transform.localScale = originalScale * 0.95f;
        if (clickSound != null && audioSource != null)
            audioSource.PlayOneShot(clickSound);
    }

    public void OnPointerUp(PointerEventData e)
    {
        transform.localScale = originalScale * 1.05f;
    }
}
