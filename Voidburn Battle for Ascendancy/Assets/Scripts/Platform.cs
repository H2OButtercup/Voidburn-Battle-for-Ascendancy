using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class Platform : MonoBehaviour
{
    [Tooltip("Layer mask for player ojects")]
    public LayerMask playerLayer;

    public float collsionDisableTime = .5f;
    [SerializeField] float thickness;
    [SerializeField] Collider platformcollider;
    private HashSet<Collider> playersOn = new HashSet<Collider>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    //void Update()
    //{
    //    foreach(var playerCollider in playersOn)
    //    {
    //        if (playerCollider == null) continue;

    //        playerController player = playerCollider.GetComponent<playerController>();
    //        if(player != null && player.IsPressingDrop() && player.groundedCheck)
    //        {
    //            StartCoroutine(DisableCollision(playerCollider));
    //        }
    //    }
    //}

    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            playersOn.Add(collision.collider);
        }
    }


    private void OnCollisionExit(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            playersOn.Remove(collision.collider);
        }

    }

    IEnumerator DisableCollision(Collider playerCollider)
    {
        Physics.IgnoreCollision(platformcollider, playerCollider,true);

        float timer = collsionDisableTime;
        while(timer > 0 && playerCollider != null)
        {
            timer -= Time.deltaTime;

            if (playerCollider.transform.position.y < transform.position.y - thickness)
                break;


            yield return null;
        }

        if (playerCollider != null)
        {
            Physics.IgnoreCollision(platformcollider, playerCollider, false);
        }
    }

  
}
