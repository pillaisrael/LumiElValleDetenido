using UnityEngine;

// "EL JUEGO SE APAGA": cada cierto tiempo la pantalla falla
// y la luz de Lumi se apaga por un momento.
public class Glitch : MonoBehaviour
{
    private float espera;

    void Start()
    {
        espera = Random.Range(4f, 6f);
    }

    void Update()
    {
        espera -= Time.deltaTime;
        if (espera <= 0f)
        {
            espera = Random.Range(6f, 9f);
            GameManager.instancia.Falla();
        }
    }
}
