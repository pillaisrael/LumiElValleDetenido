using UnityEngine;

// Un faro apagado. Si Lumi lo toca, se enciende y el tiempo vuelve en esa parte del valle.
// (En el nivel de Nadie, esta misma meta es la chispa de Lumi.)
public class PuertaFaro : MonoBehaviour
{
    public SpriteRenderer luz;

    public void Encender()
    {
        if (luz != null) luz.color = Color.white;
    }
}
