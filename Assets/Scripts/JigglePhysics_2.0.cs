using UnityEngine;

public class JigglePhysics_2_0 : MonoBehaviour
{
    //public Transform anchor;

    //[Header("Fizyka")]
    //public float masa = 1.0f;
    //public float wspolczynnikSprezystosci = 60f;
    //public float wspolczynnikTlumienia = 8f;

    //[Header("Sztywność osi")]
    //public Vector3 sztywnoscOsi = new Vector3(1.0f, 0.6f, 1.3f);

    //[Header("Limity wychylenia lokalnego")]
    //public Vector3 maksymalneWychylenie = new Vector3(0.08f, 0.04f, 0.12f);

    //[Header("Wygładzanie przyspieszenia")]
    //public float czasWygładzaniaPrzyspieszenia = 0.05f;

    //Vector3 pozycjaSpoczynkowa;
    //Vector3 predkoscLokalna;

    //Vector3 ostatniaPozycjaAnchora;
    //Vector3 ostatniaPredkoscAnchora;
    //Vector3 przyspieszenieAnchoraWygładzone;

    void Start()
    {
        //pozycjaSpoczynkowa = transform.localPosition;
        //ostatniaPozycjaAnchora = anchor.position;
        //ostatniaPredkoscAnchora = Vector3.zero;
    }

    void LateUpdate()
    {
        //    float dt = Time.deltaTime;
        //    if (dt <= 0f) return;

        //    Vector3 aktualnaPozycjaAnchora = anchor.position;
        //    Vector3 predkoscAnchora =
        //        (aktualnaPozycjaAnchora - ostatniaPozycjaAnchora) / dt;

        //    Vector3 przyspieszenieSurowe =
        //        (predkoscAnchora - ostatniaPredkoscAnchora) / dt;

        //    przyspieszenieAnchoraWygładzone = Vector3.Lerp(
        //        przyspieszenieAnchoraWygładzone,
        //        przyspieszenieSurowe,
        //        dt / czasWygładzaniaPrzyspieszenia
        //    );

        //    ostatniaPozycjaAnchora = aktualnaPozycjaAnchora;
        //    ostatniaPredkoscAnchora = predkoscAnchora;

        //    Vector3 silaBezwladnosciWorld =
        //        -masa * przyspieszenieAnchoraWygładzone;

        //    Vector3 silaBezwladnosciLokalna =
        //        anchor.InverseTransformVector(silaBezwladnosciWorld);

        //    Vector3 wychylenie =
        //        transform.localPosition - pozycjaSpoczynkowa;

        //    Vector3 silaSprezyny = new Vector3(
        //        -wspolczynnikSprezystosci * sztywnoscOsi.x * wychylenie.x,
        //        -wspolczynnikSprezystosci * sztywnoscOsi.y * wychylenie.y,
        //        -wspolczynnikSprezystosci * sztywnoscOsi.z * wychylenie.z
        //    );

        //    Vector3 silaTlumienia =
        //        -wspolczynnikTlumienia * predkoscLokalna;

        //    Vector3 silaCalkowita =
        //        silaBezwladnosciLokalna + silaSprezyny + silaTlumienia;

        //    Vector3 przyspieszenie =
        //        silaCalkowita / masa;

        //    predkoscLokalna += przyspieszenie * dt;

        //    Vector3 nowaPozycja =
        //        transform.localPosition + predkoscLokalna * dt;


        //    Vector3 lokalneWychylenie =
        //        nowaPozycja - pozycjaSpoczynkowa;

        //    lokalneWychylenie.x = Mathf.Clamp(
        //        lokalneWychylenie.x,
        //        -maksymalneWychylenie.x,
        //         maksymalneWychylenie.x
        //    );

        //    lokalneWychylenie.y = Mathf.Clamp(
        //        lokalneWychylenie.y,
        //        -maksymalneWychylenie.y,
        //         maksymalneWychylenie.y
        //    );

        //    lokalneWychylenie.z = Mathf.Clamp(
        //        lokalneWychylenie.z,
        //        0f,
        //        maksymalneWychylenie.z
        //    );

        //    transform.localPosition =
        //        pozycjaSpoczynkowa + lokalneWychylenie;
    }
}
