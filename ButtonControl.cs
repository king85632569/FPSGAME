using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonControl : MonoBehaviour
{
    public GameObject Player;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Player == null)
            return;

        if ((Player.GetComponent<PlayerController>().PC == true)&& (Player.GetComponent<PlayerController>().death == false)) {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            //RunButtonEnter();
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            //RunButtonExit();
        }
        if (Input.GetKeyDown("space"))
        {
            JumpButtonEnter();
        }
        else if (Input.GetKeyUp("space"))
        {
            JumpButtonExit();
        }
        if (Input.GetMouseButtonDown(0))
        {
            ShootButtonEnter();
        }
        if(Input.GetMouseButtonUp(0))
        {
            ShootButtonExit();
        }
        if (Input.GetKeyDown("r"))
        {
            RideButtonEnter();
        }
        else if (Input.GetKeyUp("r"))
        {
            RideButtonExit();
        }
        if (Input.GetKeyDown("e"))
        {
            SkillButtonEnter1();
        }
        else if (Input.GetKeyUp("e"))
        {
            SkillButtonExit1();
        }
        if (Input.GetKeyDown("q"))
        {
            SkillButtonEnter2();
        }
        else if (Input.GetKeyUp("q"))
        {
            SkillButtonExit2();
        }
        if (Input.GetKeyDown("f"))
        {
            OpenDoorButtonEnter();
        }
        else if (Input.GetKeyUp("f"))
        {
            OpenDoorButtonExit();
        }


        if (Input.GetMouseButtonDown(1))
        {
            AimButton();
        }
        if (Input.GetKeyDown("g"))
        {
            FirstPersonButton();
        }
        }
    }

    public void OpenDoorButtonEnter()
    {
        Player.GetComponent<PlayerController>().OpenDoor_State = true;
    }
    public void OpenDoorButtonExit()
    {
        Player.GetComponent<PlayerController>().OpenDoor_State = false;
    }

    public void SkillButtonEnter1()
    {
        Player.GetComponent<PlayerController>().Skill_State1 = true;
    }
    public void SkillButtonExit1()
    {
        Player.GetComponent<PlayerController>().Skill_State1 = false;
    }
    public void SkillButtonEnter2()
    {
        Player.GetComponent<PlayerController>().Skill_State2 = true;
    }
    public void SkillButtonExit2()
    {
        Player.GetComponent<PlayerController>().Skill_State2 = false;
    }

    public void AimButton()
    {
        if (Player.GetComponent<PlayerController>().Aim_State == false)
        {
            Player.GetComponent<PlayerController>().Aim_State = true;
        }
        else
        {
            Player.GetComponent<PlayerController>().Aim_State = false;
        }
    }
    public void FirstPersonButton()
    {
        if (Player.GetComponent<PlayerController>().FirstPerson == false)
        {
            Player.GetComponent<PlayerController>().FirstPerson = true;
        }
        else
        {
            Player.GetComponent<PlayerController>().FirstPerson = false;
        }
    }
    public void ShootButtonUI()
    {
        if (Player.GetComponent<PlayerController>().Shoot_State == true)
        {
            Player.GetComponent<PlayerController>().Shoot_State = false;
        }
        else
        {
            Player.GetComponent<PlayerController>().Shoot_State = true;
        }
    }
    public void ShootButtonEnter()
    {
        Player.GetComponent<PlayerController>().Shoot_State = true;
    }
    public void ShootButtonExit()
    {
        Player.GetComponent<PlayerController>().Shoot_State = false;
    }

    public void JumpButtonEnter()
    {
        Player.GetComponent<PlayerController>().Jump_State = true;
    }
    public void JumpButtonExit()
    {
        Player.GetComponent<PlayerController>().Jump_State = false;
    }

    public void RunButtonEnter()
    {
        Player.GetComponent<PlayerController>().Run_State = true;
    }
    public void RunButtonExit()
    {
        Player.GetComponent<PlayerController>().Run_State = false;
    }
    public void RideButtonEnter()
    {
        if (Player.GetComponent<PlayerController>().grounded == true) 
        {
            Player.GetComponent<PlayerController>().Ride_Try = true;
        }
    }
    public void RideButtonExit()
    {
        Player.GetComponent<PlayerController>().Ride_Try = false;
    }
}
