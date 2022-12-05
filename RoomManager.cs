using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using System.IO;
using TMPro;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomManager : MonoBehaviourPunCallbacks
{
	public static RoomManager Instance;
	public GameObject PlayerManager;
	public int Character_Number;
	public bool Team;//Ture=Red ; False=Blue
	public bool MasterClient_In_Scene;
	public GameObject[] All_PlayerManager;
	public int Blue_Survive, Red_Survive;
	public TMP_Text text;
	public int PlayerNumber;
	public int mod;
	//public bool AI_Set;
	PhotonView PV;
	[SerializeField] Player[] players;

	void Awake()
	{
		if(Instance)
		{
			Destroy(gameObject);
			return;
		}
		Blue_Survive = 0;
		Red_Survive = 0;

		DontDestroyOnLoad(gameObject);
		Instance = this;
		PV = GetComponent<PhotonView>();
	}
	void Update()
	{
		//if ((PhotonNetwork.IsMasterClient) && (MasterClient_In_Scene))
		if (MasterClient_In_Scene)
		{
			//RoomOptions roomOptions = new RoomOptions();
			//roomOptions.CleanupCacheOnLeave = false;
			GetPlayer();
		}
	}
	public void GetPlayer()
	{
		Blue_Survive = 0;
		Red_Survive = 0;
		int x = 0;
		All_PlayerManager = GameObject.FindGameObjectsWithTag("PlayerManager");
		for (int i = 0; i < All_PlayerManager.Length; i++)
		{
			x += 1;
			if ((All_PlayerManager[i].GetComponent<PlayerManager>().Destroy == false)&& (All_PlayerManager[i].GetComponent<PlayerManager>().death_times_over == false))
			{
				if (All_PlayerManager[i].GetComponent<PlayerManager>().Team == true)
				{
					Red_Survive += 1;
				}
				else if (All_PlayerManager[i].GetComponent<PlayerManager>().Team == false)
				{
					Blue_Survive += 1;
				}
			}
		}
		if (x == All_PlayerManager.Length)
		{
			if (Red_Survive == 0)
			{
				text.text = "藍隊勝利";
			}
			else if (Blue_Survive == 0)
			{
				text.text = "紅隊勝利";
			}
		}
	}



	public void Character_Number_Add()
	{
		if((Character_Number>=0)&&(Character_Number < 3))
		{
			Character_Number += 1;
		}
		else
		{
			Character_Number = 0;
		}
	}


	public override void OnEnable()
	{
		base.OnEnable();
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	public override void OnDisable()
	{
		base.OnDisable();
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
	{
		if (scene.buildIndex == 1) // We're in the game scene
		{
			Debug.Log(PhotonNetwork.LocalPlayer.ActorNumber);
			PlayerNumber = PhotonNetwork.LocalPlayer.ActorNumber;
			int TeamSet = PlayerNumber % 2;
			Debug.Log(TeamSet);
			if (TeamSet == 1)
			{
				Team = true;
			}
			else
			{
				Team = false;
			}
			PlayerManager = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
			PlayerManager.GetComponent<PlayerManager>().Team_Set(Team);
			PlayerManager.GetComponent<PlayerManager>().Character_Number = Character_Number;
			MasterClient_In_Scene = true;
		}
	}
}