using UnityEngine;

public class ChuteBaseSpeed : MonoBehaviour
{
    Vector3 initialPosition;
    [SerializeField] Vector3 initialVelocity;
    Vector3 velocity;
    float t = 0;
    float g = 9.81f;
    [SerializeField, Range(0f, 1f)]float coefRestitution = 0.2f;

    [SerializeField] GameObject impactIndicator;
    
    void Start()
    {
        initialPosition = gameObject.transform.position;
        
        
        float d = initialVelocity.y * initialVelocity.y + 2 * g * initialPosition.y;
        float t1 = (-initialVelocity.y + Mathf.Sqrt(d)) / (-g);
        float t2 = (-initialVelocity.y - Mathf.Sqrt(d)) / (-g);
        float tImpact = Mathf.Max(t1, t2);
        Debug.Log(tImpact);

        float xImpact = initialVelocity.x * tImpact + initialPosition.x;
        float yImpact = (-g * tImpact * tImpact / 2) + initialVelocity.y * tImpact + initialPosition.y;
        float zImpact = initialVelocity.z * tImpact + initialPosition.z;

        impactIndicator.transform.position = new Vector3 (xImpact, yImpact, zImpact);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        t += Time.fixedDeltaTime;
        float x = initialVelocity.x * t + initialPosition.x;
        float y = (-g * t * t / 2) + initialVelocity.x * t +  initialPosition.y;
        float z = initialVelocity.x * t + initialPosition.z;
        transform.position = new Vector3(x, y, z);

        velocity = initialPosition - transform.position / t;

        if (transform.position.y < 0.5f)
        {
            Vector3 newVelocity = new Vector3(initialVelocity.x,
                (-g * t + initialVelocity.y) * -coefRestitution, initialVelocity.z);

            initialVelocity = new Vector3(newVelocity.x,
                newVelocity.y * (-coefRestitution), newVelocity.z);

            initialPosition = gameObject.transform.position;
            t = 0;
            
            
        }
    }
}
