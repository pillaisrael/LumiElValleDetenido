using UnityEngine;

// Parece un faro, pero es una sombra disfrazada (tiene el componente Peligro).
// De vez en cuando su luz se pone morada y abre un ojo: esa es la pista.
public class FaroFalso : MonoBehaviour
{
    public SpriteRenderer luz;
    public GameObject ojo;
    public Color colorMorado = new Color(0.6f, 0.35f, 0.85f);
    public float cadaCuanto = 2.5f;

    private Color colorNormal;
    private float tiempo;

    void Start()
    {
        colorNormal = luz.color;
        ojo.SetActive(false);
    }

    void Update()
    {
        tiempo += Time.deltaTime;
        bool delatado = (tiempo % cadaCuanto) > cadaCuanto - 0.35f;
        luz.color = delatado ? colorMorado : colorNormal;
        ojo.SetActive(delatado);
    }
}
