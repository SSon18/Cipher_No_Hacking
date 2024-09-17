using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Transform previousRoom;
    [SerializeField] private Transform nextRoom;
    [SerializeField] private CameraControl cam;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (cam != null)
            {
                if (collision.transform.position.x < transform.position.x)
                {
                    cam.MoveToNewRoom(nextRoom);
                }
                else
                {
                    cam.MoveToNewRoom(previousRoom);
                }
            }
            else
            {
                Debug.LogError("Camera reference is missing in the Door script.");
            }
        }
    }

    private void Awake()
    {
        cam = Camera.main.GetComponent<CameraControl>();
    }

}
