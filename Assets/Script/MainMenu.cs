using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Animator transitionAnimator; // Animator untuk animasi transisi
    public string animationTriggerName = "Start"; // Nama trigger untuk memulai animasi
    public float animationDuration = 1f; // Durasi animasi, sesuaikan dengan panjang animasi Anda

    public GameObject[] menuButtons; // Semua tombol menu (Play, Option, Exit)

    public void PlayGame()
    {
        HideAllButtons();
        StartCoroutine(PlayAnimationAndLoadScene());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator PlayAnimationAndLoadScene()
    {
        if (transitionAnimator != null)
        {
            // Trigger animasi
            transitionAnimator.SetTrigger(animationTriggerName);

            // Tunggu hingga animasi selesai
            yield return new WaitForSeconds(animationDuration);
        }

        // Pindah ke scene berikutnya
        // SceneManager.LoadSceneAsync(1);
        FindObjectOfType<LoadScreen>().LoadScene("CutScene");
    }

    private void HideAllButtons()
    {
        foreach (GameObject button in menuButtons)
        {
            if (button != null)
            {
                button.SetActive(false); // Menyembunyikan tombol
            }
        }
    }
}
