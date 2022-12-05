using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingController : MonoBehaviour
{
    public float mouseSensitivity;
    public Slider Sensitivity_Slider;

    public float All_Volume;
    public Slider All_Volume_Slider;

    public float Music_Volume;
    public Slider Music_Volume_Slider;

    public float Game_Volume;
    public Slider Game_Volume_Slider;

    public GameObject Player;
    // Start is called before the first frame update
    void Start()
    {
        GetFloat();
    }

    // Update is called once per frame
    void Update()
    {
        SetFloat();
        if (Player == null)
            return;
        Sensitivity();
    }
    public void Sensitivity()
    {
        Player.GetComponent<PlayerController>().mouseSensitivity = mouseSensitivity*20f;
    }

    public void SetFloat()
    {
        mouseSensitivity = Sensitivity_Slider.value;
        PlayerPrefs.SetFloat("mouseSensitivity", mouseSensitivity);

        All_Volume = All_Volume_Slider.value;
        PlayerPrefs.SetFloat("All_Volume", All_Volume);

        Music_Volume = Music_Volume_Slider.value;
        PlayerPrefs.SetFloat("Music_Volume", Music_Volume);

        Game_Volume = Game_Volume_Slider.value;
        PlayerPrefs.SetFloat("Game_Volume", Game_Volume);
    }

    public void GetFloat()
    {
        Sensitivity_Slider.value = PlayerPrefs.GetFloat("mouseSensitivity",0.5F);
        All_Volume_Slider.value = PlayerPrefs.GetFloat("All_Volume", 0.5F);
        Music_Volume_Slider.value = PlayerPrefs.GetFloat("Music_Volume", 0.5F);
        Game_Volume_Slider.value = PlayerPrefs.GetFloat("Game_Volume", 0.5F);
    }

    public void DeleteFloat()
    {
        Debug.Log("­«¸m");
        //PlayerPrefs.DeleteKey("mouseSensitivity");
        Sensitivity_Slider.value = 0.5f;
        //PlayerPrefs.DeleteKey("All_Volume");
        All_Volume_Slider.value = 0.5f;
        //PlayerPrefs.DeleteKey("Music_Volume");
        Music_Volume_Slider.value = 0.5f;
        //PlayerPrefs.DeleteKey("Game_Volume");
        Game_Volume_Slider.value = 0.5f;
    }
}
