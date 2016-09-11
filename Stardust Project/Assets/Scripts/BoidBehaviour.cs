using UnityEngine;
using System.Collections.Generic;

public class BoidBehaviour : MonoBehaviour
{
    private List<Boid> boids = new List<Boid>();
    private Dictionary<int, List<Boid>> flocks = new Dictionary<int, List<Boid>>();
    float yLim;
    float xLim;
    int nextFlockID = 0;

    [SerializeField]
    private float boidSpeed = 2f;
    [SerializeField]
    private float sightRange = 2f;
    [SerializeField]
    private float flockPerceptionRange = 4f;
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
            var b = boidsGO[i].GetComponent<Boid>();
            boids.Add(b);
            b.setBehaviour(this);
        }

        yLim = 5.45f;
        xLim = 9.15f;
    }

    void Update()
    {
        Vector3 vel;
        ResetFlocks();
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
        int flockID = boid.getFlockID();
        List<Boid> flock = getNearBoids(boid);
        if(flocks.ContainsKey(flockID)) flock = flocks[flockID]; 

        Vector2 av = Vector3.zero;

        foreach (Boid b in flock)
            if(b != boid && Vector2.Distance(b.transform.position, boid.transform.position) < flockPerceptionRange)
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
                UpdateFlockID(boid, b);
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

    void UpdateFlockID(Boid a, Boid b)
    {
        var ida = a.getFlockID();
        var idb = b.getFlockID();
        if(ida == 0)
        {
            if(idb == 0)
            {
                a.setFlockId(++nextFlockID);
                b.setFlockId(nextFlockID);
            }
            else
                a.setFlockId(idb);
        }
        else
        {
            if (ida == idb) return;
            if (idb == 0)
                b.setFlockId(ida);
            else
            {
                int smallerID = GetSmallerFlock(ida, idb);
                List<Boid> smaller = new List<Boid>(flocks[smallerID]);
                int biggerID = smallerID == ida ? idb : ida;
                /*foreach(Boid boid in smaller)
                {
                    boid.setFlockId(biggerID);
                }*/
                if (biggerID == ida) b.setFlockId(ida);
                else a.setFlockId(idb);
            }
        }
    }
    int GetSmallerFlock(int ida, int idb)
    {
        int faCount = flocks[ida].Count;
        int fbCount = flocks[idb].Count;
        int min = Mathf.Min(faCount, fbCount);
        int smaller = min == faCount ? ida : idb;
        return smaller;
    }
    void ResetFlocks()
    {
        nextFlockID = 0;
        foreach(Boid b in boids)
        {
            b.setFlockId(0);
        }
    }

    public void AddToFlock(Boid b, int id)
    {
        if (id == 0) return;
        if (flocks.ContainsKey(id))
            flocks[id].Add(b);
        else
        {
            List<Boid> f = new List<Boid>();
            f.Add(b);
            flocks.Add(id, f);
        }

    }
    public void RemoveFromFlock(Boid b, int lastId)
    {
        if (!flocks.ContainsKey(lastId)) return;
        flocks[lastId].Remove(b);
        if (flocks[lastId].Count <= 0)
            flocks.Remove(lastId);
    }
}
