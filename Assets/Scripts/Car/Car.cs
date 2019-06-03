using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField]
    private float driveSpeed;

    private CarStateMachine myStateMachine;
    private MeshRenderer[] myRenderers;
    private Color[] myDefaultColors;

	void Awake()
    {
        InitializeStateMachine();
    }

    void Start()
    {
        myRenderers = this.GetComponentsInChildren<MeshRenderer>();
        StoreDefaultColors();
    }

    public void Drive(Vector3 direction)
	{
		Vector3 deltaPosition = direction * driveSpeed * Time.deltaTime;
		this.transform.position += deltaPosition;
	}

    public void ChangeColor(Color newColor)
    {
        foreach (MeshRenderer renderer in myRenderers)
        {
            renderer.material.color = newColor;
        }
    }

    public void RestoreDefaultColor()
    {
        for(int i = 0; i < myRenderers.Length; i++)
        {
            myRenderers[i].material.color = myDefaultColors[i];
        }
    }

    private void InitializeStateMachine()
    {
        myStateMachine = GetComponent<CarStateMachine>();

        var states = new Dictionary<string, CarState>()
        {
            { typeof(DriveState).Name, new DriveState(this) },
        };

        myStateMachine.SetStates(states);
    }

    private void StoreDefaultColors()
    {
        myDefaultColors = new Color[myRenderers.Length];

        for(int i = 0; i < myRenderers.Length; i++)
        {
            myDefaultColors[i] = myRenderers[i].material.color;
        }
    }
}