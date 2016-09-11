using UnityEngine;
using System.Collections.Generic;

[RequireComponent( typeof( Rigidbody2D ) )]
public class Boid : MonoBehaviour {

    Rigidbody2D rb;
    Vector3 nextVelocity = Vector3.up; //DoubleBuffer
    public List<Boid> nearBoids { get; private set; }
    public int flockID = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Color w = Color.white * flockID / 20;
        w.a = 1f;
        GetComponent<SpriteRenderer>().color = w;
    }

    public Vector3 getVelocity() { return rb.velocity; }
    public void setVelocity(Vector3 vel) { nextVelocity = vel; }
    public void UpdateVelocity() { rb.velocity = nextVelocity; }
    public Vector3 getForwardVector() { return transform.up; }
    public void UpdateNearList(List<Boid> near) { nearBoids = near; }
    public void LookFoward()
    {
        float angle = Vector3.Angle(Vector3.up, rb.velocity);
        transform.localEulerAngles = angle * -Vector3.forward;
    }
}
