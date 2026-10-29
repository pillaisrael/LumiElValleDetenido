using UnityEngine;

// Si un script implementa esto, decide cuando su objeto es peligroso
// (por ejemplo: una sombra congelada es una estatua y no hace dano).
public interface ICondicionPeligro
{
    bool PeligroActivo();
}

// Marca: si Lumi toca un objeto con este componente, se congela y pierde.
public class Peligro : MonoBehaviour
{
    public bool Activo()
    {
        foreach (ICondicionPeligro c in GetComponents<ICondicionPeligro>())
        {
            if (!c.PeligroActivo()) return false;
        }
        return true;
    }
}
