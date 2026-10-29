using UnityEngine;

// Nadie: la sombrita de un ojo que acompana a Lumi.
// Le duele la luz, asi que flota al borde del brillo de Lumi.
public class NadieCompanero : MonoBehaviour
{
    public Transform pupila;
    private Vector3 posPupila;

    void Start()
    {
        if (pupila != null) posPupila = pupila.localPosition;
    }

    void Update()
    {
        Transform lumi = GameManager.instancia.LumiActual();
        if (lumi == null || !lumi.gameObject.activeInHierarchy) return;

        Transform visual = lumi.Find("Visual");
        float mira = visual != null ? Mathf.Sign(visual.localScale.x) : 1f;
        Vector3 objetivo = lumi.position + new Vector3(-mira * 1.3f, 0.9f + Mathf.Sin(Time.time * 3f) * 0.12f, 0f);
        transform.position = Vector3.Lerp(transform.position, objetivo, 4f * Time.deltaTime);

        if (pupila != null)
        {
            Vector3 haciaLumi = (lumi.position - transform.position).normalized;
            pupila.localPosition = posPupila + haciaLumi * 0.05f;
        }
    }
}
