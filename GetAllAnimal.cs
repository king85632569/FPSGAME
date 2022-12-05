using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class GetAllAnimal : MonoBehaviourPunCallbacks
{
    public GameObject Target, Animal_Target;
    public float Animal_distance, HightDistance, distance;
    public PlayerController playerController;
    public Transform Player_transform, Animal_transform;
    Vector3 vDir = Vector3.zero;
    public Vector3 Ride_Vector;
    public PhotonView PV;
    public CharacterController Player_collider;
    public bool Ride_Start, Ride_Over;
    public CharacterController CharacterController;

    public TMP_Text Skill_CD_UI;
    public Image Skill_UI;
    public GameObject Skill_Button_UI;
    public float Skill_CD, Skill_CD_Time;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        //playerController = GetComponentInParent<PlayerController>();
        Player_transform = GetComponent<Transform>();
        Player_collider= GetComponent<CharacterController>();
        Ride_Start = false;
        CharacterController = GetComponent<CharacterController>();
        Skill_Button_UI.SetActive(false);

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (Ride_Over)
        {
            Vector3 moveDirectionHorizontal = transform.right * 75;
            CharacterController.Move(moveDirectionHorizontal * Time.deltaTime);
            Ride_Over = false;
        }
    }
    void Update()
    {
        if ((playerController.playerManager.death == false) && (playerController.AI_Mode == false))
        {
            OnGetAnimal();
            if (PV.IsMine)
            {
                UI_Controller();
            }
        }
        else if ((playerController.Ride_State == true) && (playerController.playerManager.death == true) && (playerController.AI_Mode == false))
        {
            AnimalState(false);
            Ride_Over = true;
            playerController.Ride_Try = false;
        }
    }
    void UI_Controller()
    {
        if (playerController.Ride_State)
        {
            Skill_Button_UI.SetActive(true);
            Skill_CD = Target.GetComponent<Animal>().Attack_CD;
            Skill_CD_Time = Target.GetComponent<Animal>().Attack_CD_Time;
            if (Skill_CD < Skill_CD_Time)
            {
                if ((Skill_CD_UI) && (Skill_UI))
                {
                    float cd = Skill_CD_Time - Skill_CD;
                    Skill_CD_UI.text = cd.ToString("#0.0");
                    Skill_UI.fillAmount = 1 - (Skill_CD / Skill_CD_Time);
                }
                Skill_CD += Time.deltaTime;
            }
            else
            {
                if ((Skill_CD_UI) && (Skill_UI))
                {
                    Skill_CD_UI.text = null;
                    Skill_UI.fillAmount = 0;
                }
            }
        }
        else
        {
            Skill_Button_UI.SetActive(false);
        }
    }

    [PunRPC]
    void RPC_AnimalState(bool Ride_State)
    {
        //Debug.Log("變更動物狀態");
        if (Target == null)
            return;
        if (Ride_State == true)
        {
            Target.GetComponent<Animal>().Ride = Ride_State;
            Target.GetComponent<Animal>().Player = this.gameObject;
            Target.GetComponent<Animal>().Ride_Change = false;

            transform.position = new Vector3(Target.transform.position.x + Ride_Vector.x, Target.transform.position.y + Ride_Vector.y, Target.transform.position.z + Ride_Vector.z);
            transform.rotation = Target.transform.rotation;
            Player_collider.height = Target.GetComponent<Animal>().Collider_Height;
            Player_collider.center = new Vector3(0, Target.GetComponent<Animal>().Collider_Value_Y, 0);
            Ride_Vector = Target.GetComponent<Animal>().Ride_Vector;

            playerController.TakeDamageNerf = Target.GetComponent<Animal>().TakeDamageNerf;
        }
        else if (Ride_State == false)
        {
            if (Target.GetComponent<Animal>().Ride == true)
            {
                Target.transform.position = this.transform.position;
                Target.transform.eulerAngles = this.transform.eulerAngles;
            }
            Target.GetComponent<Animal>().Ride = Ride_State;
            Target.GetComponent<Animal>().Player = null;
            Target.GetComponent<Animal>().Ride_Change = false;


            Player_collider.center = new Vector3(0, 0, 0);
            Player_collider.height = 2;

            playerController.TakeDamageNerf = 1;

            if (playerController.CharacterSetActive == true)
            {
                int itemIndex = playerController.itemIndex;
                SingleShotGun ShotGun = playerController.items[itemIndex].GetComponent<SingleShotGun>();
                ShotGun.Ride_Shoot = false;
            }
        }
    }

    //[PunRPC]
    void Ride(bool Ride_State)
    {
        //Debug.Log("變更動物位置");
        if (Target == null)
            return;
        if ((Ride_State == true) && (Target.GetComponent<Animal>().RidePlayer == 1))
        {
            Target.transform.position = this.transform.position;
            Target.transform.eulerAngles = this.transform.eulerAngles;
        }
    }
    //[PunRPC]
    void AnimalState(bool Ride_State)
    {
        if (Target == null)
            return;
        playerController.RideControl(Ride_State);

        PV.RPC("RPC_AnimalState", RpcTarget.AllBuffered, Ride_State);
        //Target.GetComponent<Animal>().Ride = Ride_State;
        //Target.GetComponent<Animal>().Ride_Change = false;
    }

    void OnGetAnimal()
    {
        GameObject[] Animal = GameObject.FindGameObjectsWithTag("Animal");
        float distance_min = 1000;
        int id = 0;
        for (int i = 0; i < Animal.Length; i++)
        {
            if (Animal[i].activeSelf == true)
            {
                HightDistance = Animal[i].transform.position.y - transform.position.y;
                Animal_distance = Vector3.Distance(transform.position, Animal[i].transform.position);
                if ((Animal_distance < distance_min) && (HightDistance < 5) && (HightDistance > -2))
                {
                    distance_min = Animal_distance;
                    id = i;
                    if ((Animal[id].tag == "Animal")&&(playerController.Ride_State == false))
                    {
                        Animal_Target = Animal[id];
                        Target = Animal_Target;
                    }
                }
            }
        }
        if (Animal.Length == 0)
        {
            Target = null;
        }
        else if (Animal.Length == 1)
        {
            id = 0;
        }
        if (Target != Animal_Target)
        {
            if (!PV.IsMine)
                return;
        }


        if (Target == null)
        {
            //PV.RPC("AnimalTarget", RpcTarget.AllBuffered);
        }
        if (Target != null)
        {
            if ((!PV.IsMine) && (Ride_Start==false))
            {
                Debug.Log("Ride_Start1");
                if (playerController.Ride_State == true)
                {
                    PV.RPC("RPC_AnimalState", RpcTarget.AllBuffered, true);
                    Debug.Log("Ride_Start2");
                }
                else if (playerController.Ride_State == false)
                {
                    PV.RPC("RPC_AnimalState", RpcTarget.AllBuffered, false);
                    Debug.Log("Ride_Start3");
                }
                Ride_Start = true;
            }


            //if (!PV.IsMine)
            //return;

            if (PV.IsMine) 
            {
                distance = Vector3.Distance(transform.position, Target.transform.position);
                if ((distance <= 7) && (playerController.Ride_State == false) && (playerController.Ride_Try == true) && (Target.GetComponent<Animal>().RidePlayer == 0) && (Target.GetComponent<Animal>().Decode == true))
                {
                    AnimalState(true);

                    playerController.Ride_Try = false;
                }
                else if ((playerController.Ride_State == true) && (playerController.Ride_Try == true) && (Target.GetComponent<Animal>().RidePlayer == 1) && (Target.GetComponent<Animal>().Decode == true))
                {
                    AnimalState(false);
                    Ride_Over = true;

                    playerController.Ride_Try = false;
                }
            }

            if (playerController.Ride_State == true)
            {
                Ride(playerController.Ride_State);
            }
            
        }
    }
}
