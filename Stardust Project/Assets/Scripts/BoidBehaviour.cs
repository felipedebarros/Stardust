using UnityEngine;
using System.Collections.Generic;

public class BoidBehaviour : MonoBehaviour
{
    private List<Boid> boids = new List<Boid>();
    float yLim;
    float xLim;

    [SerializeField]
    private float boidSpeed = 2f;
    [SerializeField]
    private float sightRange = 2f;
    [SerializeField]
    private float cohesionFactor = .1f;
    [SerializeField]
    private float separationFactor = .1f;
    [SerializeField]
    private float alignmentFactor = .1f;

    void Start()
    {
        GameObject[] boidsGO = GameObject.FindGameObjectsWithTag("Boid");
        for (int i = 0; i < boidsGO.Length; i++)
        {
            boids.Add(boidsGO[i].GetComponent<Boid>());
        }

        yLim = 5.45f;
        xLim = 9.15f;
    }

    void Update()
    {
        Vector3 vel;
        foreach(Boid b in boids)
        {
            b.UpdateNearList(getNearBoids(b));

            vel = b.getForwardVector();

            vel += Cohesion(b) * cohesionFactor;
            vel += Separation(b) * separationFactor;
            vel += Alignment(b) * alignmentFactor;

            b.setVelocity(vel * boidSpeed);
            b.UpdateVelocity();

            LoopScreen(b);

            b.LookFoward();
        }
    }

    Vector3 Cohesion(Boid boid)
    {
        List<Boid> near = boid.nearBoids;
        Vector3 pcm = Vector3.zero;

        foreach (Boid b in near)
            pcm += b.transform.position;

        if (near.Count > 0) pcm /= near.Count;
        return pcm - boid.transform.position;
    }
    Vector3 Separation(Boid boid)
    {
        List<Boid> near = boid.nearBoids;
        Vector3 lg = Vector3.zero;

        foreach (Boid b in near)
            lg += b.transform.position;

        return boid.transform.position - lg;
    }
    Vector3 Alignment(Boid boid)
    {
        List<Boid> flock = getNearBoids(boid); 

        Vector2 av = Vector3.zero;

        foreach (Boid b in flock)
            av += (Vector2) Vector3.Normalize(b.GetComponent<Rigidbody2D>().velocity);

        return av - boid.GetComponent<Rigidbody2D>().velocity;
    }

    List<Boid> getNearBoids(Boid boid)
    {
        List<Boid> near = new List<Boid>();
        foreach(Boid b in boids)
        {
            if (b != boid && Vector2.Distance(b.transform.position, boid.transform.position) < sightRange)
            {
                near.Add(b);
            }
        }
        return near;
    }

    void LoopScreen(Boid boid)
    {
        if (boid.transform.position.y > yLim || boid.transform.position.y < -yLim)
        {
            Vector3 pos = boid.transform.position;
            pos.y -= .1f * Mathf.Sign(pos.y);
            pos.y *= -1;
            boid.transform.position = pos;
        }

        if (boid.transform.position.x > xLim || boid.transform.position.x < -xLim)
        {
            Vector3 pos = boid.transform.position;
            pos.x -= .1f * Mathf.Sign(pos.x);
            pos.x *= -1;
            boid.transform.position = pos;
        }
    }
}
