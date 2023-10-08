using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuTracking : MonoBehaviour
{
    public Image VictoryEZ;     public Image VictoryMi;     public Image Victoryha;

    // Start is called before the first frame update
    void Start()
    {
    StaticTracker.totalUpgrade=0;
    if (StaticTracker.E1beaten){VictoryEZ.fillAmount=1.0f;}
    if (StaticTracker.E2beaten){VictoryMi.fillAmount=1.0f;}
    if (StaticTracker.E3beaten){Victoryha.fillAmount=1.0f;}

    }

    // Update is called once per frame
    
}
