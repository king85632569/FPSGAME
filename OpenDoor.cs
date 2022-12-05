using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    //public bool Open;
    public Animator Anim;
    public PhotonView PV;
    public int CloseTime;
    public bool Need_Trigger;
    public bool DoorOpen;
    public bool Decode;
    // Start is called before the first frame update
    void Start()
    {
        Anim = GetComponent<Animator>();
        PV = GetComponent<PhotonView>();
        //Decode = true;
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.tag == "Move_Collider")&&(Need_Trigger==false)&&(Decode==true))
        {
            PV.RPC("Open_Door", RpcTarget.AllBuffered, true);
        }
        if ((other.gameObject.tag == "Enemy") && (Decode == true))
        {
            PV.RPC("Open_Door", RpcTarget.AllBuffered, true);
        }
        if ((other.gameObject.tag == "Move_Collider") && (other.GetComponent<PlayerController>().AI_Mode) && (Decode == true))
        {
             PV.RPC("Open_Door", RpcTarget.AllBuffered, true);
        }
        if ((other.gameObject.tag == "Animal_Door_Trigger") && (Decode == true))
        {
            PV.RPC("Open_Door", RpcTarget.AllBuffered, true);
        }
    }
    void OnTriggerStay(Collider other)
    {
        if ((other.gameObject.tag == "Move_Collider") && (Need_Trigger == true) && (Decode == true))
        {
            if ((other.GetComponent<PlayerController>().OpenDoor_State)&&(DoorOpen == false))
            {
                PV.RPC("Open_Door", RpcTarget.AllBuffered, true);
                other.GetComponent<PlayerController>().OpenDoor_State = false;
            }
            else if ((other.GetComponent<PlayerController>().OpenDoor_State) && (DoorOpen == true))
            {
                PV.RPC("Open_Door", RpcTarget.AllBuffered, false);
                other.GetComponent<PlayerController>().OpenDoor_State = false;
            }

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Move_Collider")
        {
        }
        if ((other.gameObject.tag == "Enemy") && (Decode == true))
        {
            PV.RPC("Open_Door", RpcTarget.AllBuffered, false);
        }
        if ((other.gameObject.tag == "Move_Collider") && (other.GetComponent<PlayerController>().AI_Mode) && (Decode == true))
        {
            PV.RPC("Open_Door", RpcTarget.AllBuffered, false);
        }
        if ((other.gameObject.tag == "Animal_Door_Trigger") && (Decode == true) && (Need_Trigger == true))
        {
            PV.RPC("Open_Door", RpcTarget.AllBuffered, false);
        }
    }
    [PunRPC]
    void Open_Door(bool Open)
    {
        if (Open)
        {
            if (Need_Trigger == false)
            {
                Anim.SetBool("Open", true);
                Invoke("Close_Door", CloseTime);
            }

            if ((Need_Trigger == true) && (DoorOpen==false))
            {
                Anim.SetBool("Open", true);
                DoorOpen = true;
            }
        }
        else
        {
            Anim.SetBool("Open", false);
            DoorOpen = false;
        }
    }
    void Close_Door()
    {
        PV.RPC("Open_Door", RpcTarget.AllBuffered, false);
    }
}
