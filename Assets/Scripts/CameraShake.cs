using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float shakeMagnitude = 0.35f;
    public float shakeDistance = 50f;

    private Vector3 basePosition;
    private GameObject[] tornadoes;

    void Start()
    {
        basePosition = transform.localPosition;
        tornadoes = GameObject.FindGameObjectsWithTag("Tornado");
    }

    void LateUpdate()
    {
        bool shouldShake = false;
        Vector3 playerPos = transform.position;

        foreach (GameObject tornado in tornadoes)
        {
            float dist = Vector3.Distance(playerPos, tornado.transform.position);
            if (dist <= shakeDistance)
            {
                shouldShake = true;
                break;
            }
        }

        if (shouldShake)
            transform.localPosition += Random.insideUnitSphere * shakeMagnitude;
    }
}