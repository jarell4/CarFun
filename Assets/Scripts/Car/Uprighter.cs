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
    private float lastCastedUprightHeight;

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

        if(transform.localPosition.y != goalUprightHeight)
        {
            return false;
        }

        return true;
	}

    private void MakeUpright()
    {
        Vector3 newForward = Vector3.Cross(transform.right, goalUprightDirection);
        Quaternion goalRotation = Quaternion.FromToRotation(Vector3.forward, newForward);

        //float heightAdjust = lastCastedUprightHeight - goalUprightHeight;
        //Vector3 translation = transform.up * heightAdjust;

        //transform.localPosition += translation;

        float goalHeight = 0f;

        StartCoroutine(LerpToGoalRotation(goalRotation, goalHeight));
    }

	private IEnumerator LerpToGoalRotation(Quaternion goalRotation, float goalHeight)
	{
        Quaternion startRotation = transform.rotation;
        float startHeight = transform.localPosition.y;

        float timer = 0;
		while(timer < checkFrequency)
        {
            float step = timer / checkFrequency;

            Quaternion newRotation = Quaternion.Lerp(startRotation, goalRotation, step);
            transform.rotation = newRotation;

            //Vector3 startPosition = new Vector3(transform.localPosition.x, startHeight, transform.localPosition.z);
            //Vector3 goalPosition = new Vector3(transform.localPosition.x, goalHeight, transform.localPosition.z);

            //Vector3 newPosition = Vector3.Lerp(startPosition, goalPosition, step);
            //transform.localPosition = newPosition;

            timer += Time.deltaTime;
            yield return null;
        }
	}

	private void RecalculateUprightGoals()
    {
        List<Vector3> raycastedDirections = new List<Vector3>();
        float sumHeights = 0f;

        for (int i = 0; i < uprightingCasters.childCount; i++)
        {
            Transform currentCaster = uprightingCasters.GetChild(i);

            RaycastHit hit = DoRaycast(currentCaster);
            if (IsInvalidRaycastHit(hit))
            {
                continue;
            }

            raycastedDirections.Add(hit.normal);

            if (i == 0)
            {
                Vector3 rayOriginPointInMyLocalSpace = transform.InverseTransformPoint(currentCaster.position);
                Vector3 rayImpactPointInMyLocalSpace = transform.InverseTransformPoint(hit.point);

                float rayDistanceInMyLocalSpace = Vector3.Distance(rayOriginPointInMyLocalSpace, rayImpactPointInMyLocalSpace);

                lastCastedUprightHeight = rayDistanceInMyLocalSpace;
            }
        }

        goalUprightDirection = averageRaycastedNormals(raycastedDirections);

        Debug.Log("goalUprightDirection is :" + goalUprightDirection.ToString());

        //lastCastedUprightHeight = sumHeights / uprightingCasters.childCount;
    }

    private Vector3 averageRaycastedNormals(List<Vector3> raycastedDirections)
    {
        Vector3 sumDirection = Vector3.zero;
        foreach (Vector3 vector in raycastedDirections)
        {
            sumDirection += vector;
        }

        Vector3 resultantDirection = sumDirection / raycastedDirections.Count;

        return resultantDirection;
    }

    private bool IsInvalidRaycastHit( RaycastHit hit)
    {
        if(hit.collider == null)
        {
            Debug.Log("Found invalid raycast");
            return true;
        }

        return false;
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
}
