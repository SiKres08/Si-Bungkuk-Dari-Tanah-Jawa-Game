using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Pastikan ini ditambahkan

public class LoadScreen : MonoBehaviour
{
    [SerializeField] private GameObject loadingCanvas; // Canvas Loading
    [SerializeField] private Slider progressBar; // Slider Loading (Progress Bar)

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        Debug.Log("Coroutine dimulai!");
        // Tampilkan loading canvas
        if (loadingCanvas != null)
        {
            loadingCanvas.SetActive(true);
        }

        // Mulai proses loading
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false; // Jangan langsung masuk ke scene baru

        // Update progress bar selama loading
        while (!operation.isDone)
        {
            // Nilai progress dihitung dari 0 hingga 0.9 (90%)
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            Debug.Log("Loading Progress: " + progress);

            // Update nilai slider jika ada
            if (progressBar != null)
            {
                progressBar.value = progress; // Update slider
                Debug.Log("Slider Progress: " + progressBar.value); // Debug nilai slider
            }

            // Aktifkan scene jika progress mencapai 90%
            if (operation.progress >= 0.9f)
            {
                Debug.Log("Loading selesai! Mengaktifkan scene...");
                // Tambahkan delay (opsional)
                yield return new WaitForSeconds(1f);
                operation.allowSceneActivation = true;
            }

            if (progressBar == null)
            {
                Debug.LogError("Progress Bar tidak diassign di Inspector!");
            }


            yield return null; // Tunggu frame berikutnya
        }
    }
}
