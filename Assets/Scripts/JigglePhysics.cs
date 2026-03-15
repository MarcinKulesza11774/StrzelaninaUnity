using UnityEngine;

public class JigglePhysics : MonoBehaviour
{
    public Transform anchor;

    [Header("Fizyka")]
    public float masa = 0.1f;
    public float wspolczynnikSprezystosci = 15f;
    public float wspolczynnikTlumienia = 2f;

    [Header("Skala bezwładności (zacznij od 0.01, zwiększaj powoli)")]
    [Range(0f, 1f)]
    public float skalaBezwladnosci = 0.01f;

    [Header("Sztywność osi")]
    public Vector3 sztywnoscOsi = new Vector3(1.0f, 0.6f, 1.3f);

    [Header("Limity wychylenia lokalnego")]
    public Vector3 maksymalneWychylenie = new Vector3(0.0005f, 0.0005f, 0.0005f);

    [Header("Wygładzanie przyspieszenia")]
    public float czasWygladzaniaPrzyspieszenia = 0.1f;

    Vector3 pozycjaSpoczynkowa;
    Vector3 predkoscLokalna;
    Vector3 ostatniaPozycjaAnchora;
    Vector3 ostatniaPredkoscAnchora;
    Vector3 przyspieszenieAnchoraWygladzone;

    void Start()
    {
        pozycjaSpoczynkowa = transform.localPosition;
        ostatniaPozycjaAnchora = anchor.position;
        ostatniaPredkoscAnchora = Vector3.zero;
    }

    void LateUpdate()
    {
        float dt = Time.deltaTime;
        if (dt <= 0f) return;

        Vector3 aktualnaPozycjaAnchora = anchor.position;
        Vector3 predkoscAnchora =
            (aktualnaPozycjaAnchora - ostatniaPozycjaAnchora) / dt;
        Vector3 przyspieszenieSurowe =
            (predkoscAnchora - ostatniaPredkoscAnchora) / dt;

        float alpha = 1f - Mathf.Exp(-dt / Mathf.Max(czasWygladzaniaPrzyspieszenia, 0.0001f));
        przyspieszenieAnchoraWygladzone = Vector3.Lerp(
            przyspieszenieAnchoraWygladzone,
            przyspieszenieSurowe,
            alpha
        );

        ostatniaPozycjaAnchora = aktualnaPozycjaAnchora;
        ostatniaPredkoscAnchora = predkoscAnchora;

        // skalaBezwladnosci pozwala przyciąć siłę bez ruszania reszty parametrów
        Vector3 silaBezwladnosciWorld = -masa * przyspieszenieAnchoraWygladzone * skalaBezwladnosci;
        Vector3 silaBezwladnosciLokalna = anchor.InverseTransformDirection(silaBezwladnosciWorld);

        Vector3 wychylenie = transform.localPosition - pozycjaSpoczynkowa;
        Vector3 silaSprezyny = new Vector3(
            -wspolczynnikSprezystosci * sztywnoscOsi.x * wychylenie.x,
            -wspolczynnikSprezystosci * sztywnoscOsi.y * wychylenie.y,
            -wspolczynnikSprezystosci * sztywnoscOsi.z * wychylenie.z
        );
        Vector3 silaTlumienia = -wspolczynnikTlumienia * predkoscLokalna;

        Vector3 silaCalkowita = silaBezwladnosciLokalna + silaSprezyny + silaTlumienia;
        Vector3 przyspieszenie = silaCalkowita / masa;

        predkoscLokalna += przyspieszenie * dt;
        Vector3 nowaPozycja = transform.localPosition + predkoscLokalna * dt;
        Vector3 lokalneWychylenie = nowaPozycja - pozycjaSpoczynkowa;

        if (Mathf.Abs(lokalneWychylenie.x) > maksymalneWychylenie.x)
        {
            lokalneWychylenie.x = Mathf.Sign(lokalneWychylenie.x) * maksymalneWychylenie.x;
            predkoscLokalna.x = 0f;
        }
        if (Mathf.Abs(lokalneWychylenie.y) > maksymalneWychylenie.y)
        {
            lokalneWychylenie.y = Mathf.Sign(lokalneWychylenie.y) * maksymalneWychylenie.y;
            predkoscLokalna.y = 0f;
        }
        if (Mathf.Abs(lokalneWychylenie.z) > maksymalneWychylenie.z)
        {
            lokalneWychylenie.z = Mathf.Sign(lokalneWychylenie.z) * maksymalneWychylenie.z;
            predkoscLokalna.z = 0f;
        }

        transform.localPosition = pozycjaSpoczynkowa + lokalneWychylenie;
    }
}