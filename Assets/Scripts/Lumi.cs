using UnityEngine;

// Lumi: la ultima chispa del Gran Faro. Su luz hace que el tiempo corra.
// Con Q contiene su luz (todo se congela), con SHIFT hace un destello.
// Este mismo script controla a NADIE cuando lo juegas (esNadie = true):
// Nadie no tiene luz y la luz lo quema.
public class Lumi : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 7f;
    public float fuerzaSalto = 13f;
    public float multiplicadorCaida = 2f;   // cae mas rapido de lo que sube
    public float velocidadMaxCaida = 20f;

    [Header("Ayudas para que el salto se sienta justo")]
    public float tiempoCoyote = 0.13f;
    public float tiempoBuffer = 0.13f;

    [Header("Luz")]
    public bool esNadie = false;
    public float radioNormal = 2.6f;
    public float radioContenido = 0.3f;
    public float radioDestello = 40f;
    public float duracionDestello = 0.6f;
    public float esperaDestello = 2f;

    [Header("Partes")]
    public Transform visual;

    private Rigidbody2D rb;
    private Animator anim;
    private FuenteLuz luz;
    private Collider2D col;
    private float movX;
    private bool enSuelo;
    private bool estabaEnSuelo = true;
    private float caidaMasRapida;
    private float contadorCoyote;
    private float contadorBuffer;
    private bool terminado;
    private bool rebotando;
    private float destello;
    private float recarga;
    private Vector2 velPlataforma;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        luz = GetComponent<FuenteLuz>();
        col = GetComponent<Collider2D>();
    }

    public bool DestelloActivo() { return destello > 0f; }

    void Update()
    {
        if (terminado) return;

        movX = Entrada.Horizontal();

        // ----- LUZ -----
        if (!esNadie)
        {
            if (recarga > 0f) recarga -= Time.deltaTime;
            if (destello > 0f) destello -= Time.deltaTime;
            if (Entrada.Destello() && recarga <= 0f)
            {
                destello = duracionDestello;
                recarga = esperaDestello;
                GameManager.instancia.Sonar("destello");
                GameManager.instancia.CrearPedazos(transform.position, new Color(1f, 0.9f, 0.5f), 8);
            }

            if (Entrada.ContenerPresionado()) GameManager.instancia.Sonar("contener");
            if (destello > 0f) luz.radio = radioDestello;
            else if (Entrada.Contener() || GameManager.instancia.luzApagada) luz.radio = radioContenido;
            else luz.radio = radioNormal;
        }
        else
        {
            // A Nadie le duele la luz
            if (FuenteLuz.Ilumina(new Bounds(transform.position, new Vector3(0.3f, 0.3f, 0f)))) { Morir(); return; }
        }

        // ----- SALTO -----
        if (enSuelo) contadorCoyote = tiempoCoyote;
        else contadorCoyote -= Time.deltaTime;

        if (Entrada.SaltoPresionado()) contadorBuffer = tiempoBuffer;
        else contadorBuffer -= Time.deltaTime;

        if (contadorBuffer > 0f && contadorCoyote > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
            contadorBuffer = 0f;
            contadorCoyote = 0f;
            enSuelo = false;
            GameManager.instancia.CrearPolvo(transform.position + Vector3.down * 0.45f, 2);
            GameManager.instancia.Sonar("salto");
        }

        if (rb.linearVelocity.y <= 0f) rebotando = false;
        if (Entrada.SaltoSoltado() && rb.linearVelocity.y > 0f && !rebotando)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }

        // Polvo al aterrizar
        if (!enSuelo) caidaMasRapida = Mathf.Min(caidaMasRapida, rb.linearVelocity.y);
        if (enSuelo && !estabaEnSuelo)
        {
            if (caidaMasRapida < -6f)
            {
                GameManager.instancia.CrearPolvo(transform.position + Vector3.down * 0.45f, 4);
                GameManager.instancia.Sonar("aterrizar");
            }
            caidaMasRapida = 0f;
        }
        estabaEnSuelo = enSuelo;

        // Mirar hacia donde camina
        if (movX > 0f) visual.localScale = new Vector3(1f, 1f, 1f);
        else if (movX < 0f) visual.localScale = new Vector3(-1f, 1f, 1f);

        // Animaciones
        anim.SetFloat("Velocidad", Mathf.Abs(movX));
        anim.SetFloat("VelY", rb.linearVelocity.y - velPlataforma.y);
        anim.SetBool("EnSuelo", enSuelo);

        if (transform.position.y < -11f) Morir();
    }

    void FixedUpdate()
    {
        Vector2 plataforma = velPlataforma;
        enSuelo = false;          // OnCollisionStay2D lo vuelve a poner en true
        velPlataforma = Vector2.zero;
        if (terminado) return;

        Vector2 v = rb.linearVelocity;
        v.x = movX * velocidad + plataforma.x;

        // Encima de un ascensor que baja, bajar con el
        if (plataforma.y < 0f && v.y <= 0.1f) v.y = plataforma.y;

        if (v.y < 0f && plataforma.y >= 0f)
        {
            v.y += Physics2D.gravity.y * rb.gravityScale * (multiplicadorCaida - 1f) * Time.fixedDeltaTime;
        }
        if (v.y < -velocidadMaxCaida) v.y = -velocidadMaxCaida;

        rb.linearVelocity = v;
    }

    void OnCollisionStay2D(Collision2D otro)
    {
        for (int i = 0; i < otro.contactCount; i++)
        {
            if (otro.GetContact(i).normal.y > 0.5f)
            {
                PlataformaMovil pm = otro.gameObject.GetComponent<PlataformaMovil>();
                if (pm != null) velPlataforma = pm.velocidadActual;
                if (rb.linearVelocity.y <= 0.1f + Mathf.Max(0f, velPlataforma.y))
                {
                    enSuelo = true;
                    return;
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D otro)
    {
        // Una roca que cae encima aplasta a Lumi
        ObjetoCongelado o = otro.gameObject.GetComponent<ObjetoCongelado>();
        if (o != null && o.aplasta && o.Cayendo())
        {
            for (int i = 0; i < otro.contactCount; i++)
            {
                if (otro.GetContact(i).normal.y < -0.5f) { Morir(); return; }
            }
        }
    }

    void OnTriggerStay2D(Collider2D otro)
    {
        if (terminado) return;

        Peligro p = otro.GetComponent<Peligro>();
        if (p != null && p.Activo())
        {
            Morir();
            return;
        }

        PuertaFaro faro = otro.GetComponent<PuertaFaro>();
        if (faro != null)
        {
            terminado = true;
            faro.Encender();
            GameManager.instancia.LlegarAlFaro();
            gameObject.SetActive(false);   // entra al faro
        }
    }

    // Lo usa la seta saltarina
    public void Rebotar(float fuerza)
    {
        if (terminado) return;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerza);
        rebotando = true;
        GameManager.instancia.Sonar("seta");
        contadorCoyote = 0f;
        enSuelo = false;
    }

    public void Morir()
    {
        if (terminado) return;
        terminado = true;
        GameManager.instancia.LumiMurio(transform.position, esNadie);
        Destroy(gameObject);
    }
}
