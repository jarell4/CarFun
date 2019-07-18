using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Uprighter : MonoBehaviour 
{
    [SerializeField]
	private float checkFrequency = 0.5f;

    [SerializeField]
    private Transform uprightingCasters;

	private Vector3 goalUprightDirection;

    private float goalUprightHeight;
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

        return true;
	}

    private void MakeUpright()
    {
        Vector3 newForward = Vector3.Cross(transform.right, goalUprightDirection);
        Quaternion goalRotation = Quaternion.FromToRotation(Vector3.forward, newForward);

        StartCoroutine(LerpToGoalRotation(goalRotation));
    }

	private IEnumerator LerpToGoalRotation(Quaternion goalRotation)
	{
        Quaternion startRotation = transform.rotation;

        float timer = 0;
		while(timer < checkFrequency)
        {
            float step = timer / checkFrequency;
            //step *= 0.5f;

            Quaternion newRotation = Quaternion.Lerp(startRotation, goalRotation, step);
            transform.rotation = newRotation;

            timer += Time.deltaTime;
            yield return null;
        }
	}

	private void RecalculateUprightGoals()
	{
        Vector3[] raycastedDirections = new Vector3[uprightingCasters.childCount];

        for (int i = 0; i < raycastedDirections.Length; i++)
        {
            RaycastHit hit = DoRaycastForCalculatingUprightDirection(
                uprightingCasters.GetChild(i));

            Vector3 newDirection = hit.normal;
            raycastedDirections[i] = newDirection;
        }

        Vector3 sumDirection = Vector3.zero;
        foreach (Vector3 vector in raycastedDirections)
        {
            sumDirection += vector;
        }
        goalUprightDirection = sumDirection.normalized;
	}

    private RaycastHit DoRaycastForCalculatingUprightDirection(Transform casterTransform)
    {
        RaycastHit hit;
        if (Physics.Raycast(casterTransform.position, casterTransform.forward, out hit))
        {
            Debug.DrawLine(casterTransform.position, hit.point, Color.cyan, checkFrequency);
            return hit;
        }
        else
        {
            Debug.Log("we didn't hit a a mesh to align to");
            return hit;
        }
    }
}
