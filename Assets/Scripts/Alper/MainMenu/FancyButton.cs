using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FancyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public AudioSource uiAudioSource; 
    public AudioClip hoverSound;
    public AudioClip clickSound;
    public Image glowOverlay;
    
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

    if (uiAudioSource == null)
    {
        AudioSource[] allSources = FindObjectsOfType<AudioSource>(true);

        foreach (AudioSource src in allSources)
        {
            if (src.gameObject.name.Contains("UI") ||
                src.gameObject.name.Contains("ui") ||
                src.gameObject.name.Contains("Ui") ||
                src.gameObject.name.Contains("uI"))
            {
                uiAudioSource = src;
                break;
            }
        }

        if (uiAudioSource == null)
            Debug.LogWarning("FancyButton: UI ile ilgili bir AudioSource bulunamadı!");
    }
}



    public void OnPointerEnter(PointerEventData e)
    {
        transform.localScale = originalScale * 1.05f;
        image.color = new Color(1f, 0.95f, 0.8f, 1f);

        if (glowOverlay != null)
            glowOverlay.enabled = true;

        if (hoverSound != null && uiAudioSource != null)
            uiAudioSource.PlayOneShot(hoverSound);
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

        if (clickSound != null && uiAudioSource != null)
            uiAudioSource.PlayOneShot(clickSound);
    }

    public void OnPointerUp(PointerEventData e)
    {
        transform.localScale = originalScale * 1.05f;
    }
}