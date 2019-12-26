using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] switches;

    [SerializeField]
    GameObject exitDoor;

    [SerializeField]
    Text switchCounter;

    int noOfswitches = 0;

    private void Start()
    {
        GetNoOfSwitches();
    }

    public int GetNoOfSwitches()
    {

        int x = 0;

        for (int i = 0; i < switches.Length; i++)
        {
            if(switches[i].GetComponent<ButtonSwitches>().isOn == false)
            {
                x++;
            }
            else if (switches[i].GetComponent<ButtonSwitches>().isOn == true)
            {
                noOfswitches--;
            }

        }

        noOfswitches = x;

        return noOfswitches;
    }

    public void DoorState()
    {

    }
}
