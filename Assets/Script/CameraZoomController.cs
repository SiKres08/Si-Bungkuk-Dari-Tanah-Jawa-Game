using UnityEngine;
using Cinemachine;

public class CameraZoomController : MonoBehaviour
{
    [Header("Camera Settings")]
    public CinemachineVirtualCamera virtualCamera; // Assign Cinemachine Virtual Camera
    public float normalFOV = 60f;  // FOV normal (zoom in)
    public float zoomOutFOV = 80f; // FOV zoom out (lebih lebar)
    public float zoomSpeed = 2f;   // Kecepatan transisi zoom
    public float verticalOffset = 5f; // Offset Y saat zoom out (naik ke atas)

    [Header("Target Settings")]
    public Transform player;       // Transform dari player
    public Transform boss;         // Transform dari boss atau objek
    public float triggerDistance = 10f; // Jarak maksimum trigger zoom out

    private Vector3 originalOffset; // Offset awal kamera
    private CinemachineTransposer transposer;

    void Start()
    {
        if (virtualCamera != null)
        {
            // Ambil Transposer dari Virtual Camera
            transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                originalOffset = transposer.m_FollowOffset; // Simpan offset awal
            }

            virtualCamera.m_Lens.FieldOfView = normalFOV; // Set FOV awal
        }
    }

    void Update()
    {
        if (player != null && boss != null)
        {
            // Hitung jarak player ke boss
            float distance = Vector3.Distance(player.position, boss.position);

            if (distance <= triggerDistance)
            {
                ZoomOut();
            }
            else
            {
                ZoomIn();
            }
        }
    }

    void ZoomOut()
    {
        // Transisi FOV
        virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, zoomOutFOV, Time.deltaTime * zoomSpeed);

        // Ubah offset kamera ke atas (Y)
        if (transposer != null)
        {
            Vector3 newOffset = new Vector3(originalOffset.x, originalOffset.y + verticalOffset, originalOffset.z);
            transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, newOffset, Time.deltaTime * zoomSpeed);
        }
    }

    void ZoomIn()
    {
        // Transisi FOV kembali ke normal
        virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, normalFOV, Time.deltaTime * zoomSpeed);

        // Reset offset ke posisi awal
        if (transposer != null)
        {
            transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, originalOffset, Time.deltaTime * zoomSpeed);
        }
    }
}
