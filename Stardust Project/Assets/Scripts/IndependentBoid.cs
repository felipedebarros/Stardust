using UnityEngine;
using System.Collections.Generic;

[RequireComponent( typeof( Rigidbody2D ) )]
public class IndependentBoid : MonoBehaviour
{
    private Rigidbody2D rb;
    private List<IndependentBoid> nearBoids = new List<IndependentBoid>();
    private Vector3 nextVel;
    float yLim;
    float xLim;

    [SerializeField]
    private float boidSpeed = 2f;
    [SerializeField]
    private float cohesionFactor = .1f;
    [SerializeField]
    private float separationFactor = 1f;
    [SerializeField]
    private float alignmentFactor = .1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        yLim = 5.45f;
        xLim = 9.15f;
    }

    void Update()
    {
        nextVel = transform.up;

        nextVel += Vector3.Normalize(Cohesion()) * cohesionFactor;
        nextVel += Vector3.Normalize(Separation()) * separationFactor;
        nextVel += Vector3.Normalize(Alignment()) * alignmentFactor;

        rb.velocity = Vector3.Normalize(nextVel) * boidSpeed;

        LookFoward();
        //LoopScreen();
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (!coll.isTrigger && coll.tag == "Boid")
        {
            var boid = coll.GetComponent<IndependentBoid>();
            if(!nearBoids.Contains(boid))
                nearBoids.Add(boid);
        }
    }
    void OnTriggerExit2D(Collider2D coll)
    {
        if (!coll.isTrigger && coll.tag == "Boid")
        {
            var boid = coll.GetComponent<IndependentBoid>();
            if (!nearBoids.Contains(boid))
                nearBoids.Remove(boid);
        }
    }

    Vector3 Cohesion()
    {
        Vector3 pcm = Vector3.zero;

        foreach (IndependentBoid b in nearBoids)
            pcm += b.transform.position;

        if (nearBoids.Count > 0) pcm /= nearBoids.Count;
        return pcm - transform.position;
    }
    Vector3 Separation()
    {
        Vector3 lg = Vector3.zero;

        foreach (IndependentBoid b in nearBoids)
            lg -= (b.transform.position - transform.position);

        return lg;
    }
    Vector3 Alignment()
    {
        Vector2 av = Vector3.zero;

        foreach (IndependentBoid b in nearBoids)
            av += b.GetComponent<Rigidbody2D>().velocity;

        if (nearBoids.Count > 0) av /= nearBoids.Count;
        return av - rb.velocity;
    }

    void LookFoward()
    {
        float angle = Vector3.Angle(Vector3.up, rb.velocity);
        transform.localEulerAngles = angle * -Vector3.forward;
    }

    void LoopScreen()
    {
        if (transform.position.y > yLim || transform.position.y < -yLim)
        {
            Vector3 pos = transform.position;
            pos.y -= .1f * Mathf.Sign(pos.y);
            pos.y *= -1;
            transform.position = pos;
        }

        if (transform.position.x > xLim || transform.position.x < -xLim)
        {
            Vector3 pos = transform.position;
            pos.x -= .1f * Mathf.Sign(pos.x);
            pos.x *= -1;
            transform.position = pos;
        }
    }
}
