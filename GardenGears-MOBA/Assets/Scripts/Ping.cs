using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class Ping : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ping;
    [SerializeField] private TextMeshProUGUI master;
    // [SerializeField] TMP_InputField tickInput;
    
    void Start()
    {
        ping = GetComponent<TextMeshProUGUI>();
        master.text = $"Master : {PhotonNetwork.IsMasterClient}";
    }
    
    void Update()
    {
        ping.text = $"Ping : {PhotonNetwork.GetPing()}";
    }
    
    /*

    public void UpdateTick()
    {
        PhotonNetwork.SerializationRate = Int32.Parse(tickInput.text);
    }
    
    */
}