using UnityEngine;

 
public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 3f;
    // [SerializeField] private int damage = 10;
    
    private Rigidbody rb;
    void Start()
    {
        
        rb = GetComponent<Rigidbody>();
        
        rb.linearVelocity = transform.forward * speed;
        
        Destroy(gameObject, lifetime);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.layer == LayerMask.NameToLayer("Character"))
            return;
        
        
        Destroy(gameObject);
    }
}

