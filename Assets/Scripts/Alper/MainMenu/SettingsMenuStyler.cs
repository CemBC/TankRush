using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteAlways]
public class SettingsMenuStyler : MonoBehaviour
{
    [Header("Assign UI Elements")]
    public RectTransform background;
    public TextMeshProUGUI titleText;
    public Slider musicSlider;
    public Slider effectsSlider;
    public Button backButton;

    [Header("Sprite References")]
    public Sprite woodPanel;
    public Sprite woodBar;
    public Sprite handleSprite;
    public Sprite buttonSprite;

    [Header("Font Settings")]
    public TMP_FontAsset menuFont;
    public Color textColor = new Color(0.23f, 0.12f, 0f); // koyu kahverengi

    void OnEnable()
    {
        ApplyStyle();
    }

    public void ApplyStyle()
    {
        if (background && woodPanel)
        {
            var img = background.GetComponent<Image>();
            if (img) {
                img.sprite = woodPanel;
                img.type = Image.Type.Sliced;
                background.sizeDelta = new Vector2(800, 600);
            }
        }

        if (titleText)
        {
            titleText.font = menuFont;
            titleText.fontSize = 48;
            titleText.color = textColor;
        }

        SetupSlider(musicSlider);
        SetupSlider(effectsSlider);
        SetupButton(backButton);
    }

    void SetupSlider(Slider slider)
    {
        if (!slider) return;

        var bg = slider.transform.Find("Background")?.GetComponent<Image>();
        var handle = slider.handleRect?.GetComponent<Image>();
        var fill = slider.fillRect?.GetComponent<Image>();

        if (bg && woodBar)
        {
            bg.sprite = woodBar;
            bg.type = Image.Type.Sliced;
        }
        if (fill)
        {
            fill.color = new Color(1f, 0.85f, 0.6f); // açık tahta rengi
        }
        if (handle && handleSprite)
        {
            handle.sprite = handleSprite;
            handle.preserveAspect = true;
        }
    }

    void SetupButton(Button button)
    {
        if (!button) return;

        var img = button.GetComponent<Image>();
        var text = button.GetComponentInChildren<TextMeshProUGUI>();

        if (img && buttonSprite)
        {
            img.sprite = buttonSprite;
            img.type = Image.Type.Sliced;
        }
        if (text)
        {
            text.font = menuFont;
            text.fontSize = 40;
            text.color = textColor;
        }
    }
}
