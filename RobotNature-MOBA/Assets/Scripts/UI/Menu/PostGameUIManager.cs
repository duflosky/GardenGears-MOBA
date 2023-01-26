using GameStates;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.Menu
{
    public class PostGameUIManager : MonoBehaviour
    {
        public static PostGameUIManager Instance;

        [SerializeField] private GameObject postGameCanvasOrange;
        [SerializeField] private GameObject postGameCanvasPurple;
        [SerializeField] private TextMeshProUGUI orangeTeamText;
        [SerializeField] private TextMeshProUGUI orangeText;
        [SerializeField] private TextMeshProUGUI purpleTeamText;
        [SerializeField] private TextMeshProUGUI purpleText;

        [SerializeField] private Button rematchButton;
    
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                DestroyImmediate(gameObject);
                return;
            }

            Instance = this;
        }

        public void DisplayPostGame(Enums.Team winner)
        {
            if (GameStateMachine.Instance.GetPlayerTeam() == Enums.Team.Team1)
            {
                if (GameStateMachine.Instance.GetPlayerTeam() == winner)
                {
                    postGameCanvasPurple.SetActive(false);
                    postGameCanvasOrange.SetActive(true);
                    orangeTeamText.text = $"{winner} a gagné!";
                    orangeText.text = "Vous avez gagné!";
                }
                else
                {
                    postGameCanvasPurple.SetActive(false);
                    postGameCanvasOrange.SetActive(true);
                    orangeTeamText.text = $"{GameStateMachine.Instance.GetPlayerTeam()} a perdu!";
                    orangeText.text = "Vous avez perdu!";
                }
            }
            else if (GameStateMachine.Instance.GetPlayerTeam() == Enums.Team.Team2)
            {
                if (GameStateMachine.Instance.GetPlayerTeam() == winner)
                {
                    postGameCanvasOrange.SetActive(false);
                    postGameCanvasPurple.SetActive(true);
                    purpleTeamText.text = $"{GameStateMachine.Instance.GetPlayerTeam()} a gagné!";
                    purpleText.text = "Vous avez gagné!";
                }
                else
                {
                    postGameCanvasOrange.SetActive(false);
                    postGameCanvasPurple.SetActive(true);
                    orangeTeamText.text = $"{GameStateMachine.Instance.GetPlayerTeam()} a perdu!";
                    orangeText.text = "Vous avez perdu!";
                }
            }
        }

        public void OnRematchClick()
        {
            // rematchButton.interactable = false;
            // GameStateMachine.Instance.SendSetToggleReady(true);
            Application.Quit();
        }
    }
}