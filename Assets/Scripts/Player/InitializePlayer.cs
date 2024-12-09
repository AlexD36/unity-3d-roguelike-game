using UnityEngine;

public class InitializePlayer : MonoBehaviour
{
    public Camera cam;

    void Start()
    {
        Player.Camera = cam;
    }
}
