using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGardens_001 : MonoBehaviour
{

    // Prefabs to use
    public GameObject nodePrefab;
    public GameObject connectorPrefab;

    // How many nodes should it make
    public int nodeCount = 100;

    // How far should it spread the points
    public Vector3 bounds = Vector3.zero;

    // Max Nudge strength
    public float maxNudgeStrength;

    // Array to store the nodes created
    Node[] nodes;

    // Storing active connections in a dictionary
    Dictionary<string, GameObject> connections;
    // keeping dead ones in a list
    List<GameObject> deadConnections;

    
    void Start()
    {

        connections = new Dictionary<string, GameObject>();
        deadConnections = new List<GameObject>();

        CreateNodes();
        
    }


    void Update()
    {

        CheckConnections();

    }

    void FixedUpdate()
    {

        if( Input.GetKeyDown( KeyCode.Space ) )
        {
            NudgeNodes();
        }

    }

    void NudgeNodes()
    {

        foreach( Node n in nodes )
        {

            n.RandomNudge( maxNudgeStrength );

        }

    }


    void CreateNodes()
    {

        // create the nodes array
        nodes = new Node[ nodeCount ];

        for( int i = 0; i < nodeCount; i++ )
        {

            GameObject go = Instantiate( nodePrefab, transform );

            // create a random position within the bounds
            Vector3 pos = new Vector3(
                Random.Range( -bounds.x, bounds.x ),
                Random.Range( -bounds.y, bounds.y ),
                Random.Range( -bounds.z, bounds.z )
            );

            // move the prefab to that position
            go.transform.localPosition = pos;

            // add the prefab to the array
            nodes[ i ] = go.GetComponent<Node>();

            // set id and bounds to stay within
            nodes[ i ].id = i;
            nodes[ i ].bounds = bounds;

        }

    }

    
    void CheckConnections()
    {

        // store new connections to avoid checking those for removal
        List<string> newConnections = new List<string>();

        for( int i = 0; i < nodes.Length; i++ )
        {

            // grab the node
            Node n = nodes[ i ];

            // check it's list of nearby nodes to see if they already have a connection
            foreach( int id in n.nearby )
            {

                string connectionId = string.Format( "{0}_{1}", Mathf.Min( n.id, id ), Mathf.Max( n.id, id ) );

                // if there isn't a connection with this id, make one
                if( !connections.ContainsKey( connectionId ) )
                {

                    MakeConnection( n.id, id, connectionId );
                    newConnections.Add( connectionId );

                }

            }

        }

        // store keys to connections that should be removed
        List<string> toRemove = new List<string>();

        // now check each connection ( apart from new ones )
        foreach( KeyValuePair<string, GameObject> conn in connections )
        {

            // if not in new connections
            if( !newConnections.Contains( conn.Key ) )
            {

                // grab the two ids
                int[] ids = GetConnectionIds( conn.Key );

                // and the nodes
                Node a = nodes[ ids[ 0 ] ];
                Node b = nodes[ ids[ 1 ] ];

                // if they're still nearby, update the rotation 
                if( a.nearby.Contains( ids[ 1 ] ) )
                {

                    UpdateConnection( a, b, conn.Value );

                }
                else // otherwise add the key to toRemove array
                {

                    toRemove.Add( conn.Key );

                }

            }

        }

        // lastly remove any dead connections found
        foreach( string key in toRemove )
        {

            // grab the connection, remove from active dictionary and add to dead list
            GameObject conn = connections[ key ];
            connections.Remove( key );
            deadConnections.Add( conn );
            conn.SetActive( false );

        }

    }


    // split the string by the under score and parse the two ids
    int[] GetConnectionIds( string key )
    {

        string[] split = key.Split( '_' );

        int a = int.Parse( split[ 0 ] );
        int b = int.Parse( split[ 1 ] );

        int[] ret = new int[ 2 ];
        ret[ 0 ] = Mathf.Min( a, b );
        ret[ 1 ] = Mathf.Max( a, b );

        return ret;

    }

    // makes a new connection between two nodes
    void MakeConnection( int a, int b, string id )
    {

        // grab the node positions
        Vector3 posA = nodes[ a ].transform.localPosition;
        Vector3 posB = nodes[ b ].transform.localPosition;

        // Create the connection geo
        GameObject go = GetConnection();

        // move it to position of node a
        go.transform.localPosition = posA;

        // point it at node b
        Vector3 dif = Vector3.Normalize( posB - posA );
        go.transform.localRotation = Quaternion.LookRotation( Vector3.Normalize( dif ), Vector3.forward );

        // scale the z axis to the distance between the two points
        Vector3 scale = go.transform.localScale;
        scale.z = dif.magnitude;
        go.transform.localScale = scale;

        connections.Add( id, go );

    }

    void UpdateConnection( Node a, Node b, GameObject connection )
    {

        // grab the node positions
        Vector3 posA = a.transform.localPosition;
        Vector3 posB = b.transform.localPosition;

        connection.transform.localPosition = posA;

        // point it at node b
        Vector3 dif = posB - posA;
        connection.transform.localRotation = Quaternion.LookRotation( Vector3.Normalize( dif ), Vector3.forward );

        // scale the z axis to the distance between the two points
        Vector3 scale = connection.transform.localScale;
        scale.z = dif.magnitude;
        connection.transform.localScale = scale;

    }

    
    GameObject GetConnection()
    {

        GameObject go;

        // if there's a dead one remove it from dead array and return
        if( deadConnections.Count > 0 )
        {

            go = deadConnections[ 0 ];
            go.SetActive( true );
            deadConnections.RemoveAt( 0 );

        }
        else // otherwise create a new one
        {

            go = Instantiate( connectorPrefab, transform );

        }

        return go;

    }

}
