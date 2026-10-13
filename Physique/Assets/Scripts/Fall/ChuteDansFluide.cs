using UnityEngine;

public class ChuteDansFluide : MonoBehaviour
{
    [SerializeField] float masse = 1; //en kg
    [SerializeField] float g = 9.80665f; //gravité sur Terre
    float tempsEcoule = 0;
    Vector3 positionInitiale;
    [SerializeField] Vector3 vitesseInitiale;

    [SerializeField] float rho_MasseVolumiqueFluide = 1000; //en kg par mètre cube, exemple : 1000kg /mcube pour l'eau
    [SerializeField] float rho_Air = 1.2f;
    float volumeObjet; // ici je vais le calculer a partir du volume d'une sphère

    Transform tf;

    bool wasInWater = false;

    void Start()
    {
        tf = gameObject.GetComponent<Transform>();
        positionInitiale = tf.position;

        float rayonSphere = transform.localScale.x / 2;
        volumeObjet = (4f / 3f) * Mathf.PI * rayonSphere * rayonSphere * rayonSphere;
        //volume sphere : 4/3 * pi * rayon Au cube 

        wasInWater = (positionInitiale.y <= 0);
    }

    void FixedUpdate()
    {
        tempsEcoule += Time.deltaTime;

        bool isInWater = (transform.position.y <= 0);
        if (isInWater != wasInWater) // si transition de fluide
        {
            float rhoPrecedent = wasInWater ? rho_MasseVolumiqueFluide : rho_Air;

            positionInitiale = transform.position;
            vitesseInitiale = new Vector3(vitesseInitiale.x,
                                                 (-1f + rhoPrecedent * volumeObjet / masse)*g * tempsEcoule + vitesseInitiale.y,
                                                 vitesseInitiale.z);

            tempsEcoule = 0;

            wasInWater = isInWater;
        }
        float rhoActuel = isInWater ? rho_MasseVolumiqueFluide : rho_Air;

        Vector3 newPosition = new Vector3(vitesseInitiale.x * tempsEcoule + positionInitiale.x,
                                        0.5f * g * (-1f + rhoActuel * volumeObjet / masse) * tempsEcoule * tempsEcoule + vitesseInitiale.y * tempsEcoule + positionInitiale.y,
                                        vitesseInitiale.z * tempsEcoule + positionInitiale.z);
        tf.position = newPosition;


    }
}
