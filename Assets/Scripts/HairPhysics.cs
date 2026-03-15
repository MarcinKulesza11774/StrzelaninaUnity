using UnityEngine;

/// <summary>
/// Fizyka włosów – jeden skrypt na korzeń łańcucha, reszta automatycznie.
///
/// SETUP:
///   1. Dodaj skrypt do pierwszej kości włosów (nasada, np. hair_root)
///   2. anchor = rodzic tej kości (np. kość głowy)
///   3. Skrypt automatycznie wykrywa wszystkie kości-dzieci w dół
///   4. Opcjonalnie: wrzuć SphereCollider głowy do sferyKolizji
/// </summary>
public class HairPhysics : MonoBehaviour
{
    [Header("Anchor (kość głowy / rodzic nasady)")]
    public Transform anchor;

    [Header("── Sprężyna ──────────────────────────────────────────────")]
    [Tooltip("Sztywność u nasady (przy głowie). Zacznij od 80–120.")]
    [Range(1f, 300f)] public float sztywnosc = 100f;

    [Tooltip("Sztywność na końcu włosa – zazwyczaj dużo mniej niż nasada. Zacznij od 10–20.")]
    [Range(1f, 300f)] public float sztywnosc_koniec = 15f;

    [Tooltip("Tłumienie drgań. Zacznij od 4–8.")]
    [Range(0.1f, 40f)] public float tlumienie = 5f;

    [Header("── Bezwładność ───────────────────────────────────────────")]
    [Tooltip("Jak mocno włos reaguje na ruch głowy. Zacznij od 0.01.")]
    [Range(0f, 1f)] public float skalaBezwladnosci = 0.01f;

    [Tooltip("Wygładzanie przyspieszenia. Zacznij od 0.05.")]
    [Range(0.01f, 0.3f)] public float wygładzanie = 0.05f;

    [Header("── Limity wychylenia (dla MMD zacznij od 0.003–0.008) ──")]
    [Tooltip("Nasada – mniejsze limity bo głowa blisko.")]
    public Vector3 limit_nasada = new Vector3(0.003f, 0.002f, 0.003f);

    [Tooltip("Koniec – większe limity bo końce są luźne.")]
    public Vector3 limit_koniec = new Vector3(0.008f, 0.005f, 0.008f);

    [Header("── Kolizja ze sferami ────────────────────────────────────")]
    public SphereCollider[] sferyKolizji;

    // ── Stan per kość ──────────────────────────────────────────────────────
    struct KoscStan
    {
        public Transform transform;
        public Vector3 pozycjaSpoczynkowa;
        public Vector3 predkoscLokalna;
        public float sztywnosc;
        public Vector3 limit;
    }

    KoscStan[] kosci;
    Vector3 ostatniaPozycjaAnchora;
    Vector3 ostatniaPredkoscAnchora;
    Vector3 przyspieszenieWygladzone;

    // ══════════════════════════════════════════════════════════════════════
    void Start()
    {
        if (anchor == null)
        {
            Debug.LogError("[HairPhysics] Brak Anchor!", this);
            enabled = false;
            return;
        }

        // Zbierz łańcuch: ta kość + wszystkie dzieci w dół (jedna gałąź)
        var lista = new System.Collections.Generic.List<Transform>();
        Transform current = transform;
        while (current != null)
        {
            lista.Add(current);
            current = current.childCount > 0 ? current.GetChild(0) : null;
        }

        int n = lista.Count;
        kosci = new KoscStan[n];

        for (int i = 0; i < n; i++)
        {
            float t = n > 1 ? (float)i / (n - 1) : 0f;
            kosci[i] = new KoscStan
            {
                transform = lista[i],
                pozycjaSpoczynkowa = lista[i].localPosition,
                predkoscLokalna = Vector3.zero,
                sztywnosc = Mathf.Lerp(sztywnosc, sztywnosc_koniec, t),
                limit = Vector3.Lerp(limit_nasada, limit_koniec, t)
            };
        }

        ostatniaPozycjaAnchora = anchor.position;
        ostatniaPredkoscAnchora = Vector3.zero;
    }

    void LateUpdate()
    {
        float dt = Time.deltaTime;
        if (dt <= 0f || kosci == null) return;

        // ── Przyspieszenie anchora (wspólne dla całego łańcucha) ───────────
        Vector3 aktualnaPos = anchor.position;
        Vector3 predkosc = (aktualnaPos - ostatniaPozycjaAnchora) / dt;
        Vector3 przyspieszenie = (predkosc - ostatniaPredkoscAnchora) / dt;

        float alpha = 1f - Mathf.Exp(-dt / Mathf.Max(wygładzanie, 0.0001f));
        przyspieszenieWygladzone = Vector3.Lerp(
            przyspieszenieWygladzone, przyspieszenie, alpha);

        ostatniaPozycjaAnchora = aktualnaPos;
        ostatniaPredkoscAnchora = predkosc;

        // ── Symulacja każdej kości ─────────────────────────────────────────
        for (int i = 0; i < kosci.Length; i++)
        {
            ref KoscStan k = ref kosci[i];

            // Rodzic tej kości – anchor dla kości 0, poprzednia kość dla reszty
            Transform rodzic = i == 0 ? anchor : kosci[i - 1].transform;

            Vector3 bezwladnosc = rodzic.InverseTransformDirection(
                -przyspieszenieWygladzone * skalaBezwladnosci);

            Vector3 wychylenie = k.transform.localPosition - k.pozycjaSpoczynkowa;
            Vector3 sila = -k.sztywnosc * wychylenie
                                 - tlumienie * k.predkoscLokalna
                                 + bezwladnosc;

            k.predkoscLokalna += sila * dt;
            Vector3 nowaPozycja = k.transform.localPosition + k.predkoscLokalna * dt;
            Vector3 lokalneWychylenie = nowaPozycja - k.pozycjaSpoczynkowa;

            // Clamp + reset prędkości
            if (Mathf.Abs(lokalneWychylenie.x) > k.limit.x)
            {
                lokalneWychylenie.x = Mathf.Sign(lokalneWychylenie.x) * k.limit.x;
                k.predkoscLokalna.x = 0f;
            }
            if (Mathf.Abs(lokalneWychylenie.y) > k.limit.y)
            {
                lokalneWychylenie.y = Mathf.Sign(lokalneWychylenie.y) * k.limit.y;
                k.predkoscLokalna.y = 0f;
            }
            if (Mathf.Abs(lokalneWychylenie.z) > k.limit.z)
            {
                lokalneWychylenie.z = Mathf.Sign(lokalneWychylenie.z) * k.limit.z;
                k.predkoscLokalna.z = 0f;
            }

            k.transform.localPosition = k.pozycjaSpoczynkowa + lokalneWychylenie;

            // Kolizja ze sferami
            if (sferyKolizji == null) continue;
            foreach (var sfera in sferyKolizji)
            {
                if (sfera == null) continue;
                float promien = sfera.radius * sfera.transform.lossyScale.x;
                Vector3 centrum = sfera.transform.TransformPoint(sfera.center);
                Vector3 delta = k.transform.position - centrum;
                float dist = delta.magnitude;
                if (dist < promien)
                {
                    k.transform.position += delta.normalized * (promien - dist);
                    Vector3 vWorld = rodzic.TransformDirection(k.predkoscLokalna);
                    vWorld -= Vector3.Project(vWorld, delta.normalized);
                    k.predkoscLokalna = rodzic.InverseTransformDirection(vWorld);
                }
            }
        }
    }
}