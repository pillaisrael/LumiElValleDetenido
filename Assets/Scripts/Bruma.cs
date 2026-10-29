using System.Collections.Generic;
using UnityEngine;

// LA BRUMA (nivel 10). Pelea como tu... porque ES tu:
// graba cada movimiento de Lumi y lo repite unos segundos despues.
// Si te alcanza, te congela. El DESTELLO la detiene un momento.
public class Bruma : MonoBehaviour
{
    public float[] retrasoPorPilar = { 1.9f, 1.5f, 1.2f };
    public Transform visual;

    struct Paso { public float tiempo; public Vector3 pos; public float mira; }
    private List<Paso> grabacion = new List<Paso>();
    private float reloj;          // tiempo de la grabacion
    private float reproduccion;   // momento que la Bruma esta repitiendo
    private bool activa;
    private bool lumiSeMovio;
    private Vector3 inicio;

    void Start()
    {
        inicio = transform.position;
        visual.gameObject.SetActive(false);
    }

    void Despertar()
    {
        activa = true;
        visual.gameObject.SetActive(true);
        GameManager.instancia.CrearPedazos(transform.position, new Color(0.25f, 0.1f, 0.4f), 10);
        GameManager.instancia.MostrarAviso("LA BRUMA DESPIERTA");
        GameManager.instancia.Sonar("bruma");
        Invoke(nameof(OcultarAviso), 1.2f);
    }

    void OcultarAviso()
    {
        GameManager.instancia.textoAviso.gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        Transform lumi = GameManager.instancia.LumiActual();
        if (lumi == null || !lumi.gameObject.activeInHierarchy) return;

        // Grabar a Lumi
        reloj += Time.fixedDeltaTime;
        Transform v = lumi.Find("Visual");
        grabacion.Add(new Paso { tiempo = reloj, pos = lumi.position, mira = v != null ? v.localScale.x : 1f });

        // Despierta cuando Lumi da su primer paso (y empieza a copiarla)
        if (!lumiSeMovio && Vector3.Distance(lumi.position, inicio) > 0.3f)
        {
            lumiSeMovio = true;
            reproduccion = reloj;
            Invoke(nameof(Despertar), retrasoPorPilar[0]);
        }

        if (!activa) return;

        // El destello la congela un momento
        if (GameManager.instancia.DestelloActivo()) return;

        float retraso = retrasoPorPilar[Mathf.Clamp(GameManager.instancia.pilaresEncendidos, 0, retrasoPorPilar.Length - 1)];
        float objetivo = reloj - retraso;
        // Si se quedo atras, corre un poco mas rapido para alcanzarte
        float velocidad = (objetivo - reproduccion) > 0.3f ? 1.5f : 1f;
        reproduccion = Mathf.Min(reproduccion + Time.fixedDeltaTime * velocidad, objetivo);

        // Buscar en la grabacion la posicion de ese momento
        while (grabacion.Count > 2 && grabacion[1].tiempo <= reproduccion) grabacion.RemoveAt(0);
        if (grabacion.Count > 0 && grabacion[0].tiempo <= reproduccion)
        {
            transform.position = grabacion[0].pos;
            visual.localScale = new Vector3(grabacion[0].mira, 1f, 1f);
        }
    }

    void OnTriggerStay2D(Collider2D otro)
    {
        if (!activa) return;
        Lumi l = otro.GetComponent<Lumi>();
        if (l != null) l.Morir();
    }

    public void Desaparecer()
    {
        GameManager.instancia.CrearPedazos(transform.position, new Color(0.25f, 0.1f, 0.4f), 20);
        Destroy(gameObject);
    }
}
