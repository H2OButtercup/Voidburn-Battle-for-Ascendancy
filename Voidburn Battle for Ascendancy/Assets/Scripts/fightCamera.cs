using System.Collections.Specialized;
using UnityEngine;
using Unity.Cinemachine;

public class fightCamera : MonoBehaviour
{

    // The two characters to follow
    private Transform player1;
    private Transform player2;

    [Header("Camera Settings")]
    public CinemachineCamera fighterCam;
    public float minZoom = 6f;
    public float maxZoom = 12f;

    private void Awake()
    {
        // Find the GameObjects with the specified tags and get their transforms
        GameObject p1Object = GameObject.FindWithTag("Player1");
        if(p1Object != null)
        {
            player1 = p1Object.transform;
        }

        GameObject p2Object = GameObject.FindWithTag("Player2");
        if (p1Object != null)
        {
            player2 = p2Object.transform;
        }
    }
    // Update is called once per frame
    void LateUpdate()
    {
        // check if both players are assigned
        if (player1 != null &&  player2 != null)
        {
            // Calculate the midpoint between the two players
            Vector3 midpoint = (player1.position + player2.position) / 2f;
            
            // Set the sphere's position to the calculated midpoint
            transform.position = midpoint;
        }

        if (fighterCam != null)
        {
            float distance = Vector3.Distance(player1.position, player2.position);
            float zoom = Mathf.Clamp(distance, minZoom, maxZoom);

            var orbital = fighterCam.GetComponent<CinemachineOrbitalFollow>();
            if (orbital != null)
            {
                orbital.Radius = zoom;
            }
        }
    }
}
