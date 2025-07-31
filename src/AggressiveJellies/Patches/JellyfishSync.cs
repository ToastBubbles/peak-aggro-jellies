using Photon.Pun;
using UnityEngine;

public class JellyfishSync : MonoBehaviourPun
{
    [PunRPC]
    public void MoveJellyfish(Vector3 newPosition, Quaternion newRotation)
    {
        transform.position = newPosition;
        transform.rotation = newRotation;
    }
}