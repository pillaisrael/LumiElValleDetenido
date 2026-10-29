using System.Collections.Generic;
using UnityEngine;

// Todo lo que da luz: Lumi, los faroles, los pilares y los cristales.
// "Donde no llega la luz, el tiempo se detiene."
public class FuenteLuz : MonoBehaviour
{
    public static List<FuenteLuz> todas = new List<FuenteLuz>();

    public float radio = 2.5f;
    public bool encendida = true;
    public SpriteRenderer halo;   // circulo de luz que se dibuja alrededor

    void OnEnable() { todas.Add(this); }
    void OnDisable() { todas.Remove(this); }

    void Update()
    {
        if (halo != null)
        {
            float r = encendida ? radio : 0f;
            float actual = halo.transform.localScale.x;
            float nuevo = Mathf.Lerp(actual, r * 2f, 15f * Time.deltaTime);
            halo.transform.localScale = new Vector3(nuevo, nuevo, 1f);
        }
    }

    // ¿Alguna luz toca este rectangulo?
    public static bool Ilumina(Bounds zona)
    {
        foreach (FuenteLuz f in todas)
        {
            if (!f.encendida || f.radio <= 0f) continue;
            Vector3 centro = f.transform.position;
            centro.z = zona.center.z;
            Vector3 cercano = zona.ClosestPoint(centro);
            if (Vector2.Distance(cercano, centro) <= f.radio) return true;
        }
        return false;
    }
}
