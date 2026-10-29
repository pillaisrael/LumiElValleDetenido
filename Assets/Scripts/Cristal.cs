using UnityEngine;

// Cristal de las cuevas: si Lumi lo ilumina, refleja la luz hacia
// una plataforma lejana y la hace moverse por unos segundos.
public class Cristal : MonoBehaviour
{
    public float duracion = 8f;
    public PlataformaMovil objetivo;
    public Transform rayo;   // linea de luz que va hasta la plataforma

    private Congelable congelable;
    private bool estabaVivo;

    void Start()
    {
        congelable = GetComponent<Congelable>();
    }

    void Update()
    {
        if (objetivo == null) { rayo.gameObject.SetActive(false); return; }

        if (congelable.vivo) objetivo.energia = duracion;
        if (congelable.vivo && !estabaVivo) GameManager.instancia.Sonar("cristal");
        estabaVivo = congelable.vivo;

        bool activo = objetivo.energia > 0f;
        rayo.gameObject.SetActive(activo);
        if (activo)
        {
            Vector3 a = transform.position;
            Vector3 b = objetivo.transform.position;
            rayo.position = (a + b) / 2f;
            rayo.localScale = new Vector3(Vector3.Distance(a, b), 0.08f, 1f);
            rayo.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(b.y - a.y, b.x - a.x) * Mathf.Rad2Deg);
        }
    }
}
