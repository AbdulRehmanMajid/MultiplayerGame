using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
public class ShootingAi : NetworkBehaviour
{
   
    public NetworkVariable<float> Health = new NetworkVariable<float>(300,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    public GameObject healthbarui;
    public Slider slider;
    public GameObject Cube;
    public float max_health = 300f;

    void Awake()
    {
        slider.value = calculateHealth();
    }
    float calculateHealth()
    {
        return Health.Value / max_health;

    }
    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float amount)
    {
        Health.Value -= amount;
        slider.value = Health.Value / max_health;
        updatehealthClientRpc();
        

        if(Health.Value<=0)
        {
            
            Health.Value = 300f;
            
            
        }
        

    }

    [ClientRpc]
    void updatehealthClientRpc()
    {
        slider.value = Health.Value / max_health;

    }
    
    IEnumerator lol()
    {
        
        Instantiate(Cube,this.transform.position + new Vector3(0f,3f,0f),this.transform.rotation);
        yield return new WaitForSeconds(0.25f);
        Destroy(this.gameObject);
        

    }

    void Update()
    {
        slider.value = calculateHealth();
    }

    

}
