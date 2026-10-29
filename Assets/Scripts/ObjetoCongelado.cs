using UnityEngine;

// Cosas del valle detenidas en el aire:
//  CAER: gotas, escombros, rocas y estalactitas. Congeladas son suelo.
//        Si la luz las toca, el tiempo vuelve y se caen.
//  ATRAVESAR: cascadas y aldeanos. Congelados son solidos;
//        con luz se mueven y se pueden atravesar.
public class ObjetoCongelado : MonoBehaviour, ICondicionPeligro
{
    public enum Modo { Caer, Atravesar }
    public Modo modo = Modo.Caer;
    public bool seRompeAlCaer = true;      // gotas, escombros, estalactitas
    public bool aplasta = false;            // rocas
    public bool peligrosoAlCaer = false;    // estalactitas
    public Color colorPedazos = new Color(0.4f, 0.6f, 0.9f);

    private Congelable congelable;
    private Rigidbody2D rb;
    private Collider2D col;
    private Vector2 velocidadGuardada;
    private bool estabaVivo;
    private float temblor;
    private bool yaSeMovio;
    private Vector3 posTemblor;

    void Start()
    {
        congelable = GetComponent<Congelable>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public bool Cayendo()
    {
        return rb != null && rb.bodyType == RigidbodyType2D.Dynamic && rb.linearVelocity.y < -1f;
    }

    public bool PeligroActivo()
    {
        if (peligrosoAlCaer) return Cayendo();
        return true;
    }

    void Update()
    {
        bool vivo = congelable.vivo;

        if (modo == Modo.Caer)
        {
            if (vivo && !estabaVivo)
            {
                if (!yaSeMovio) { GameManager.instancia.Sonar("crujido"); temblor = peligrosoAlCaer ? 0.35f : 0.18f; posTemblor = transform.position; }   // tiembla antes de caer
                else Liberar();
            }
            if (!vivo && estabaVivo) Congelar();

            if (temblor > 0f)
            {
                temblor -= Time.deltaTime;
                transform.position = posTemblor + new Vector3(Random.Range(-0.05f, 0.05f), 0f, 0f);
                if (temblor <= 0f)
                {
                    transform.position = posTemblor;
                    yaSeMovio = true;
                    if (congelable.vivo) Liberar();
                }
            }
            if (transform.position.y < -12f) Destroy(gameObject);
        }
        else
        {
            if (vivo != estabaVivo)
            {
                col.enabled = !vivo;
                congelable.CambiarAlfa(vivo ? 0.45f : 1f);

                // Si se congela con Lumi adentro, Lumi queda atrapada en el hielo
                if (!vivo)
                {
                    Transform lumi = GameManager.instancia.LumiActual();
                    if (lumi != null && col.bounds.Intersects(lumi.GetComponent<Collider2D>().bounds))
                    {
                        lumi.GetComponent<Lumi>().Morir();
                    }
                }
            }
        }
        estabaVivo = vivo;
    }

    void Liberar()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 3f;
        rb.linearVelocity = velocidadGuardada;
    }

    void Congelar()
    {
        temblor = 0f;
        if (rb.bodyType == RigidbodyType2D.Dynamic)
        {
            velocidadGuardada = rb.linearVelocity;
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    void OnCollisionEnter2D(Collision2D otro) { Choque(otro.gameObject); }
    void OnTriggerEnter2D(Collider2D otro) { Choque(otro.gameObject); }

    void Choque(GameObject otro)
    {
        if (!seRompeAlCaer || rb == null || rb.bodyType != RigidbodyType2D.Dynamic) return;
        if (otro.GetComponent<Bloque>() == null && otro.GetComponent<ObjetoCongelado>() == null) return;
        if (rb.linearVelocity.y > -0.5f && !Cayendo()) return;
        GameManager.instancia.CrearPedazos(transform.position, colorPedazos, 5);
        GameManager.instancia.Sonar("romper");
        Destroy(gameObject);
    }
}
