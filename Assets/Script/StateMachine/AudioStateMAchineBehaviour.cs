using UnityEngine;

public class AudioStateMAchineBehaviour : StateMachineBehaviour
{
    public AudioClip audioClip; // Audio clip yang ingin diputar
    private AudioSource audioSource; // Audio source untuk memutar audio

    // Dipanggil ketika memasuki state animasi
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Mengambil AudioSource yang ada di GameObject
        audioSource = animator.gameObject.GetComponent<AudioSource>();
        
        if (audioSource && audioClip)
        {
            audioSource.PlayOneShot(audioClip); // Memutar audio saat memasuki state
        }
    }
}
