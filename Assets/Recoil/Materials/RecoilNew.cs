using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class RecoilNew : NetworkBehaviour
{

    [Header("Recoil Settings")]
	public float rotationSpeed = 6;
	public float returnSpeed = 25;
    public float firerate;
	
    bool can_Shoot = true;
	[Space()]

	[Header("Hipfire:")]
	public Vector3 RecoilRotation = new Vector3(2f, 2f, 2f);
	[Space()]

	[Header("Aiming")]
	public Vector3 RecoilRotationAiming = new Vector3(0.5f, 0.5f, 1.5f);
	[Space()]

	[Header("State")]
	public bool aiming;

	private Vector3 currentRotation;
	private Vector3 Rot;
	public float recoil_reduction = 2f;

	private void FixedUpdate()
	{
		if(!IsOwner)return;
		currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, returnSpeed * Time.deltaTime);
		
		Rot = Vector3.Slerp(Rot, currentRotation, rotationSpeed * Time.fixedDeltaTime);
		transform.localRotation = Quaternion.Euler(Rot);
	}

	public void Fire()
	{
		if(!IsOwner)return;
		if (aiming)
		{
			Vector3 recoil = new Vector3(-RecoilRotationAiming.x, Random.Range(-RecoilRotationAiming.y, RecoilRotationAiming.y), Random.Range(-RecoilRotationAiming.z, RecoilRotationAiming.z));
			currentRotation += recoil;
			
			
		}
		else
		{
			Vector3 recoil_aim = new Vector3(-RecoilRotationAiming.x, Random.Range(-RecoilRotationAiming.y, RecoilRotationAiming.y), Random.Range(-RecoilRotationAiming.z, RecoilRotationAiming.z));
			currentRotation += recoil_aim;
			
		}
	}

	private void Update()
	{
		if(!IsOwner)return;
		if (Input.GetKey(KeyCode.Mouse0) && can_Shoot)
		{
			//StartCoroutine(shoot());
		}
		if (Input.GetKey(KeyCode.Mouse1))
		{
			aiming = true;
		}
		else
		{
			aiming = false;
		}
	}
    IEnumerator shoot()
    {
        Fire();
        can_Shoot = false;
        yield return new WaitForSeconds(firerate);
        can_Shoot = true;

    }
	public void SetRecoil(float return_speed,float RotSpeed,Vector3 recoil)
	{
		if(!IsOwner)return;
		returnSpeed = return_speed;
		rotationSpeed = RotSpeed;
		RecoilRotationAiming = recoil;

	}
}
