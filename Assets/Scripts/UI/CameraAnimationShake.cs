using UnityEngine;

public class CameraAnimationShake : MonoBehaviour
{
    private Animator camAnim;

    void Start()
    {
        camAnim = GetComponent<Animator>();
    }

    public void PlayShake()
    {
        camAnim.Play("Shake");
    }
}
