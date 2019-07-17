using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Uprighter : MonoBehaviour 
{
    [SerializeField]
	private float checkFrequency = 0.5f;

    [SerializeField]
    private Transform uprightingCasters;

	private Vector3 uprightDirection;

	void Start ()
	{
		StartCoroutine(RegularlyCheckUpright());
	}

	private IEnumerator RegularlyCheckUpright()
	{
		while(true)
		{
			RecalculateUprightDirection();

			if(!IsUpright())
			{
                MakeUpright();
			}
			
			yield return new WaitForSeconds(checkFrequency);
		}
	}

	private bool IsUpright()
	{
        if(transform.up != uprightDirection)
        {
            return false;
        }

        return true;
	}

    private void MakeUpright()
    {
        Quaternion goalRotation = Quaternion.FromToRotation(Vector3.up, uprightDirection);
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

	private void RecalculateUprightDirection()
	{
        Vector3[] raycastedDirections = new Vector3[uprightingCasters.childCount];

        for (int i = 0; i < raycastedDirections.Length; i++)
        {
            Vector3 newDirection = DoRaycastForCalculatingUprightDirection(
                uprightingCasters.GetChild(i));

            raycastedDirections[i] = newDirection;
        }

        Vector3 sumDirection = Vector3.zero;

        foreach (Vector3 vector in raycastedDirections)
        {
            sumDirection += vector;
        }

        uprightDirection = sumDirection.normalized;
	}

    private Vector3 DoRaycastForCalculatingUprightDirection(Transform casterTransform)
    {
        RaycastHit hit;
        if (Physics.Raycast(casterTransform.position, casterTransform.forward, out hit))
        {
            Debug.DrawLine(casterTransform.position, hit.point, Color.cyan, checkFrequency);

            if (hit.collider is MeshCollider)
            {
                GameObject hitObject = hit.collider.gameObject;
                Vector3 normalDirection = GetNormalFromRaycastedTriangle(hitObject, hit.triangleIndex);
                return normalDirection;
            }
        }

        Debug.Log("we didn't hit a a mesh to align to");
        return Vector3.zero;
    }

	private Vector3 GetNormalFromRaycastedTriangle(GameObject hitObject, int triangleIndex)
    {
        Mesh hitMesh = hitObject.GetComponent<MeshFilter>().mesh;
        int[] vertexIndices = GetTriangleVertexIndicesFromMesh(triangleIndex, hitMesh);

        Vector3[] vertices = hitMesh.vertices;
        Vector3 facePosition = GetFacePositionFromVertices(hitObject, vertices, vertexIndices);

        Vector3[] vertexNormals = GetVertexNormalsFromVertices(hitObject, vertices, vertexIndices);

        Vector3 faceNormal = GetFaceNormalFromVertexNormals(vertexNormals);

        Debug.DrawRay(facePosition, faceNormal, Color.yellow, checkFrequency);

        return faceNormal;
    }

    private int[] GetTriangleVertexIndicesFromMesh(int triangleIndex, Mesh hitMesh)
    {
        int[] triangles = hitMesh.triangles;

        int[] vertexIndices = new int[3];

        vertexIndices[0] = triangles[triangleIndex * 3];
        vertexIndices[1] = triangles[triangleIndex * 3 + 1];
        vertexIndices[2] = triangles[triangleIndex * 3 + 2];

        return vertexIndices;
    }

    private Vector3 GetFacePositionFromVertices(GameObject hitObject, Vector3[] vertices, int[] vertexIndices)
    {
        Vector3 vertex1Position = hitObject.transform.TransformPoint(vertices[vertexIndices[0]]);
        Vector3 vertex2Position = hitObject.transform.TransformPoint(vertices[vertexIndices[1]]);
        Vector3 vertex3Position = hitObject.transform.TransformPoint(vertices[vertexIndices[2]]);

        Vector3 facePosition = (vertex1Position + vertex2Position + vertex3Position) / 3;

        return facePosition;
    }

    private Vector3[] GetVertexNormalsFromVertices(GameObject hitObject, Vector3[] vertices, int[] vertexIndices)
    {
        Vector3[] normals = new Vector3[3];

        normals[0] = hitObject.transform.TransformDirection(vertices[vertexIndices[0]]);
        normals[1] = hitObject.transform.TransformDirection(vertices[vertexIndices[1]]);
        normals[2] = hitObject.transform.TransformDirection(vertices[vertexIndices[2]]);

        return normals;
    }

    private Vector3 GetFaceNormalFromVertexNormals(Vector3[] normals)
    {
        Vector3 faceNormal = Vector3.Cross(normals[1] - normals[0], normals[1] - normals[2]);

        return -faceNormal.normalized;
    }
}
