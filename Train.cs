using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Train : MonoBehaviourPunCallbacks
{
    //public bool Open;
    public Animator Anim, AnimDoor;
    public PhotonView PV;
    public GameObject Train_transform, Carriage_front, Carriage_rear, Train_Obj, Carriage_front_Obj, Carriage_rear_Obj;
    public GameObject Carriage_front_Trigger, Carriage_rear_Trigger;
    public bool Enabled;
    public float normalizedTime;
    AnimatorStateInfo animatorInfo;

    // Start is called before the first frame update
    void Start()
    {
        Anim = GetComponent<Animator>();
        AnimDoor= Train_Obj.GetComponent<Animator>();
        PV = GetComponent<PhotonView>();
        CloseDoor();
        //Anim.SetBool("Run", true);
        if (PhotonNetwork.IsMasterClient)
        {
            Anim.Play("train");
        }

    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PV.RPC("RPC_Get_normalizedTime", RpcTarget.All, normalizedTime);
        }

    }
    [PunRPC]
    void RPC_Get_normalizedTime(float normalizedTime)
    {
        Debug.Log("動畫進度:"+normalizedTime);

        if (!PhotonNetwork.IsMasterClient)
        {
            Anim.Play("train", 0, normalizedTime);
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {

       if (PhotonNetwork.IsMasterClient)
       {
            AnimatorStateInfo stateinfo = Anim.GetCurrentAnimatorStateInfo(0);
            normalizedTime=stateinfo.normalizedTime;
            Train_Obj.transform.position = Train_transform.transform.position;
            Train_Obj.transform.rotation = Train_transform.transform.rotation;
            Carriage_front_Obj.transform.position = Carriage_front.transform.position;
            Carriage_front_Obj.transform.rotation = Carriage_front.transform.rotation;
            Carriage_rear_Obj.transform.position = Carriage_rear.transform.position;
            Carriage_rear_Obj.transform.rotation = Carriage_rear.transform.rotation;
        }/*
        Train_Obj.transform.position = Vector3.Lerp(Train_Obj.transform.position, Train_transform.transform.position, Time.time * 2f);
        Train_Obj.transform.rotation = Quaternion.Lerp(Train_Obj.transform.rotation, Train_transform.transform.rotation, Time.time * 2f);
        Carriage_front_Obj.transform.position = Vector3.Lerp(Carriage_front_Obj.transform.position, Carriage_front.transform.position, Time.time * 2f);
        Carriage_front_Obj.transform.rotation = Quaternion.Lerp(Carriage_front_Obj.transform.rotation, Carriage_front.transform.rotation, Time.time * 2f);
        Carriage_rear_Obj.transform.position = Vector3.Lerp(Carriage_rear_Obj.transform.position, Carriage_rear.transform.position, Time.time * 2f);
        Carriage_rear_Obj.transform.rotation = Quaternion.Lerp(Carriage_rear_Obj.transform.rotation, Carriage_rear.transform.rotation, Time.time * 2f);*/
    }
    void OpenDoor()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //Anim_enabled();
            PV.RPC("RPC_OpenDoor", RpcTarget.All);
        }
    }
    [PunRPC]
    void RPC_OpenDoor()
    {
        Anim.speed = 0;
        Carriage_front_Trigger.SetActive(false);
        Carriage_rear_Trigger.SetActive(false);
        AnimDoor.SetBool("Open", true);
        Invoke("CloseDoor", 10);
        if (PhotonNetwork.IsMasterClient)
        {
            PV.RPC("RPC_Get_normalizedTime", RpcTarget.All, normalizedTime);
        }
    }
    void CloseDoor()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PV.RPC("RPC_CloseDoor", RpcTarget.All);
        }
    }
    [PunRPC]
    void RPC_CloseDoor()
    {
        Anim.speed = 1;
        Carriage_front_Trigger.SetActive(true);
        Carriage_rear_Trigger.SetActive(true);
        AnimDoor.SetBool("Open", false);
        if (PhotonNetwork.IsMasterClient)
        {
            PV.RPC("RPC_Get_normalizedTime", RpcTarget.All, normalizedTime);
        }
    }
}
