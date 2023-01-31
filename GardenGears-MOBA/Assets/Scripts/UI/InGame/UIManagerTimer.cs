using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.InGame;
using UnityEngine;

namespace UI.InGame
{
    public partial class UIManager
    {
        [Header("Timer")] 
        [SerializeField] private TextMeshProUGUI timerText;

        public void UpdateTimerText(int minute, int second)
        {
            if(second < 10) timerText.text = $"{minute} : 0{second}";
            else timerText.text = $"{minute} : {second}";
        }
    }
}
