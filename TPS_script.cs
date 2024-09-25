using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TPS_script : MonoBehaviour
{

    public Animator animator;
    public Transform cameraTransform;
    private Rigidbody rb;

    [Header("Inputs")]
    public float VerticalInput;
    public float HorizontalInput;
    public float blendTreeXParameter;
    public float blendTreeYParameter;

    [Header("Configurations")]
    public float TransitionSpeed;
    public float cameraAngleMinThreshold = 170f;
    public float cameraAngleMaxThreshold = 200f;
    public float currentAngleThreshold;
    private float angle;

    [Header("Leaning & Turning Mechanics")]
    public float leanAngleMax = 30f;
    public float leanDuration = 1.5f;
    public float turnDuration = 1.5f;
    public float turnSpeed = 5f;
    public float cameraAngleThresholdForLean = 90f;
    private float currentLeanAngle = 0f;
    private Quaternion targetRotation;
    private bool isLeaning = false;
    private bool isTurning = false;

    [Header("Testing")]
    public float moveParameter;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        HorizontalInput = Input.GetAxis("Horizontal");
        VerticalInput = Input.GetAxis("Vertical");

        blendTreeXParameter = (HorizontalInput + 1) / 2;
        blendTreeYParameter = (VerticalInput + 1) / 2;


        // ---------------------------------------------------------------------------------------------

        animator.SetFloat("idle_turn_blend", blendTreeXParameter); // turn left or right when idle


        if (VerticalInput != 0)
        {
            MoveForward();
        }
        if (VerticalInput == 0)
        {
            StartCoroutine(StopMoving());
        }

        if (HorizontalInput != 0f && VerticalInput == 0f)
        {
            // Set 'move' float in the animator
            animator.SetFloat("move", 1f);
            Quaternion cameraRotation = Quaternion.Euler(0f, cameraTransform.rotation.y, cameraTransform.rotation.z);
            // Calculate target rotation
            Quaternion targetRotation = cameraRotation * Quaternion.LookRotation(new Vector3(HorizontalInput, 0f, 0f));

            // Turn towards target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);


            animator.SetFloat("move_direction_blend", 0.5f);
        }

    }



    void MoveForward()
    {
        float targetLeanAngle = Mathf.Clamp(-Input.GetAxis("Mouse X") * leanAngleMax, -leanAngleMax, leanAngleMax);

        animator.SetFloat("move_bf_blend", VerticalInput);

        if (VerticalInput > 0)
        {
            animator.SetFloat("move", 1); // To make the character move
            animator.SetFloat("move_direction_blend", blendTreeXParameter); // To change the direction according to parameter

            if (Input.GetAxis("Horizontal") == 0)
            {
                if (angle < cameraAngleThresholdForLean)
                {
                    // Calculate lean angle based on mouse input


                    // Start leaning and turning if not already doing so
                    if (!isLeaning && !isTurning)
                    {
                        isLeaning = true;
                        isTurning = true;
                        StartCoroutine(LeanAndTurnTowards(targetLeanAngle));
                    }
                }
                else
                {
                    // Stop leaning and turning if camera is outside the threshold
                    isLeaning = false;
                    isTurning = false;
                    currentLeanAngle = 0f;
                }
            }
        }
        else if (VerticalInput < 0)
        {
            animator.SetFloat("move", 0);
        }

        if (VerticalInput > 0 && Input.GetKey(KeyCode.LeftShift)) // To sprint around
        {
            animator.SetFloat("move_type_blend", 1);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift)) // if true, run the coroutine once to stop the character
        {
            animator.SetFloat("move_type_blend", 0);
        }

        // Character will turn 180 degrees if the angle between him and the camera is more than the set threshold
        angle = Vector3.Angle(transform.forward, cameraTransform.forward);
        currentAngleThreshold = angle;

        if (angle >= cameraAngleMinThreshold)
        {
            animator.Play("turn180");
        }



    }

    IEnumerator StopMoving()
    {
        yield return new WaitForSeconds(0.1f);

        if (VerticalInput == 0)
        {
            animator.SetFloat("move", -1);
        }
        else
        {
            yield return null;
        }
    }

    private IEnumerator LeanAndTurnTowards(float targetLeanAngle)
    {
        float elapsedTime = 0f;
        float startingLeanAngle = currentLeanAngle;
        targetRotation = cameraTransform.rotation;

        while (elapsedTime < leanDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / leanDuration;


            // Turn
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, t);

            // Lean
            currentLeanAngle = Mathf.Lerp(startingLeanAngle, targetLeanAngle, t);
            transform.localEulerAngles = new Vector3(0f, transform.localEulerAngles.y, currentLeanAngle);


            yield return null;
        }

        // Ensure the final lean angle and rotation match the targets
        currentLeanAngle = targetLeanAngle;
        transform.rotation = targetRotation;
        transform.localEulerAngles = new Vector3(0f, transform.localEulerAngles.y, currentLeanAngle);

        isLeaning = false;
        isTurning = false;
    }
}
