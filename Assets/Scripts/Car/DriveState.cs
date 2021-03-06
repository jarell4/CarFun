﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriveState : CarState
{
	public DriveState(Car car) : base(car){}

	protected override void OnStateEnter()
	{
		ShowCarInDrive();
	}

	protected override string Tick()
	{
		car.Drive(car.transform.forward);
		return null;
	}

	private void ShowCarInDrive()
	{
		car.ChangeColor(Color.blue);
	}
}
