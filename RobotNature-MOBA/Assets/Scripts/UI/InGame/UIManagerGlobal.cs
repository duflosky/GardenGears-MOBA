using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI.InGame
{
    public partial class UIManager
    {
        [Header("Global UI")]
        [SerializeField] TextMeshProUGUI team1KillCounter;
        [SerializeField] TextMeshProUGUI team2KillCounter;


        public void UpdateKillCount(int team1, int team2)
        {
            team1KillCounter.text = team1.ToString();
            team2KillCounter.text = team2.ToString();
        }
    }
}