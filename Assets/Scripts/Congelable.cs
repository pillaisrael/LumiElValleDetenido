using UnityEngine;

// Lo que esta dentro de la luz VIVE (se mueve y tiene sus colores).
// Lo que queda en sombra se CONGELA (se queda quieto y se pone azul).
public class Congelable : MonoBehaviour
{
    public Color tinteHielo = new Color(0.66f, 0.84f, 1f);
    public bool celdaFija = false;   // usa siempre la misma zona (para las espinas)
    public bool cambiarColor = true;

    public bool vivo { get; private set; }
    [HideInInspector] public bool forzarVivo;   // un cristal le manda luz

    private SpriteRenderer[] partes;
    private Color[] colores;
    private Animator anim;
    private bool primeraVez = true;
    private Bounds zonaFija;

    void Awake()
    {
        partes = GetComponentsInChildren<SpriteRenderer>(true);
        colores = new Color[partes.Length];
        for (int i = 0; i < partes.Length; i++) colores[i] = partes[i].color;
        anim = GetComponent<Animator>();
        zonaFija = new Bounds(transform.position, new Vector3(1f, 1f, 1f));
    }

    public Bounds Zona()
    {
        if (celdaFija) return zonaFija;
        Bounds b = new Bounds(transform.position, Vector3.zero);
        bool primero = true;
        foreach (SpriteRenderer sr in partes)
        {
            if (sr == null || EsLuz(sr)) continue;
            if (primero) { b = sr.bounds; primero = false; }
            else b.Encapsulate(sr.bounds);
        }
        return b;
    }

    void Update()
    {
        bool v = forzarVivo || FuenteLuz.Ilumina(Zona());
        if (v != vivo || primeraVez)
        {
            primeraVez = false;
            vivo = v;
            Pintar();
        }
    }

    void Pintar()
    {
        if (anim != null) anim.speed = vivo ? 1f : 0f;
        if (!cambiarColor) return;
        for (int i = 0; i < partes.Length; i++)
        {
            if (partes[i] == null || EsLuz(partes[i])) continue;
            Color c = vivo ? colores[i] : Color.Lerp(colores[i], tinteHielo, 0.65f);
            c.a = partes[i].color.a;
            partes[i].color = c;
        }
        if (anim != null) anim.speed = vivo ? 1f : 0f;
    }

    // Los halos y rayos de luz no cuentan como parte del objeto
    bool EsLuz(SpriteRenderer sr)
    {
        return sr.gameObject.name == "Halo" || sr.gameObject.name == "Rayo";
    }

    // Para scripts que cambian la transparencia
    public void CambiarAlfa(float a)
    {
        foreach (SpriteRenderer sr in partes)
        {
            if (sr == null || EsLuz(sr)) continue;
            Color c = sr.color; c.a = a; sr.color = c;
        }
    }
}
