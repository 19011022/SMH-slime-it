using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class OyuncuHareket : MonoBehaviour
{
    public Animator animator;
    public Transform skillParent;

    public bool vuruyor;
    Rigidbody rb;

    public float donmeHiz;
    public float ileriHiz;

    float hiz;
    bool basiliyor;
    float vurmaLayerWeight;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OyunBasladi()
    {
        enabled = true;
        KameraTakip.kt.OyunBasladi();
    }

    private void Update()
    {
        //vuruyor = Input.GetMouseButton(1);
        Girdi();

        vurmaLayerWeight = Mathf.Lerp(vurmaLayerWeight, vuruyor ? 1 : 0, Time.deltaTime * 2);

        animator.SetBool("Vuruyor", vuruyor);
        animator.SetLayerWeight(1, vurmaLayerWeight);

        skillParent.eulerAngles = Vector3.up * Time.time * 180;
    }

    void Girdi()
    {
        if (JoystickGirdi.jy.aktiflik)
        {
            if (basiliyor)
            {
                hiz = Mathf.Lerp(hiz, 1, Time.deltaTime * 5);
            }
            else
            {
                ParmakBasti();
            }
        }
        else
        {
            hiz = Mathf.Lerp(hiz, 0, Time.deltaTime * 10);

            if (basiliyor)
            {
                animator.SetBool("Kosma", false);
                basiliyor = false;
            }
        }
    }

    public void ParmakBasti()
    {
        animator.SetBool("Kosma", true);
        basiliyor = true;
    }

    void FixedUpdate()
    {
        Hareket();
    }

    void Hareket()
    {
        Vector3 joyDeger = JoystickGirdi.hareketDegeri;
        Vector3 yon = Vector3.forward;
        yon.x = joyDeger.x;
        yon.z = joyDeger.y;

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(yon), Time.deltaTime * donmeHiz);
        Vector3 hizVek = transform.forward * ileriHiz * hiz;// * Mathf.Clamp(JoystickGirdi.hareketDegeri.magnitude, .2f, 1);
        hizVek.y = rb.velocity.y;
        rb.velocity = hizVek;

        //animator.SetFloat("hiz", Mathf.Clamp(JoystickGirdi.hareketDegeri.magnitude * 2, .5f, 1));
    }
}
