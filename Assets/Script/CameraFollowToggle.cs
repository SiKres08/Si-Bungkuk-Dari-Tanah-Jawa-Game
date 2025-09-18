using UnityEngine;
using Cinemachine;

public class CameraFollowToggle : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera; // Referensi ke Virtual Camera
    public Collider2D triggerZone;                  // Collider tempat kamera berhenti
    private bool isInTriggerZone = false;         // Apakah pemain di dalam trigger?

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInTriggerZone = true;
            StopCameraFollow();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInTriggerZone = false;
            ResumeCameraFollow();
        }
    }

    private void StopCameraFollow()
    {
        // Menyetel "Follow" menjadi null untuk menghentikan kamera mengikuti pemain
        if (virtualCamera != null)
        {
            virtualCamera.Follow = null;
        }
    }

    private void ResumeCameraFollow()
    {
        // Menyetel kembali "Follow" ke pemain
        if (virtualCamera != null && !isInTriggerZone)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                virtualCamera.Follow = player.transform;
            }
        }
    }
}
