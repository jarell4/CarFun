using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarStateMachine : MonoBehaviour 
{
	[SerializeField]
	private string currentStateName;
	private CarState currentState;

	private Dictionary<string, CarState> possibleStates;

	void Update()
	{
		CheckNullSetDefaultState();
		
		string resultStateName = currentState.UpdateState();
		SetStateByName(resultStateName);
	}

	public void SetStates(Dictionary<string, CarState> stateDictionary)
	{
		possibleStates = stateDictionary;
	}

	public void SetStateByName(string wantedState)
	{
		if(wantedState != null &&
			wantedState != currentState.GetType().Name)
		{
			SwitchToNewState(wantedState);
		}
	}

	private void CheckNullSetDefaultState()
	{
		if(currentState == null)
		{
			SwitchToNewState("DriveState");
		}
	}

	private void SwitchToNewState(string newState)
	{
		currentState = possibleStates[newState];
		currentStateName = currentState.GetType().Name;
	}
}