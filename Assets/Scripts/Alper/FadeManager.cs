using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;
    [Header("Fade Settings")]
    public Image fadeImage;           // Siyah Image (ekranı karartan)
    public float fadeDuration = 1.5f; // Geçiş süresi (sn)

    private bool isFading = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Eğer fadeImage sahnede varsa açılışta fade-in yap
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 1f;
            fadeImage.color = c;
            StartCoroutine(FadeIn());
        }
    }

    public void FadeToScene(string sceneName)
    {
        if (!isFading)
            StartCoroutine(FadeOutAndLoad(sceneName));
    }

    IEnumerator FadeIn()
    {
        isFading = true;
        float t = 1f;
        while (t > 0f)
        {
            t -= Time.deltaTime / fadeDuration;
            if (fadeImage != null)
                fadeImage.color = new Color(0, 0, 0, t);
            yield return null;
        }
        isFading = false;
    }

    IEnumerator FadeOutAndLoad(string sceneName)
    {
        isFading = true;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            if (fadeImage != null)
                fadeImage.color = new Color(0, 0, 0, t);
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(sceneName);

        // Sahne yüklenince fade-in tekrar çalıştır
        yield return new WaitUntil(() => SceneManager.GetActiveScene().isLoaded);
        StartCoroutine(FadeIn());
    }
}
