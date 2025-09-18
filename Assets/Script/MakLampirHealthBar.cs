using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MakLampirHealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public TMP_Text healthBarText;
    Damageable bossDamageable;

    private void Awake() 
    {
        GameObject player = GameObject.FindGameObjectWithTag("Boss");

        if(player == null)
        {
            Debug.Log("No player found in the scene. make sure it has tag 'Boss'");
        }
        bossDamageable = player.GetComponent<Damageable>();
    }

    // Start is called before the first frame update
    void Start()
    {
        healthSlider.value = CalculateSliderPercentage(bossDamageable.Health, bossDamageable.MaxHealth);
        healthBarText.text = "MAK LAMPIR   " + bossDamageable.Health + "/" + bossDamageable.MaxHealth;
    }

    private void OnEnable()
    {
        bossDamageable.healthChanged.AddListener(OnPLayerHealChanged);
    }

    private void OnDisable() 
    {
        bossDamageable.healthChanged.RemoveListener(OnPLayerHealChanged);
    }

    private float CalculateSliderPercentage(float currentHealth, float MaxHealth) 
    {
        return currentHealth / MaxHealth;
    }

    // Update is called once per frame
    void OnPLayerHealChanged(int newHealth,  int maxHealth)
    {
        healthSlider.value = CalculateSliderPercentage(newHealth, maxHealth);
        healthBarText.text = "MAK LAMPIR  " + newHealth + "/" + maxHealth;
    }
}
