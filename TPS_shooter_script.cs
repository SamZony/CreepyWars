using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class TPS_shooter_script : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;

    public CinemachineFreeLook FreeLookCamera;
    public CinemachineVirtualCamera VirtualCamera;
    public GameObject reticle;
    private Image reticle_up;
    private Image reticle_down;
    private Image reticle_left;
    private Image reticle_right;

    private bool aiming;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        reticle.SetActive(aiming);
    }

    // Update is called once per frame
    void Update()
    {
        aiming = Input.GetMouseButton(1);

        reticle.SetActive(aiming);

        if (aiming)
        {
            VirtualCamera.Priority = 15;
            FreeLookCamera.Priority = 5;
        }
        else
        {
            VirtualCamera.Priority = 5;
            FreeLookCamera.Priority = 15;
        }
    }
}
