using UnityEngine;

// Espinas de La Bruma: salen en la oscuridad y se esconden con la luz.
public class EspinaBruma : MonoBehaviour, ICondicionPeligro
{
    public float velocidad = 10f;
    public bool afuera { get; private set; }

    private Congelable congelable;
    private Vector3 posArriba;

    void Start()
    {
        congelable = GetComponent<Congelable>();
        posArriba = transform.position;
    }

    void Update()
    {
        // Con luz se esconden en el suelo; en la oscuridad salen
        Vector3 objetivo = congelable.vivo ? posArriba + Vector3.down * 0.85f : posArriba;
        transform.position = Vector3.MoveTowards(transform.position, objetivo, velocidad * Time.deltaTime);
        afuera = Vector3.Distance(transform.position, posArriba) < 0.4f;
    }

    public bool PeligroActivo()
    {
        return afuera;
    }
}
