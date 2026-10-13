using UnityEngine;

public class ChuteVitesseInitiale : MonoBehaviour
{
    float g = 9.81f;
    Vector3 initialPosition;
    [SerializeField] Vector3 initialVelocity;
    [SerializeField, Range(0, 1)] float coeffRestitution;
    float t = 0;
    [SerializeField] GameObject impactIndicator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialPosition = gameObject.transform.position;

        float delta = initialVelocity.y * initialVelocity.y + 2 * g * initialPosition.y;
        float t1 = (-initialVelocity.y + Mathf.Sqrt(delta))/(-g);
        float t2 = (-initialVelocity.y - Mathf.Sqrt(delta)) / (-g);
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
        float y = (-g * t * t / 2) + initialVelocity.y * t + initialPosition.y;
        float z = initialVelocity.z * t+ initialPosition.z;
        transform.position = new Vector3(x, y, z);

        if(transform.position.y<0.5f)
        {
            Vector3 vitesseImpact = new Vector3(initialVelocity.x,
                                                 -g * t + initialVelocity.y,
                                                 initialVelocity.z);
            initialVelocity = new Vector3(vitesseImpact.x,
                                          vitesseImpact.y * (-coeffRestitution),
                                          vitesseImpact.z);

            initialPosition = new Vector3(x, 0.5f, z);
            t = 0;
        }
    }
}

