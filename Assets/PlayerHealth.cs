using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;

    private int health;
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyAttackHitbox"))
        {
            health--;
            print(other.name);
        }

        if (health <= 0)
        {
            //DEath
            Destroy(gameObject);
        }
    }
}
