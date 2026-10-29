using UnityEngine;

// Abuela Vela: congelada a media frase. Solo habla mientras Lumi la ilumina.
public class AbuelaVela : MonoBehaviour
{
    [HideInInspector] public string frase = "...";
    private Congelable congelable;
    private bool hablando;

    void Start()
    {
        congelable = GetComponent<Congelable>();
    }

    void Update()
    {
        if (congelable.vivo && !hablando)
        {
            hablando = true;
            GameManager.instancia.Sonar("abuela");
            GameManager.instancia.MostrarDialogo("ABUELA VELA:  " + frase);
        }
        else if (!congelable.vivo && hablando)
        {
            hablando = false;
            GameManager.instancia.MostrarDialogo("");
        }
    }
}
