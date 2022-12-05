using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassBroken : MonoBehaviour
{
    public PhotonView PV;
    public bool Broken;
    public GameObject Player;
    // Start is called before the first frame update
    void Awake()
    {
        Broken = false;
        PV = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Broken)
        {
            PV.RPC("Grass_Broken", RpcTarget.All, false);
            Broken = false;
        }
    }
    [PunRPC]
    void Grass_Broken(bool Kinematic)
    {
        this.gameObject.GetComponent<Rigidbody>().isKinematic = Kinematic;
        //this.gameObject.GetComponent<Rigidbody>().AddForce(transform.up * -10);
        this.gameObject.GetComponent<Rigidbody>().AddForce(transform.right * 1);
        this.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * 1);
        Destroy(this.gameObject, 5);
    }
}
