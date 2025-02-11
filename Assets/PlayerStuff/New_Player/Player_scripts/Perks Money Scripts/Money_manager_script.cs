using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class Money_manager_script : NetworkBehaviour
{
    #region Public Fields
    [SerializeField] public NetworkVariable<int> Money = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] public NetworkVariable<int> Salvage = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public TextMeshProUGUI money_text;
    #endregion

    #region Unity Methods
    void Update()
    {
        if (!IsOwner) 
            return;
        
        money_text.text = Salvage.Value.ToString();
    }
    #endregion

    #region Server RPC Methods
    [ServerRpc(RequireOwnership = false)]
    public void TakeMoney_ServerRpc(int amount)
    {
        Money.Value -= amount;
    }

    [ServerRpc(RequireOwnership = false)]
    public void GiveMoney_debug_ServerRpc(int amount)
    {
        Money.Value += amount;
    }

    [ServerRpc(RequireOwnership = false)]
    public void GiveSalvage_debug_ServerRpc(int amount)
    {
        Salvage.Value += amount;
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeSalvage_ServerRpc(int amount)
    {
        Salvage.Value -= amount;
    }
    #endregion
}
