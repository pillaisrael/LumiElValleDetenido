using UnityEngine;

// Luciernaga del bosque: vuela en circulos solo cuando la luz la toca.
public class Luciernaga : MonoBehaviour
{
    private Congelable congelable;
    private Vector3 centro;
    private float tiempo;
    private float velocidad;

    void Start()
    {
        congelable = GetComponent<Congelable>();
        centro = transform.position;
        tiempo = Random.Range(0f, 10f);
        velocidad = Random.Range(1.5f, 3f);
    }

    void Update()
    {
        if (!congelable.vivo) return;   // el tiempo esta detenido
        tiempo += Time.deltaTime * velocidad;
        transform.position = centro + new Vector3(Mathf.Cos(tiempo) * 0.6f, Mathf.Sin(tiempo * 1.7f) * 0.4f, 0f);
    }
}
