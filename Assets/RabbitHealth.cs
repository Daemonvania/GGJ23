using System.Collections;
using UnityEngine;


    public class RabbitHealth : MonoBehaviour
    {
        public float maxHealth = 100;

        public float health;

        public bool canTakeDamage = false;
        // Start is called before the first frame update
        void Start()
        {
            health = maxHealth;
        }

        // Update is called once per frame
        private void OnTriggerEnter(Collider other)
        {
            if (!canTakeDamage)
            {
                return;
            }
            if (other.CompareTag("PlayerAttackHitbox"))
            {
                health -= 10;
                print("enemyTakeDamage");
                canTakeDamage = false;
                StartCoroutine(enableDamage());
            }

            if (health <= 50)
            {
                //phase 2 (attack twice maybe)
                
            }
            if (health <= 0)
            {
                //DEath
                Destroy(gameObject);
            }
        }

        private IEnumerator enableDamage()
        {
            yield return new WaitForSeconds(0.3f);
            canTakeDamage = true;
        }
    }