using UnityEngine;


    public class RabbitHealth : MonoBehaviour
    {
        public float maxHealth = 100;

        private float health;
        // Start is called before the first frame update
        void Start()
        {
            health = maxHealth;
        }

        // Update is called once per frame
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("PlayerAttackHitbox"))
            {
                health--;
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
    }