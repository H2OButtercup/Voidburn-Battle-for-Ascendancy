using UnityEngine;

public class CPUController : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Animator animator;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float reactionTime = 0.5f;

    private Vector3 lastPlayerPosition;
    private float timeSinceLastDecision;
    private int sidestepCount;
    private int attackCount;
    private int jumpCount;

    void Update()
    {
        timeSinceLastDecision += Time.deltaTime;

        if (timeSinceLastDecision >= reactionTime)
        {
            AnalyzePlayerBehavior();
            MakeDecision();
            timeSinceLastDecision = 0f;
        }

        FacePlayer();
    }

    void FacePlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    void AnalyzePlayerBehavior()
    {
        Vector3 movementDelta = player.position - lastPlayerPosition;

        if (Mathf.Abs(movementDelta.x) > 1f)
            sidestepCount++;

        if (movementDelta.z > 1f)
            jumpCount++;

        lastPlayerPosition = player.position;
    }

    void MakeDecision()
    {
        if (sidestepCount > 3)
        {
            animator.SetTrigger("Sweep"); // Punish sidestep
            sidestepCount = 0;
        }
        else if (jumpCount > 2)
        {
            animator.SetTrigger("AntiAir");
            jumpCount = 0;
        }
        else
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance > 3f)
                MoveTowardsPlayer();
            else
                animator.SetTrigger("Attack");
        }
    }

    void MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        transform.position += direction * moveSpeed * Time.deltaTime;
        animator.SetFloat("MoveSpeed", 1f);
    }
}