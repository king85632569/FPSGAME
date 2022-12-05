using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    public PhotonView PV;
    public float Speed;
    public GameObject Bridge_Obj;
    public bool Trigger;
    public float Bridge_y, RotateTarget;
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        RotateTarget = Bridge_Obj.transform.rotation.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        Bridge_y = Bridge_Obj.transform.rotation.eulerAngles.y;
        if ((PhotonNetwork.IsMasterClient)&&(Trigger)&&(Bridge_y <= RotateTarget))
        {
            Bridge_Obj.transform.Rotate(Vector3.forward * Speed * Time.deltaTime);
        }
        else if ((PhotonNetwork.IsMasterClient) && (Trigger) && (Bridge_y >= RotateTarget))
        {
            Close_Bridge();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if ((other.gameObject.tag == "Move_Collider") && (Trigger == false))
        {
            if (other.GetComponent<PlayerController>().OpenDoor_State)
            {
                PV.RPC("Open_Bridge", RpcTarget.AllBuffered, true);
                other.GetComponent<PlayerController>().OpenDoor_State = false;
            }
        }
    }

    [PunRPC]
    void Open_Bridge(bool Open)
    {
        if (Open)
        {
            Trigger = true;
            RotateTarget += 90;
        }
        else
        {
            Trigger = false;
        }
    }

    void Close_Bridge()
    {
        PV.RPC("Open_Bridge", RpcTarget.AllBuffered, false);
    }
}
