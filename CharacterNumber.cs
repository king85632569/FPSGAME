using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterNumber : MonoBehaviour
{
    // Start is called before the first frame update
    public RoomManager RoomManager;
    public int Character_Number;
    public GameObject[] CharacterImage;
    //public GameObject[] TeamImage;
    public bool Team;//Ture=Red ; False=Blue
    public int PlayerNumber=0;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Character_Number = RoomManager.Character_Number;
        /*
        Team= RoomManager.Team;

        if (Team == true)
        {
            TeamImage[0].SetActive(true);
            TeamImage[1].SetActive(false);
        }
        else if (Team == false)
        {
            TeamImage[1].SetActive(true);
            TeamImage[0].SetActive(false);
        }*/

        for (int i = 0; i < CharacterImage.Length; i++)
        {
            if (Character_Number == i)
            {
                CharacterImage[i].SetActive(true);
            }
            else
            {
                CharacterImage[i].SetActive(false);
            }
        }
    }
}
