using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent( typeof( Rigidbody2D ) )]
public class Boid : MonoBehaviour {

    Rigidbody2D rb;
    Vector3 nextVelocity = Vector3.up; //DoubleBuffer
    private BoidBehaviour behaviour;
    public List<Boid> nearBoids { get; private set; }
    private int flockID;
    [SerializeField]
    private Text idText;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        idText.text = "0";
    }

    public int getFlockID() { return flockID; }
    public void setFlockId(int value)
    {
        if (value == flockID) return;
        behaviour.RemoveFromFlock(this, flockID);
        flockID = value;
        behaviour.AddToFlock(this, value);
        idText.text = flockID.ToString();
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
    public void setBehaviour(BoidBehaviour b) { behaviour = b; }
}
