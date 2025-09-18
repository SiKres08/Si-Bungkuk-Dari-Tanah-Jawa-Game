using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CutSceneController : MonoBehaviour
{
    public Image[] cutsceneImages; // Array of Images for the cutscene
    public float displayDuration = 2.0f; // Time each image stays visible
    public float fadeDuration = 1.0f; // Duration of fade-in and fade-out
    public string nextSceneName; // Name of the next scene to load

    private bool isSkipped = false;

    private void Start()
    {
        // Start the cutscene sequence
        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        foreach (Image image in cutsceneImages)
        {
            if (isSkipped) break;

            // Enable the image
            image.gameObject.SetActive(true);
            
            // Fade in
            yield return StartCoroutine(FadeImage(image, 0f, 1f));

            // Wait for display duration
            yield return new WaitForSeconds(displayDuration);

            // Fade out
            yield return StartCoroutine(FadeImage(image, 1f, 0f));

            // Disable the image
            image.gameObject.SetActive(false);
        }

        // Cutscene is complete, load the next scene
        Debug.Log("Cutscene complete! Loading next scene...");
        SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator FadeImage(Image image, float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;

        // Set initial alpha
        Color color = image.color;
        color.a = startAlpha;
        image.color = color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            color.a = alpha;
            image.color = color;
            yield return null;
        }

        // Ensure final alpha is set
        color.a = endAlpha;
        image.color = color;
    }

    public void SkipCutscene()
    {
        isSkipped = true;
        StopAllCoroutines();
        Debug.Log("Cutscene skipped! Loading next scene...");
        FindObjectOfType<LoadScreen>().LoadScene(nextSceneName);
    }
}
