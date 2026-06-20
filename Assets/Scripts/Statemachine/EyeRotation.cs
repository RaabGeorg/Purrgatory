using UnityEngine;
using Random = UnityEngine.Random;

public class EyeRotation : MonoBehaviour
{
    public float randomRotation;
    
    public void Start()
    { 
        randomRotation = Random.Range(0f, 180f);
    }
    
    public void Update()
    {
        transform.Rotate(Vector3.up, randomRotation * Time.deltaTime);
    }
    
}
