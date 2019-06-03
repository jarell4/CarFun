using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour 
{
	[SerializeField]
	private GameObject redLight;
	[SerializeField]
	private GameObject yellowLight;
	[SerializeField]
	private GameObject greenLight;

	[SerializeField]
	private float slowLightTime = 1f;

	private LightType lightState;

	public void SetGo()
	{
		if(lightState == LightType.Red)
		{
			lightState = LightType.Green;
			UpdateLights();
		}
	}

	public void SetStop()
	{
		if(lightState != LightType.Red)
		{
			lightState = LightType.Yellow;
			UpdateLights();

			StartCoroutine(SetStateToStopAfterSlowLight());
		}
	}

	private void UpdateLights()
	{
		DisableAllLights();
		
		switch(lightState)
		{
			case LightType.Red:
				redLight.SetActive(true);
				break;

			case LightType.Yellow:
				yellowLight.SetActive(true);
				break;

			case LightType.Green:
				greenLight.SetActive(true);
				break;
		}
	}

	private void DisableAllLights()
	{
		redLight.SetActive(false);
		yellowLight.SetActive(false);
		greenLight.SetActive(false);
	}

	private IEnumerator SetStateToStopAfterSlowLight()
	{
		yield return new WaitForSeconds(slowLightTime);

		lightState = LightType.Red;
		UpdateLights();
	}
}
