using UnityEngine;

// Borra el objeto despues de un tiempo (para los pedazos de Lumi).
public class AutoDestruir : MonoBehaviour
{
    public float tiempo = 1.2f;

    void Start()
    {
        Destroy(gameObject, tiempo);
    }
}
