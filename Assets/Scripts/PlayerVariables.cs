using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVariables : MonoBehaviour, ITimeControl
{
    // Health, TimeControl and ChangeTime

    // HEALTH
    public uint MaxHealth;
    uint Health;

    // TIME
    public float TimeControl { get; set; } = 1f;

    // WAIT


    private void Start()
    {
        Health = MaxHealth;
    }

    public void TakeDamage(uint damage)
    {
        if (damage >= Health)
        {
            // game over
        }
        else Health -= damage;
    }

    public void Heal(uint heal)
    {
        if (heal + Health >= MaxHealth)
        {
            Health = MaxHealth;
        }
        else Health += heal;
    }

    public void ChangeTimeControl(float timeControl)
    {
        TimeControl = timeControl;
        // change other variables made to avoid having to calc them everytime
    }
}
