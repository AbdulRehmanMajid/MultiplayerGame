using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class world_gun_data_holder : NetworkBehaviour
{
             [SerializeField] public NetworkVariable<int> Gun_level = new NetworkVariable<int>(1,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
             [SerializeField] public NetworkVariable<int> Rareity_level = new NetworkVariable<int>(1,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
             public string Gun_Name;

}
