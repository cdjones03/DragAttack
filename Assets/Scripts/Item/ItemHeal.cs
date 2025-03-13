using UnityEngine;

public class ItemHeal : MonoBehaviour
{
    public int healAmount = 20;
    private float lifeTime;


    void Start()
    {
        lifeTime = 0f;
    }

    void Update()
    {
        lifeTime += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && lifeTime > 1f)
        {
            other.GetComponent<PlayerHealth>().Heal(healAmount);
            Destroy(gameObject);
        }
    }
}
