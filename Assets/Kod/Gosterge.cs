using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

[DefaultExecutionOrder(1000)]
public class Gosterge : MonoBehaviour
{
	public static List<Gosterge> tumGostergeler;
	RectTransform gosterge; // Hedefin bulunduğu yönün atanacağı gösterge
	Image gostergeImaj;

	public RectTransform mainCanvas;
	public Transform hedef;
    public Transform oyuncu;
	public float padding = 45;

	Vector2 yon; // Hedefin yonu
    Vector3 vektor; // Ara deger (temp)
	Vector3 targetPositionScreenPoint;
	Vector3 aci = Vector3.zero;
	Vector2 gidilecekKonum = Vector2.zero;

	float borderSize = 40f;
	float ekranOrani;
	float genislik, yukseklik;
	bool ikiFPSteBir = true;

	Camera main;
    private void Awake()
    {
        if (tumGostergeler == null)
        {
			tumGostergeler = new List<Gosterge>();
        }
		tumGostergeler.Add(this);
    }
    void Start()
    {
		main = Camera.main;
		ekranOrani = OyunKontrol.ekranOrani;
		genislik = Screen.width;
		yukseklik = Screen.height;

		oyuncu = OyunKontrol.ok.oyuncu.transform;
        gosterge = GetComponent<RectTransform>();
		mainCanvas = transform.parent.GetComponent<RectTransform>();
		gostergeImaj = GetComponent<Image>();

		borderSize *= ekranOrani;
		transform.parent.localScale = Vector3.one;
		//transform.localScale *= ekranOrani;
		GetComponent<RectTransform>().sizeDelta *= ekranOrani;

		Kapatma();
	}
   
    void FixedUpdate()
    {
		if (hedef == null)
			return;
		if (!ikiFPSteBir)
		{
			ikiFPSteBir = true;
			return;
		}
		ikiFPSteBir = false;


        
        targetPositionScreenPoint = main.WorldToScreenPoint(hedef.position);

        if (targetPositionScreenPoint.x <= borderSize || targetPositionScreenPoint.x >= genislik - borderSize || targetPositionScreenPoint.y <= borderSize || targetPositionScreenPoint.y >= yukseklik - borderSize)
        {
			if(OyunKontrol.oyunda)
				gostergeImaj.enabled = true;
        }
        else
		{
			Kapatma();
		}






		targetPositionScreenPoint = main.WorldToViewportPoint(hedef.position);
		

		if (targetPositionScreenPoint.z < 0)
		{

			targetPositionScreenPoint = Vector3Invert(targetPositionScreenPoint);
			targetPositionScreenPoint = Vector3FixEdge(targetPositionScreenPoint);
		}
		targetPositionScreenPoint = main.ViewportToScreenPoint(targetPositionScreenPoint);
		KeepCameraInside(targetPositionScreenPoint);

		//Vector3 dir=gosterge.anchoredPosition.normalized;
		//gosterge.position -= (Vector3)gosterge.anchoredPosition.normalized * 90 * ekranOrani;


		gosterge.anchoredPosition = Vector3.Lerp(gosterge.anchoredPosition, gidilecekKonum, Time.deltaTime * 10);


		vektor = hedef.position - oyuncu.position;
        yon.x = vektor.x;
        yon.y = vektor.z;
        yon = yon.normalized;
		aci.z = -1 * Mathf.Rad2Deg * Mathf.Atan2(yon.x, yon.y);
		//unlem.eulerAngles = aci;

		gosterge.rotation = Quaternion.Lerp(gosterge.rotation, Quaternion.Euler(aci), Time.deltaTime * 5);
	}

	//İmageyi ve unlem textini kapatır
	public void Kapatma()
	{
		if (gostergeImaj)
		{
			gostergeImaj.enabled = false;
		}
	}
	private void KeepCameraInside(Vector2 reference)
	{
		//Mantendo o objeto dentro do Tela somando com o padding
		reference.x = Mathf.Clamp(reference.x, padding, genislik - padding) - Screen.width/2;
		reference.y = Mathf.Clamp(reference.y, padding, yukseklik - padding) - Screen.height/2;
		gidilecekKonum = reference;
	}
	Vector3 Vector3Invert(Vector3 viewport_position)
	{
		Vector3 invertedVector = viewport_position;
		//Inverte position com base na dimensão da tela
		invertedVector.x = 1f - invertedVector.x;
		invertedVector.y = 1f - invertedVector.y;
		invertedVector.z = 0;
		return invertedVector;
	}
	public Vector3 Vector3FixEdge(Vector3 vector)
	{
		Vector3 vectorFixed = vector;

		float highestValue = Vector3Max(vectorFixed);

		float lowerValue = Vector3Min(vectorFixed);


		float highestValueBetween = DirectionPreference(lowerValue, highestValue);



		if (highestValueBetween == highestValue)
		{

			vectorFixed.x = vectorFixed.x == highestValue ? 1 : vectorFixed.x;
			vectorFixed.y = vectorFixed.y == highestValue ? 1 : vectorFixed.y;

		}


		if (highestValueBetween == lowerValue)
		{

			vectorFixed.x = Mathf.Abs(vectorFixed.x) == lowerValue ? 0 : Mathf.Abs(vectorFixed.x);
			vectorFixed.y = Mathf.Abs(vectorFixed.y) == lowerValue ? 0 : Mathf.Abs(vectorFixed.y);
		}
		return vectorFixed;
	}
	float Vector3Max(Vector3 vector)
	{

		//float highestValue = ;
		return Mathf.Max(vector.x, vector.y);
	}
	float Vector3Min(Vector3 vector)
	{
		float lowerValue = 0f;
		lowerValue = vector.x <= lowerValue ? vector.x : lowerValue;
		lowerValue = vector.y <= lowerValue ? vector.y : lowerValue;

		return lowerValue;
	}

	float DirectionPreference(float lowerValue, float highestValue)
	{

		lowerValue = Mathf.Abs(lowerValue);
		highestValue = Mathf.Abs(highestValue);


		//float highestValueBetween = ;

		return Mathf.Max(lowerValue, highestValue);
	}
}
