using UnityEngine;

// Pilar del Gran Faro. Lumi lo toca y se enciende.
public class Pilar : MonoBehaviour
{
    public SpriteRenderer llama;
    public FuenteLuz luz;
    private bool encendido;

    void Start()
    {
        llama.enabled = false;
        luz.encendida = false;
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (encendido || otro.GetComponent<Lumi>() == null) return;
        encendido = true;
        llama.enabled = true;
        luz.encendida = true;
        GameManager.instancia.CrearPedazos(transform.position + Vector3.up, new Color(1f, 0.84f, 0.3f), 10);
        GameManager.instancia.Sonar("pilar");
        GameManager.instancia.PilarEncendido();
    }
}
