using UnityEngine;

public class SwingingBlade : MonoBehaviour
{
    public float swingSpeed = 1.0f;
    public float swingAmplitude = 45.0f;

    private float timeOffset;
    private Vector3 initialPosition;

    private void Start()
    {
        timeOffset = Random.Range(0f, 2f * Mathf.PI); // Um zufällige Anfangsphasen zu erzeugen
        initialPosition = transform.position; // Speichere die Anfangsposition
    }

    private void Update()
    {
        float angle = Mathf.Sin((Time.time + timeOffset) * swingSpeed) * swingAmplitude;
        transform.position = initialPosition;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}