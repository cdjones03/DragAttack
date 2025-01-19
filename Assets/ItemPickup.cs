using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public float throwForce = 10f;
    public float rotationSpeed = 720f;  // Degrees per second (2 full rotations)
    public int damageAmount = 30;
    public float lifespan = 3f;
    
    private bool isPickedUp = false;
    private bool isThrown = false;
    private Transform itemSpot;
    private Rigidbody2D rb;

    private float lifeTime;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lifeTime = 0f;
    }

    void Update()
    {
        lifeTime += Time.deltaTime;
        // Rotate the item while it's thrown
        if (isThrown)
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }

        
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isPickedUp && !isThrown && lifeTime > 1f)
        {
            AnimationController player = other.GetComponent<AnimationController>();
            if (player != null && !player.HasItem())
            {
                PickupItem(player.GetItemSpot());
                player.SetHasItem(true);
            }
        }

        if (other.CompareTag("Enemy") && isThrown)
        {
            Enemy enemy = other.GetComponent<Enemy>();
            Debug.Log("Hit enemy for " + damageAmount + " item damage!");
            if (enemy != null)
            {
                enemy.TakeDamage(damageAmount);
                Destroy(gameObject);
            }
        }
    }

    public void PickupItem(Transform spot)
    {
        itemSpot = spot;
        isPickedUp = true;
        isThrown = false;
        rb.isKinematic = true;
        transform.SetParent(itemSpot);
        transform.localPosition = Vector3.zero;
    }

    public void ThrowItem(Vector2 direction, Transform playerTransform)
    {
        isPickedUp = false;
        isThrown = true;
        transform.SetParent(null);
        
        float offsetX = direction.x > 0 ? 5f : -5f;
        Vector3 throwPosition = playerTransform.position + new Vector3(offsetX, 0, 0);
        transform.position = throwPosition;
        
        rb.isKinematic = false;
        rb.velocity = Vector2.zero;
        rb.AddForce(direction * throwForce, ForceMode2D.Impulse);
        
        Destroy(gameObject, lifespan);
    }
} 