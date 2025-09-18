using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public TMP_Text healthBarText;
    Damageable playerDamageable;

    private void Awake() 
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if(player == null)
        {
            Debug.Log("No player found in the scene. make sure it has tag 'Player'");
        }
        playerDamageable = player.GetComponent<Damageable>();
    }

    // Start is called before the first frame update
    void Start()
    {
        healthSlider.value = CalculateSliderPercentage(playerDamageable.Health, playerDamageable.MaxHealth);
        healthBarText.text = "HP   " + playerDamageable.Health + "/" + playerDamageable.MaxHealth;
    }

    private void OnEnable()
    {
        playerDamageable.healthChanged.AddListener(OnPLayerHealChanged);
    }

    private void OnDisable() 
    {
        playerDamageable.healthChanged.RemoveListener(OnPLayerHealChanged);
    }

    private float CalculateSliderPercentage(float currentHealth, float MaxHealth) 
    {
        return currentHealth / MaxHealth;
    }

    // Update is called once per frame
    void OnPLayerHealChanged(int newHealth,  int maxHealth)
    {
        healthSlider.value = CalculateSliderPercentage(newHealth, maxHealth);
        healthBarText.text = "HP   " + newHealth + "/" + maxHealth;
    }
}
