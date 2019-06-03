using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CarState 
{
	public CarState(Car myCar)
	{
		this.car = myCar;
	}

	protected Car car;

	private bool wasInitialized = false;

	public string UpdateState()
	{
		BeforeTick();

		return Tick();
	}

	public void Exit()
	{
		wasInitialized = false;
		OnStateExit();
	}

	private void BeforeTick()
	{
		if(wasInitialized == false)
		{
			OnStateEnter();
			wasInitialized = true;
		}
	}

	protected abstract string Tick();

	protected virtual void OnStateEnter(){}
	protected virtual void OnStateExit(){}
}
