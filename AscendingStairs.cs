using UnityEngine;

public class AscendingStairs : MonoBehaviour
{
    private Animator animator;
    private bool isAscendingStairs;
    private bool isDescendingStairs;
    private Quaternion targetRotation;
    public float lerpSpeed;
    private GameObject upwardsTarget;
    private GameObject downwardsTarget;

    void Start()
    {
        animator = transform.GetComponent<TPS_script>().animator;

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("stairs"))

        {
            upwardsTarget = other.transform.GetChild(0).gameObject;
            downwardsTarget = other.transform.GetChild(1).gameObject;
            Vector3 stairNormal = other.transform.up;

            float dotProduct = Vector3.Dot(stairNormal, Vector3.up);

            if (dotProduct > 0)
            {
                isAscendingStairs = true;

                Vector3 targetDirection = upwardsTarget.transform.position - transform.position;
                targetDirection.y = 0f; // Ignore the y component for rotation around the y-axis
                targetRotation = Quaternion.LookRotation(targetDirection);
            }
            else
            {
                isDescendingStairs = true;

                Vector3 targetDirection = downwardsTarget.transform.position - transform.position;
                targetDirection.y = 0f; // Ignore the y component for rotation around the y-axis
                targetRotation = Quaternion.LookRotation(targetDirection);
            }

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("stairs"))

        {
            Vector3 stairNormal = other.transform.up;

            float dotProduct = Vector3.Dot(stairNormal, Vector3.up);

            if (dotProduct > 0)
            {
                isAscendingStairs = false;
            }
            else isDescendingStairs = false;
        }
    }

    void Update()
    {
        if (isAscendingStairs)
        {
            Quaternion targetRotationY = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotationY, lerpSpeed * Time.deltaTime);
            animator.Play("on_stairs");
            animator.SetFloat("stairs_movement", 1);
        }
        else if (isDescendingStairs)
        {
            Quaternion targetRotationY = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotationY, lerpSpeed * Time.deltaTime);
            animator.Play("on_stairs");
            animator.SetFloat("stairs_movement", 0);
        }
        else animator.SetFloat("stairs_movement", 0.5f);
    }
}
