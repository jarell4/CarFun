using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Uprighter : MonoBehaviour 
{
    [SerializeField]
	private float checkFrequency = 0.5f;

    [SerializeField]
    private Transform uprightingCasters;

    [SerializeField]
    private float goalUprightHeight = 1f;

    private Vector3 goalUprightDirection;
    private float adjustAmountForGoalHeight;

	void Start ()
	{
		StartCoroutine(RegularlyCheckUpright());
	}

	private IEnumerator RegularlyCheckUpright()
	{
		while(true)
		{
			RecalculateUprightGoals();

			if(!IsUpright())
			{
                MakeUpright();
			}
			
			yield return new WaitForSeconds(checkFrequency);
		}
	}

	private bool IsUpright()
	{
        if(transform.up != goalUprightDirection)
        {
            return false;
        }

        if(Mathf.Abs(adjustAmountForGoalHeight) > 0.5)
        {
            return false;
        }

        return true;
	}

    private void MakeUpright()
    {
        if (goalUprightDirection != Vector3.zero)
        {
            Vector3 newForward = Vector3.Cross(transform.right, goalUprightDirection);
            Debug.DrawRay(transform.localPosition, newForward, Color.blue, checkFrequency);

            Quaternion goalRotation = Quaternion.FromToRotation(Vector3.forward, newForward);
            StartCoroutine(LerpToGoalRotation(goalRotation));
        }

        if (float.IsNaN(adjustAmountForGoalHeight) == false)
        {
            float goalHeight = transform.position.y + adjustAmountForGoalHeight;
            StartCoroutine(LerpToGoalHeight(goalHeight));
        }
    }

	private IEnumerator LerpToGoalHeight(float goalHeight)
	{
        float startHeight = transform.localPosition.y;

        float timer = 0;
		while(timer < checkFrequency)
        {
            float step = timer / checkFrequency;

            Vector3 startPosition = new Vector3(transform.localPosition.x, startHeight, transform.localPosition.z);
            Vector3 goalPosition = new Vector3(transform.localPosition.x, goalHeight, transform.localPosition.z);

            Vector3 newPosition = Vector3.Lerp(startPosition, goalPosition, step);
            transform.localPosition = newPosition;

            timer += Time.deltaTime;
            yield return null;
        }
	}

    private IEnumerator LerpToGoalRotation(Quaternion goalRotation)
    {
        Quaternion startRotation = transform.rotation;

        float timer = 0;
        while (timer < checkFrequency)
        {
            float step = timer / checkFrequency;

            Quaternion newRotation = Quaternion.Lerp(startRotation, goalRotation, step);
            transform.rotation = newRotation;

            timer += Time.deltaTime;
            yield return null;
        }
    }

	private void RecalculateUprightGoals()
    {
        List<Vector3> raycastedDirections = new List<Vector3>();
        List<float> raycastedHeights = new List<float>();

        for (int i = 0; i < uprightingCasters.childCount; i++)
        {
            Transform currentCaster = uprightingCasters.GetChild(i);

            RaycastHit hit = DoRaycast(currentCaster);
            if (IsInvalidRaycastHit(hit))
            {continue;}

            IncludeRaycastInDirectionsCalculation(raycastedDirections, hit);
            IncludeRaycastInHeightsCalculation(raycastedHeights, hit);
        }

        goalUprightDirection = ResolveGoalDirection(raycastedDirections);
        Debug.Log("goalUprightDirection is :" + goalUprightDirection.ToString());

        adjustAmountForGoalHeight = ResolveAdjustAmountForGoalHeight(raycastedHeights);
        Debug.Log("adjustAmountForGoalHeight is :" + adjustAmountForGoalHeight.ToString());
    }

    private RaycastHit DoRaycast(Transform casterTransform)
    {
        RaycastHit hit;

        if (Physics.Raycast(casterTransform.position, casterTransform.forward, out hit))
        {
            Debug.DrawLine(casterTransform.position, hit.point, Color.cyan, checkFrequency);
        }
        else
        {
            Debug.Log(this.name + ": " + "Didn't hit a a mesh to align to");
        }

        return hit;
    }

    private bool IsInvalidRaycastHit(RaycastHit hit)
    {
        if (hit.collider == null)
        {
            Debug.Log("Found invalid raycast");
            return true;
        }

        if(hit.distance > goalUprightHeight * 2)
        {
            Debug.Log("raycast hit too far");
            return true;
        }

        return false;
    }

    private void IncludeRaycastInDirectionsCalculation(List<Vector3> directionsList, RaycastHit hit)
    {
        directionsList.Add(hit.normal);

        Debug.DrawRay(hit.point, hit.normal, Color.yellow, checkFrequency);
    }

    private void IncludeRaycastInHeightsCalculation(List<float> heightsList, RaycastHit hit)
    {
        heightsList.Add(hit.distance);
    }

    private Vector3 ResolveGoalDirection(List<Vector3> raycastedDirections)
    {
        Vector3 sumDirection = Vector3.zero;
        foreach (Vector3 vector in raycastedDirections)
        {
            sumDirection += vector;
        }

        Vector3 resultantDirection = sumDirection / raycastedDirections.Count;

        return resultantDirection.normalized;
    }

    private float ResolveAdjustAmountForGoalHeight(List<float> raycastedHeights)
    {
        float sumDistances = 0f;

        foreach(float distance in raycastedHeights)
        {
            sumDistances += distance;
        }

        Debug.Log("sumDistances: " + sumDistances.ToString());
        Debug.Log("raycastedHeights.Count: " + raycastedHeights.Count.ToString());

        float averageHeight = sumDistances / raycastedHeights.Count;
        return goalUprightHeight - averageHeight;
    }
}
