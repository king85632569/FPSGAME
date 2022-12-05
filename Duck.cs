using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duck : MonoBehaviour
{
    Rigidbody rb;
    public PhotonView PV;
    public float time;
    public int x_Power, z_Power, PowerRange, RotateSpeed;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient)
        {
            Duck_Float();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            time += Time.deltaTime;
            transform.Rotate(Vector3.down * RotateSpeed * Time.deltaTime);
            if (time >= 5f)
            {
                time = 0;
                Duck_Float();
            }
        }
    }

    public void Duck_Float()
    {
        x_Power = Random.Range(-PowerRange, PowerRange);
        z_Power = Random.Range(-PowerRange, PowerRange);
        rb.AddForce(transform.forward * x_Power);
        rb.AddForce(transform.right * z_Power);
    }
}
