using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Bubble : MonoBehaviourPunCallbacks
{
	public float damage,DamageNerf;
	public bool Team;
	public bool BloodAddition;
	public PlayerManager Enemy;
	public bool DizzyState;
	public bool Fly_Start, Fly_State, Start_Set, Bubble_Size_Set, Bubble_Fly;
	public int Player_ViewID;
	public Rigidbody rb;
	public PhotonView PV;
	public Collider Collider_Physical, Collider_Trigger;
	public GameObject Bubble_Blue,Bubble_Red, Bubble_My;

	public float Bubble_Size;
	public float Bubble_Force;

	public GameObject PlayerManager_Obj;
	public PlayerManager My_PlayerManager;
	public GameObject[] All_PlayerManager;

	public GameObject Fly_Player;
	public GameObject[] All_Player;
	public int Fly_Player_ViewID=0;
	public bool Fly_Over, Fly_Stop, FlyOut;
	public bool Ride_Shoot;
	public bool Octopus_Mode;

	// Start is called before the first frame update
	void Start()
    {
		rb = GetComponent<Rigidbody>();
		PV = GetComponent<PhotonView>();
		transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		if (Ride_Shoot)
		{
			Bubble_Size = 1.5f;
			DamageNerf = 0.5f;
		}
		else
		{
			Bubble_Size = 0.5f;
			DamageNerf = 0.1f;
		}

		PlayerManager_Obj = GameObject.Find("RoomManager").GetComponent<RoomManager>().PlayerManager;
		My_PlayerManager = PlayerManager_Obj.GetComponent<PlayerManager>();
	}

	// Update is called once per frame
	void Update()
    {

		if (PV.IsMine)
		{
			if (Fly_Start == true)
			{
				PV.RPC("RPC_Bubble_Start", RpcTarget.All, Bubble_Size, DamageNerf);
				if (Bubble_Fly == false)
				{
					Invoke("Bubble_Stop", 5);
				}
			}
		}
		if (Start_Set == false)
		{
			if ((Bubble_Size < 3)&&(Bubble_Size_Set==false))
			{
				Bubble_Size += 0.6f * Time.deltaTime;
			}
			else if ((Bubble_Size >= 3) && (Bubble_Size_Set == false))
			{
				PV.RPC("RPC_Bubble_Size", RpcTarget.All);
			}

			if (DamageNerf < 1)
			{
				DamageNerf += 0.09f * Time.deltaTime;
			}
			else
			{
				DamageNerf = 1;
			}
		}
		if (Fly_Over == true)
		{
			Bubble_Blue.SetActive(false);
			Bubble_Red.SetActive(false);
			//Bubble_My.SetActive(false);
			if (Fly_Player == null)
				return;
			if (Fly_Player.GetComponent<PlayerController>().Fall_Damage)
			{
				//Fly_Player.GetComponent<Collider>().gameObject.GetComponent<Hit>()?.Hit_Trigger(30, Team, BloodAddition, Enemy, true);
				Fly_Player.GetComponent<IDamageable>().TakeDamage(30, Enemy);
				Fly_Player.GetComponent<IDamageable>().Hurt_Show(30, false);
				Fly_Player.GetComponent<IDamageable>().SetFall_Damage(false);
			}
		}
		else if ((Enemy!=null)||(Octopus_Mode))
		{
			/*
			if (My_PlayerManager == Enemy)
			{
				Bubble_My.SetActive(true);
			}*/
			if (My_PlayerManager.Team == Team)
			{
				Bubble_Blue.SetActive(true);
			}
			else if (My_PlayerManager.Team != Team)
			{
				Bubble_Red.SetActive(true);
			}
		}
		transform.localScale = new Vector3(Bubble_Size, Bubble_Size, Bubble_Size);
		Find_Enemy();
		if ((Fly_Player_ViewID != 0)&&(Fly_Player == null)) 
		{
			Set_Fly_Player();
		}
	}
	public void Find_Enemy()
	{
		All_PlayerManager = GameObject.FindGameObjectsWithTag("PlayerManager");
		for (int i = 0; i < All_PlayerManager.Length; i++)
		{
			if (All_PlayerManager[i].GetComponent<PhotonView>().ViewID == Player_ViewID)
			{
				Enemy = All_PlayerManager[i].GetComponent<PlayerManager>();
			}
		}
	}

	public void Set_Fly_Player()
	{
		All_Player = GameObject.FindGameObjectsWithTag("Move_Collider");
		for (int i = 0; i < All_Player.Length; i++)
		{
			if (All_Player[i].GetComponent<PhotonView>().ViewID == Fly_Player_ViewID)
			{
				Fly_Player = All_Player[i];
				Fly_Player.GetComponent<PlayerController>().Fly_Bubble = this.gameObject;
			}
		}
	}

	public void Bubble_Set(float Bubble_damage, bool Bubble_Team, bool Bubble_BloodAddition, bool Bubble_DizzyState,int Player_ViewID,bool AI_Mode)
	{
		PV.RPC("RPC_Bubble_Set", RpcTarget.All, Bubble_damage, Bubble_Team, Bubble_BloodAddition, Bubble_DizzyState, Player_ViewID, AI_Mode);
	}


	[PunRPC]
	public void RPC_Bubble_Set(float Bubble_damage, bool Bubble_Team, bool Bubble_BloodAddition, bool Bubble_DizzyState,int ViewID, bool AI_Mode)
	{
		damage = Bubble_damage;
		Team = Bubble_Team;
		BloodAddition = Bubble_BloodAddition;
		DizzyState = Bubble_DizzyState;
		if (AI_Mode)
		{
			Octopus_Mode = AI_Mode;
		}
		else
		{
			Player_ViewID = ViewID;
		}
		//Player_ViewID = RPC_ViewID;
	}

	[PunRPC]
	public void RPC_Bubble_Size()
	{
		Bubble_Size = 3;
		Bubble_Size_Set = true;
		Bubble_Fly = true;
	}

	[PunRPC]
	public void RPC_Bubble_Start(float Size, float Nerf)
	{
		Start_Set = true;
		Bubble_Size = Size;
		DamageNerf = Nerf;
		Collider_Physical.enabled = true;
		Collider_Trigger.enabled = true;
		rb.useGravity = true;
		Fly_Start = false;
		rb.AddForce(transform.forward * 20*(1/DamageNerf));
	}

	public void Bubble_Fly_Over()
	{
		PV.RPC("RPC_Bubble_Fly_Over", RpcTarget.All);
	}

	public void Bubble_Destroy()
	{
		PV.RPC("RPC_Bubble_Trigger", RpcTarget.All, true, DamageNerf);
	}

	public void Bubble_Stop()
	{
		PV.RPC("RPC_Bubble_Trigger", RpcTarget.All, false, DamageNerf);
	}

	[PunRPC]
	public void RPC_Bubble_Fly_Over()
	{
		Fly_Over = true;
		rb.velocity = Vector3.zero;
		rb.useGravity = false;
		Invoke("Bubble_Destroy", 30);
	}

	[PunRPC]
	public void RPC_Bubble_Trigger(bool DestroyState,float Nerf)
	{
		if (DestroyState)
		{
			Destroy(this.gameObject);
		}
		if (Bubble_Fly == true)
		{
			FlyOut = true;
			this.transform.localEulerAngles = new Vector3(0, 0, 0);
			rb.velocity = Vector3.zero;
			DamageNerf = Nerf;
			rb.AddForce(transform.up * 30 * (1 / DamageNerf));
			Collider_Physical.enabled = false;
			Collider_Trigger.enabled = false;
			Invoke("Bubble_Fly_Over", 2.5f);
		}
		else if (Bubble_Fly == false)
		{
			//Collider_Physical.enabled = false;
			//rb.useGravity = false;
			rb.velocity = Vector3.zero;
			Fly_Stop = true;
		}
	}
	[PunRPC]
	public void RPC_Fly_Plaey_ViewID(int ViewID)
	{
		Fly_Player_ViewID = ViewID;
	}

	void OnTriggerEnter(Collider other)
	{
		if (PhotonNetwork.IsMasterClient) 
		{
			if (other.gameObject.tag == "Play_Trigger")
			{
				GameObject EnemyPlayer = other.GetComponent<Hit>().Player;
				if (Team != EnemyPlayer.GetComponent<PlayerController>().Team)
				{
					if (Octopus_Mode)
					{
						if (Fly_Stop == false)
						{
							PV.RPC("RPC_Bubble_Trigger", RpcTarget.All, false, DamageNerf);
							other.GetComponent<Collider>().gameObject.GetComponent<Hit>()?.TakeDamage_Monster(Mathf.Round(damage * DamageNerf));
						}
						else
						{
							other.GetComponent<Collider>().gameObject.GetComponent<Hit>()?.TakeDamage_Monster(Mathf.Round(damage * DamageNerf * 0.5f));
						}
					}
					else if ((Bubble_Fly) && (EnemyPlayer.GetComponent<PlayerController>().Dizzy == false))
					{
						PV.RPC("RPC_Bubble_Trigger", RpcTarget.All, false, DamageNerf);
						//other.GetComponent<Collider>().gameObject.GetComponent<Hit>()?.Hit_Trigger(Mathf.Round(damage * DamageNerf), Team, BloodAddition, Enemy, true);
						other.GetComponent<Collider>().gameObject.GetComponent<Hit>()?.Hit_Trigger(0, Team, BloodAddition, Enemy, true);
						int ViewID = EnemyPlayer.GetComponent<PhotonView>().ViewID;
						PV.RPC("RPC_Fly_Plaey_ViewID", RpcTarget.All, ViewID);
					}
					else if (Bubble_Fly == false)
					{
						if (Fly_Stop == false)
						{
							PV.RPC("RPC_Bubble_Trigger", RpcTarget.All, false, DamageNerf);
							other.GetComponent<Collider>().gameObject.GetComponent<Hit>()?.Hit_Trigger(Mathf.Round(damage * DamageNerf), Team, BloodAddition, Enemy, false);
						}
						else
						{
							other.GetComponent<Collider>().gameObject.GetComponent<Hit>()?.Hit_Trigger(Mathf.Round(damage * DamageNerf * 0.5f), Team, BloodAddition, Enemy, false);
						}
					}
				}
				else
				{
					//PlayerManager PlayerManager = EnemyPlayer.GetComponent<PlayerController>().playerManager;
					//if ((PlayerManager.GetComponent<PhotonView>().ViewID == Player_ViewID) && (Fly_Stop == true))
					if ((Bubble_Fly==false) && (Fly_Stop == true))
					{
						other.GetComponent<Collider>().gameObject.GetComponent<Hit>()?.Bubble_Addition();
						PV.RPC("RPC_Bubble_Trigger", RpcTarget.All, true, DamageNerf);
					}
				}
			}
			if (other.gameObject.tag == "Shield")
			{

			}
			if (other.gameObject.tag == "Grass")
			{
				other.gameObject.GetComponent<GrassBroken>().Broken = true;
			}
			if (other.gameObject.tag == "Enemy")
			{
				if ((other.GetComponent<AI_Monster>().Prisoner) || (other.GetComponent<AI_Monster>().Octopus))
				{
					if (Team != other.GetComponent<AI_Monster>().Monster_Team)
					{
						other.gameObject.GetComponent<AI_Monster>().Hit_Trigger(Mathf.Round(damage * DamageNerf), Team);
						other.gameObject.GetComponent<AI_Monster>().TargetPlayerViewID(Player_ViewID);
						if (Bubble_Fly == false)
						{
							PV.RPC("RPC_Bubble_Trigger", RpcTarget.All, false, DamageNerf);
						}
					}
				}
				else
				{
					other.gameObject.GetComponent<AI_Monster>().Hit_Trigger(Mathf.Round(damage * DamageNerf), Team);
					other.gameObject.GetComponent<AI_Monster>().TargetPlayerViewID(Player_ViewID);
					if (Bubble_Fly == false)
					{
						PV.RPC("RPC_Bubble_Trigger", RpcTarget.All, false, DamageNerf);
					}
				}

			}
		}
	}
}
