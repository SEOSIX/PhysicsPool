using UnityEngine;

public class ChuteLibre : MonoBehaviour
{
    Vector3 initialPosition;
    float t = 0;
    float g = -9.81f;
    
    void Start()
    {
        initialPosition = gameObject.transform.position;
    }

    void FixedUpdate()
    {
        if (transform.position.y - (transform.localScale.y /2) <= -0.1f)
            return;
        t += Time.fixedDeltaTime;
        float x = initialPosition.x;
        float y = (g * t * t / 2) + initialPosition.y;
        float z = initialPosition.z;
        transform.position = new Vector3(x, y, z);
    }

 
}
