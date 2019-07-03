using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Uprighter : MonoBehaviour 
{
	public float checkFrequency = 0.5f;

	private Vector3 uprightDirection;

	void Start ()
	{
		StartCoroutine(RegularlyCheckUpright());
	}

	private IEnumerator RegularlyCheckUpright()
	{
		for(;;)
		{
			RecalculateUprightDirection();

			if(IsUpright())
			{
				Debug.Log("I'm upright.");
			}
			else
			{
				Debug.Log("I'm not upright");
				MakeUpright();
			}

			
			yield return new WaitForSeconds(checkFrequency);
		}
	}

	private bool IsUpright()
	{
		return this.transform.localRotation.eulerAngles == uprightDirection;
	}

	private void MakeUpright()
	{
		this.transform.localRotation = Quaternion.Euler(uprightDirection);
	}

	private void RecalculateUprightDirection()
	{
		RaycastHit hit;
		if(Physics.Raycast(transform.position, -transform.up, out hit))
		{
			Debug.DrawLine(transform.position, hit.point, Color.cyan);

			if(hit.collider is MeshCollider)
			{
				GameObject hitObject = hit.collider.gameObject;
				Vector3 normalDirection = GetNormalFromRaycastedTriangle(hitObject, hit.triangleIndex);
				uprightDirection = normalDirection;
				return;
			}
		}

		//uprightDirection = Vector3.up;
	}

	private Vector3 GetNormalFromRaycastedTriangle(GameObject hitObject, int triangleIndex)
	{
		Mesh hitMesh = hitObject.GetComponent<MeshFilter>().mesh;

		int[] triangles = hitMesh.triangles;

		int vertex1 = triangles[triangleIndex * 3];
		int vertex2 = triangles[triangleIndex * 3 + 1];
		int vertex3 = triangles[triangleIndex * 3 + 2];

		Vector3[] vertexNormals = hitMesh.normals;

		Vector3 normal1 = vertexNormals[vertex1];
		Vector3 normal2 = vertexNormals[vertex2];
		Vector3 normal3 = vertexNormals[vertex3];

		Vector3 faceNormal = (normal1 + normal2 + normal3)/3;

		return faceNormal;
	}
}
