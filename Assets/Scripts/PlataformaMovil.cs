using UnityEngine;

// Ascensores y relojes: solo avanzan mientras la luz los toca
// (o mientras un cristal les manda luz).
public class PlataformaMovil : MonoBehaviour
{
    public Vector2 recorrido = new Vector2(0f, 3f);
    public float velocidad = 3f;
    public Transform manecilla;   // solo los relojes

    public Vector2 velocidadActual { get; private set; }
    [HideInInspector] public float energia;   // segundos de luz que le dio un cristal

    private Rigidbody2D rb;
    private Congelable congelable;
    private Vector2 inicio;
    private float t;          // 0 = inicio, 1 = final
    private float sentido = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        congelable = GetComponent<Congelable>();
        inicio = rb.position;
    }

    void FixedUpdate()
    {
        if (energia > 0f) energia -= Time.fixedDeltaTime;
        congelable.forzarVivo = energia > 0f;
        bool mover = congelable.vivo || energia > 0f;

        if (!mover)
        {
            velocidadActual = Vector2.zero;
            return;
        }

        t += sentido * velocidad * Time.fixedDeltaTime / recorrido.magnitude;
        if (t >= 1f) { t = 1f; sentido = -1f; }
        if (t <= 0f) { t = 0f; sentido = 1f; }

        Vector2 nueva = inicio + recorrido * t;
        velocidadActual = (nueva - rb.position) / Time.fixedDeltaTime;
        rb.MovePosition(nueva);

        if (manecilla != null) manecilla.Rotate(0f, 0f, -360f * Time.fixedDeltaTime);
    }
}
