using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVariables : MonoBehaviour, ITimeControl
{
    // PLAYER HEALTH
    
    // Health, TimeControl and ChangeTime

    // HEALTH
    public uint MaxHealth;
    uint Health;
    private HealthBar _healthBar;

    // TIME
    public float TimeControl { get; set; } = 1f;

    // WAIT


    private void Start()
    {
        Health = MaxHealth;
        _healthBar = FindObjectOfType<HealthBar>();
    }

    public void TakeDamage(uint damage)
    {
        if (damage >= Health)
        {
            // game over
        }
        else {
            Health -= damage;
            // the best should be to sync the values of the healthbar with this health Script
            _healthBar.TakeDamages(damage);
        }
    }

    public void Heal(uint heal)
    {
        if (heal + Health >= MaxHealth)
        {
            Health = MaxHealth;
        }
        else {
            Health += heal;
            // same, there's a better way to do this
            _healthBar.Heal(heal);
        }
    }

    public void ChangeTimeControl(float timeControl)
    {
        TimeControl = timeControl;
        // change other variables made to avoid having to calc them everytime
    }
}
