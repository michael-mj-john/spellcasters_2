using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable: MonoBehaviour {

    public float health;
    public float armor;

    public void TakeDamage(float damage)
    {
        //Check if we have armor.
        if (armor > 0)
            damage -= armor;

        //Check if we survive the attack.
        if (health - damage > 0)
            health -= damage;
        else
            Die();

    }

    void Die()
    {
    }
}