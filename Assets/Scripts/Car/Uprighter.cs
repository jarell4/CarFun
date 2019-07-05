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
		while(true)
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
        return transform.up == uprightDirection;
	}

	private void MakeUpright()
	{
		transform.rotation = Quaternion.FromToRotation(Vector3.up, uprightDirection);
	}

	private void RecalculateUprightDirection()
	{
		RaycastHit hit;
		if(Physics.Raycast(transform.position, -transform.up, out hit))
		{
			Debug.DrawLine(transform.position, hit.point, Color.cyan, checkFrequency);

			if(hit.collider is MeshCollider)
			{
				GameObject hitObject = hit.collider.gameObject;
				Vector3 normalDirection = GetNormalFromRaycastedTriangle(hitObject, hit.triangleIndex);
				uprightDirection = normalDirection;
				return;
			}
		}

        Debug.Log("we didn't hit a a mesh to align to");
		uprightDirection = Vector3.up;
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
