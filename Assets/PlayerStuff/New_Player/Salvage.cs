using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Salvage : NetworkBehaviour
{
    public GameObject salvage;
    public GameObject mesh;
    public int Value;

    private MeshRenderer meshRenderer;
    private NetworkObject salvageNetworkObject;

    void Start()
    {
        if (mesh != null)
            meshRenderer = mesh.GetComponent<MeshRenderer>();
        if (salvage != null)
            salvageNetworkObject = salvage.GetComponent<NetworkObject>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player_Health>(out Player_Health playerHealth))
        {
            if (meshRenderer != null)
                meshRenderer.enabled = false;

            if (!IsServer)
                return;

            if (salvageNetworkObject != null)
                salvageNetworkObject.Despawn(true);

            playerHealth.money_man.Salvage.Value += Value;
        }
    }
}
