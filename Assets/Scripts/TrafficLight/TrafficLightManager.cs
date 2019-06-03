using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightManager : MonoBehaviour 
{
	[SerializeField]
	private TrafficLight[] trafficLights;

	void Start () 
	{
		InitializeTrafficLights();
		SetLightsToGo();
	}
	
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.A))
		{
			Debug.Log("Trying to set lights to go");
			SetLightsToGo();
		}

		if(Input.GetKeyDown(KeyCode.S))
		{
			Debug.Log("Trying to set lights to stop");
			SetLightsToStop();
		}
	}

	private void InitializeTrafficLights()
	{
		if(trafficLights.Length == 0)
		{
			trafficLights = GameObject.FindObjectsOfType<TrafficLight>();
		}
	}

	private void SetLightsToGo()
	{
		foreach(TrafficLight trafficLight in trafficLights)
		{
			trafficLight.SetGo();
		}
	}

	private void SetLightsToStop()
	{
		foreach(TrafficLight trafficLight in trafficLights)
		{
			trafficLight.SetStop();
		}
	}
}
