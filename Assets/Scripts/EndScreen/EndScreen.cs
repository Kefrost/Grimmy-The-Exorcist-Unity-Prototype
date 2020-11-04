using UnityEngine;

public class EndScreen : MonoBehaviour
{
    [SerializeField] private Animator transition;

    private void Start()
    {
        transition.SetTrigger("FadeIn");
    }
}
