using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class PlayerManager : MonoBehaviour
{
	PhotonView PV;

	public GameObject controller;
	public ButtonControl ButtonControl;
	public SettingController SettingController;
	public bool PC;
	public int Character_Number=-1;
	public bool Team;//Ture=Red ; False=Blue
	public int kill_times, death_times, death_times_limit;
	public bool kill,death, death_over, TeamSet, TeamSetCheck,AI_Mode, AI_Destroy, Destroy, Replace, death_times_over;
	public string PlayerName;
	public GameObject Target, Aim_Target, PlayerController;
	public Transform spawnpoint;
	public PlayerManager playerManager;
	public Spawnpoint_Group SpawnpointGroup;
	public bool StartOver;

	void Awake()
	{
		PV = GetComponent<PhotonView>();
		playerManager = GetComponent<PlayerManager>();
		SpawnpointGroup = GameObject.Find("SpawnManager_ALL").GetComponent<Spawnpoint_Group>();
	}

	void Start()
	{
	}


	void Update()
	{

		if ((StartOver == false)  && (SpawnpointGroup.SelectOver == true))
		{
			if (PV.IsMine)
			{
				CreateController();

			}
			StartOver = true;
		}

		if (death_times == death_times_limit)
		{
			death_times_over = true;
		}
		if ((AI_Mode)&&(controller!=null))
		{
			controller.GetComponent<PlayerController>().playerManager = playerManager;
			controller.GetComponent<PlayerController>().AI_Mode = true;
			controller.GetComponent<PlayerController>().PlayerName = PlayerName;
			controller.GetComponent<PlayerController>().Character_Number = Character_Number;
		}


		if ((AI_Mode)&&(Destroy)&&(AI_Destroy==false))
		{
			PV.RPC("RPC_AI_Destroy", RpcTarget.AllBuffered,true);
		}

		if ((AI_Mode)&&(AI_Destroy)&& (controller != null)&&(PhotonNetwork.IsMasterClient))
		{
			PhotonNetwork.Destroy(controller);
		}


		if ((controller != null) && (controller.GetComponent<PlayerController>().Team != Team))
		{
			TeamSet = false;
		}
		else if(TeamSetCheck)
		{
			TeamSet = true;
		}

		if ((PV.IsMine)&&(death == true) && (death_over == false))
		{
			death_over = true;
			Invoke("Die_Invoke", 5);
		}

		if ((controller != null) && (PV.IsMine)&&(death==false))
		{

			if (Character_Number >= 0)
			{
				controller.GetComponent<PlayerController>().Character_Number = Character_Number;
			}

			if (PC == true)
			{
				controller.GetComponent<PlayerController>().PC = true;
				if (Input.GetKeyDown("p"))
				{
					PC = false;
				}
			}
			else
			{
				controller.GetComponent<PlayerController>().PC = false;
				if (Input.GetKeyDown("p"))
				{
					PC = true;
				}
			}
		}
	}
	public void kill_times_add()
	{
		kill = true;
		PV.RPC("RPC_kill_times_add", RpcTarget.AllBuffered, kill);
	}
	[PunRPC]
	void RPC_AI_Destroy(bool Destroy_bool)
	{
		AI_Destroy = Destroy_bool;
		Destroy = Destroy_bool;
	}

	public void AI_Replace(bool Replace_bool)
	{
		if (Replace == false)
		{
			PV.RPC("RPC_AI_Replace", RpcTarget.AllBuffered, Replace_bool);
		}
	}

	[PunRPC]
	void RPC_AI_Replace(bool Replace_bool)
	{
		Replace = Replace_bool;
	}
	[PunRPC]
	void RPC_Death_State(bool Death_State)
	{
		death = Death_State;
	}

	[PunRPC]
	void RPC_death_times_add(int add_time)
	{
		death = true;
		death_times += add_time;
	}

	[PunRPC]
	void RPC_kill_times_add(bool kill)
	{
		if (kill == true)
		{
			kill_times += 1;
			kill =false;
		}
	}
	public void Team_Set(bool Team)
	{
		if (PV.IsMine)
		{
			PV.RPC("RPC_Team_Set", RpcTarget.AllBuffered, Team);
		}
	}

	[PunRPC]
	void RPC_Team_Set(bool RPC_Team)
	{
		Team = RPC_Team;
		TeamSetCheck = true;
	}
	[PunRPC]
	void RPC_CreateController(bool Create)
	{
		if (Create)
		{
			controller = PhotonNetwork.InstantiateRoomObject(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
			controller.GetComponent<PlayerController>().playerManager = playerManager;
			controller.GetComponent<PlayerController>().AI_Mode = true;
			controller.GetComponent<PlayerController>().PlayerName = PlayerName;
			controller.GetComponent<PlayerController>().Character_Number = Character_Number;
		}
		else
		{
			Destroy(controller);
		}
	}
	void CreateController()
	{
		if (Team)
		{
			spawnpoint = SpawnManager.Instance.GetSpawnpoint(0,2);
		}
		else
		{
			spawnpoint = SpawnManager.Instance.GetSpawnpoint(3,5);
		}
		if (AI_Mode == false)
		{
			controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
			controller.GetComponent<PlayerController>().playerManager = playerManager;
			ButtonControl = GameObject.Find("PlayrControl").GetComponent<ButtonControl>();
			SettingController= GameObject.Find("Control_UI").GetComponent<SettingController>();
			ButtonControl.Player = controller;
			SettingController.Player = controller;
			controller.GetComponent<PlayerController>().Character_Number = Character_Number;
		}
		else if (AI_Mode)
		{
			controller = PhotonNetwork.InstantiateRoomObject(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
			controller.GetComponent<PlayerController>().playerManager = playerManager;
			controller.GetComponent<PlayerController>().AI_Mode = true;
			controller.GetComponent<PlayerController>().PlayerName = PlayerName;
			controller.GetComponent<PlayerController>().Character_Number = Character_Number;
		}

		PV.RPC("RPC_Death_State", RpcTarget.AllBuffered, false);
		death_over = false;
	}
	public void Die()
	{
		PV.RPC("RPC_death_times_add", RpcTarget.AllBuffered, 1);
	}

	public void Die_Invoke()
	{
		CancelInvoke("Die_Invoke");
		if (controller != null)
		{
			PhotonNetwork.Destroy(controller);
		}
		if (death_times < death_times_limit)
		{
			CreateController();
		}
	}
}