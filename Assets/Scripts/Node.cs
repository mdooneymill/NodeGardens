using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{

    // used for checking connections
    public int id;

    // Keep a list of nodes that are nearby
    public List<int> nearby;

    // bounds to stay within
    public Vector3 bounds;

    // reference to the rigid body componeny
    public Rigidbody rb;


    void Start()
    {

        nearby = new List<int>();
        rb = GetComponent<Rigidbody>();

    }


    // apply a random force to the rigidbody
    public void RandomNudge( float maxForce = 0.15f )
    {

        

        Vector3 force = new Vector3(
            Random.Range( -maxForce, maxForce ),
            Random.Range( -maxForce, maxForce ),
            Random.Range( -maxForce, maxForce )
        );

        rb.AddForce( force, ForceMode.Impulse );

    }


    void FixedUpdate()
    {

        // keep in bounds

        Vector3 pos = transform.localPosition;

        if( pos.x > bounds.x ) pos.x -= bounds.x * 2f;
        else if( pos.x < -bounds.x ) pos.x += bounds.x * 2f;

        if( pos.y > bounds.y ) pos.y -= bounds.y * 2f;
        else if( pos.y < -bounds.y ) pos.y += bounds.y * 2f;

        if( pos.z > bounds.z ) pos.z -= bounds.z * 2f;
        else if( pos.z < -bounds.z ) pos.z += bounds.z * 2f;

        rb.MovePosition( pos );

    }


    // Using the collider as a trigger to track nearby nodes

    // When an object enters the trigger chuck its ID into an array if it's not already there
    private void OnTriggerEnter( Collider collider )
    {

        Node n = collider.gameObject.GetComponent<Node>();

        if( n == null )
            return;

        if( !nearby.Contains( n.id ) )
        {

            nearby.Add( n.id );

        }

    }

    // when an object leaves the trigger, remove the ID from the array
    private void OnTriggerExit( Collider collider )
    {

        Node n = collider.gameObject.GetComponent<Node>();

        if( n == null )
            return;

        if( nearby.Contains( n.id ) )
        {

            nearby.Remove( n.id );

        }

    }


}
