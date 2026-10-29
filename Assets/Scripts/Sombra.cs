using UnityEngine;

// Sombra de La Bruma. Congelada es una ESTATUA: no se mueve, no hace dano
// y se puede pisar. Si la luz la toca, despierta y camina de lado a lado.
public class Sombra : MonoBehaviour, ICondicionPeligro
{
    public float velocidad = 2.5f;
    public Transform pupila;
    public Collider2D cuerpoSolido;   // se activa cuando es estatua

    private Rigidbody2D rb;
    private Congelable congelable;
    private float direccion = 1f;
    private Vector3 posPupila;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        congelable = GetComponent<Congelable>();
        if (pupila != null) posPupila = pupila.localPosition;
    }

    public bool PeligroActivo()
    {
        return congelable == null || congelable.vivo;
    }

    void FixedUpdate()
    {
        bool vivo = congelable == null || congelable.vivo;
        if (cuerpoSolido != null) cuerpoSolido.enabled = !vivo;
        if (!vivo) return;

        Vector2 pos = rb.position;
        Vector2 frente = pos + new Vector2(direccion * 0.55f, 0f);

        bool hayPared = Physics2D.Raycast(frente, new Vector2(direccion, 0f), 0.1f).collider != null;
        bool haySuelo = Physics2D.Raycast(frente, Vector2.down, 0.7f).collider != null;

        if (hayPared || !haySuelo) direccion *= -1f;

        rb.MovePosition(pos + new Vector2(direccion * velocidad * Time.fixedDeltaTime, 0f));
    }

    void Update()
    {
        // El ojo mira a Lumi (solo si esta despierta)
        Transform lumi = GameManager.instancia.LumiActual();
        if (pupila == null || lumi == null || (congelable != null && !congelable.vivo)) return;

        Vector3 haciaLumi = (lumi.position - transform.position).normalized;
        pupila.localPosition = posPupila + haciaLumi * 0.1f;
    }
}
