using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class ScoreboardItem : MonoBehaviour
{
	public TMP_Text usernameText;
	public TMP_Text killsText;
	public TMP_Text deathsText;
	public PlayerManager playerManager;
	public string playerName;
	public bool nameSet;

	void Update()
	{
		//item.playerManager = playerManager;
		if (playerManager != null)
		{
			kill_death(playerManager.kill_times, playerManager.death_times);
		}
		else
		{
			Destroy(gameObject);
		}
		if((playerName!=null)&&(usernameText.text!=playerName))
		{
			Initialize(playerName);
		}
	}

	public void Initialize(string Nickname)
	{
		usernameText.text = Nickname;
	}
	public void kill_death(int kills, int deaths)
	{
		killsText.text = kills.ToString();
		deathsText.text = deaths.ToString();
	}
}
