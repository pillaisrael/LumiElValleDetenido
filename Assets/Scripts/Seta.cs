using UnityEngine;

// Seta saltarina del bosque: lanza a Lumi muy alto... solo si esta viva (con luz).
public class Seta : MonoBehaviour
{
    public float fuerza = 19f;
    public Transform sombrero;

    private Congelable congelable;
    private float animacion;

    void Start()
    {
        congelable = GetComponent<Congelable>();
    }

    void OnTriggerStay2D(Collider2D otro)
    {
        if (!congelable.vivo || animacion > 0.1f) return;
        Lumi l = otro.GetComponent<Lumi>();
        if (l == null) return;
        l.Rebotar(fuerza);
        animacion = 0.3f;
    }

    void Update()
    {
        if (animacion > 0f)
        {
            animacion -= Time.deltaTime;
            float estirado = 1f + Mathf.Sin(animacion * 30f) * animacion;
            sombrero.localScale = new Vector3(estirado, 1f / estirado, 1f);
        }
        else
        {
            sombrero.localScale = Vector3.one;
        }
    }
}
