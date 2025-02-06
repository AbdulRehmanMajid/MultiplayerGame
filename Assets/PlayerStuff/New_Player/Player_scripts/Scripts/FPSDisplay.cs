using UnityEngine;
using TMPro;



 
public class FPSDisplay : MonoBehaviour {
	[SerializeField] private TextMeshProUGUI FpsText;
	
	private float pollingTime = 1f;
	private float time;
	private int frameCount;
   
 
	void Update() {
		
		time += Time.deltaTime;

		
		frameCount++;

		if (time >= pollingTime) {
			
			int frameRate = Mathf.RoundToInt((float)frameCount / time);
			FpsText.text = frameRate.ToString() + " fps";

			
			time -= pollingTime;
			frameCount = 0;
		}
	}
}