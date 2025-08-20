using System.ComponentModel;
using UnityEngine;

public class playerController1 : MonoBehaviour
{

    [Header("Movement Settings")]
    public float moveSpeed;
    public float dashSpeed;
    public float dashDuration;
    public float jumpForce;
    public float sideStepDistance;
    public float sideWalkSpeed;
    public float rotationSpeed;
    public float tapThreshold;
    public float crouchHeight;
    public float crouchTransitionDuration;

    [Header("References")]
    public Transform opponent;
    public Animator animator;
    bool iscrouched;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandleFacing()
    {
        if (opponent == null) return;
        Vector3 direction = opponent.position - transform.position;
        direction.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}
