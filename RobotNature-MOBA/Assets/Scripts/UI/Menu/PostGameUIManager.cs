using GameStates;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{
    public class PostGameUIManager : MonoBehaviour
    {
        public static PostGameUIManager Instance;

        [SerializeField] private GameObject postGameCanvasWinner;
        [SerializeField] private GameObject postGameCanvasLoser;
        [SerializeField] private TextMeshProUGUI winningTeamText;
        [SerializeField] private TextMeshProUGUI winningText;
        [SerializeField] private TextMeshProUGUI loserTeamText;
        [SerializeField] private TextMeshProUGUI loserText;

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
            if (GameStateMachine.Instance.GetPlayerTeam() == winner)
            {
                postGameCanvasLoser.SetActive(false);
                postGameCanvasWinner.SetActive(true);
                winningTeamText.text = $"{winner} a gagné!";
                winningText.text = "Vous avez gagné!";
            }
            else
            {
                postGameCanvasWinner.SetActive(false);
                postGameCanvasLoser.SetActive(true);
                loserTeamText.text = $"{GameStateMachine.Instance.GetPlayerTeam()} a perdu!";
                loserText.text = "Vous avez perdu!";
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