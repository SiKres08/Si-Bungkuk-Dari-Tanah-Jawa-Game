using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public GameObject boss; // Referensi ke GameObject boss

    void Update()
    {
        // Pastikan boss ada
        if (boss == null)
        {
            // Jika boss hilang (misalnya di-destroy atau keluar dari scene), nonaktifkan Wall
            gameObject.SetActive(false);
            Debug.Log("GameObject Wall dinonaktifkan karena boss hilang.");
        }
    }
}
