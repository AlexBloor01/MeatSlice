using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

public class MovementLibrary : MonoBehaviour
{
    /// <summary>
    /// Moves an object using Vector3 to Vector3 on a given Lerp Type.
    /// </summary>
    /// <typeparam name="obj">Object you want to change the position, rotation, or scale of</typeparam>
    /// <typeparam name="duration">Duration of time you want the effect to take place over</typeparam>
    /// <typeparam name="startValue">Start position of the lerp</typeparam>
    /// <typeparam name="endValue">End position of the lerp</typeparam>
    /// <typeparam name="scaledTime">Will this Lerp be effected by Time.Scale or not?</typeparam>
    /// <typeparam name="ObjectLerpType">What type of lerp do you require? World Position, Local Position, Rotation, or Scale</typeparam>
    /// <typeparam name="onComplete" >When the lerp is complete what action do you want to happen after, put null for none</typeparam>
    /// <returns></returns>
    public static IEnumerator LerpOnce(GameObject obj, float duration, Vector3 startValue, Vector3 endValue, bool scaledTime, ObjectLerpType ObjectLerpType, Action action)
    {
        //Check if it is a rotaiton. More efficient to make it its own function
        if (ObjectLerpType == ObjectLerpType.Rotation)
        {
            yield return RotationLerpOnce(obj, duration, startValue, endValue, scaledTime);
            yield break;
        }

        float timer = 0;

        while (timer < duration)
        {
            //Check if it is scaled or unscaledtime.
            //If unscaled the lerp will not require Time.timeScale be higher than 0.
            if (scaledTime)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
            }

            float percentage = timer / duration;

            switch (ObjectLerpType)
            {
                case ObjectLerpType.Position:
                    obj.transform.position = Vector3.Lerp(startValue, endValue, percentage);
                    break;

                case ObjectLerpType.LocalPosition:
                    obj.transform.localPosition = Vector3.Lerp(startValue, endValue, percentage);
                    break;

                case ObjectLerpType.LocalScale:
                    obj.transform.localScale = Vector3.Lerp(startValue, endValue, percentage);
                    break;

                default:
                    Debug.LogError("Invalid Lerp Type Specified!");
                    break;
            }

            yield return null;
        }

        action?.Invoke();
    }

    //Rotate from start Rotation to end Rotation once using Vector3 inputs once.
    private static IEnumerator RotationLerpOnce(GameObject obj, float duration, Vector3 startRotation, Vector3 endRotation, bool scaledTime)
    {
        float timer = 0;
        Quaternion beginRot = Quaternion.Euler(startRotation);
        Quaternion endRot = Quaternion.Euler(endRotation);

        while (timer < duration)
        {
            if (scaledTime)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
            }

            timer += Time.deltaTime;
            float percentage = timer / duration;
            obj.transform.rotation = Quaternion.Lerp(beginRot, endRot, percentage);
            yield return null;
        }
    }

    //List of the different lerp types available while using Vector3.
    public enum ObjectLerpType
    {
        Position,
        LocalPosition,
        LocalScale,
        Rotation
    }

    public static IEnumerator AnimationCurveLerp(GameObject obj, float duration, Vector3 startValue, Vector3 endValue, bool scaledTime, ObjectLerpType ObjectLerpType, AnimationCurve animationCurve, Action action)
    {
        //Check if it is a rotaiton. More efficient to make it its own function
        if (ObjectLerpType == ObjectLerpType.Rotation)
        {
            yield break;
        }

        float timer = 0;

        while (timer < duration)
        {
            //Check if it is scaled or unscaledtime.
            //If unscaled the lerp will not require Time.timeScale to be higher than 0.
            if (scaledTime)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
            }

            float percentage = timer / duration;

            switch (ObjectLerpType)
            {
                case ObjectLerpType.Position:
                    obj.transform.position = Vector3.Lerp(startValue, endValue, animationCurve.Evaluate(percentage));
                    break;

                case ObjectLerpType.LocalPosition:
                    obj.transform.localPosition = Vector3.Lerp(startValue, endValue, animationCurve.Evaluate(percentage));
                    break;

                case ObjectLerpType.LocalScale:
                    obj.transform.localScale = Vector3.Lerp(startValue, endValue, animationCurve.Evaluate(percentage));
                    break;

                default:
                    Debug.LogError("Invalid Lerp Type Specified!");
                    break;
            }
            yield return null;
        }

        action?.Invoke();
    }
}

public class VectorLibrary : MonoBehaviour
{
    //All values in Vector3 are the same random number between min and max.
    public static Vector3 RandomVector3Whole(float min, float max)
    {
        float value = UnityEngine.Random.Range(min, max);
        return new Vector3(value, value, value);
    }

    //Each value in Vector3 is a different random number between min and max.
    public static Vector3 RandomVector3Seperate(float min, float max)
    {
        float value1 = UnityEngine.Random.Range(min, max);
        float value2 = UnityEngine.Random.Range(min, max);
        float value3 = UnityEngine.Random.Range(min, max);
        return new Vector3(value1, value2, value3);
    }


    //Gets the center position of a group of positions.
    public static Vector3 CenterPosition(Vector3[] positions)
    {
        float x = 0, y = 0, z = 0;

        foreach (Vector3 position in positions)
        {
            x += position.x;
            y += position.y;
            z += position.z;
        }

        Vector3 centerPosition = new Vector3(x, y, z);
        centerPosition /= positions.Length;

        return centerPosition;
    }

    //Replace the position of a vector with value.
    public static Vector3 ReplaceVector(Vector3 original, float value, int positionInVector)
    {
        original[positionInVector] = value;
        return original;
    }

    public static Vector3 TimesByVector(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

}

public class Numbers : MonoBehaviour
{
    //Returns the percentage of value entered using percent.
    public static float GetPercent(float value, float percent)
    {
        percent = Mathf.Clamp(percent, 0, 100);
        return (value / 100) * percent;
    }

}

public class Meshes : MonoBehaviour
{

    //Find the index of a submesh when compared to a triangle index by iteratting through all submeshes of a mesh.
    public static int GetSubmeshIndex(Mesh mesh, int triangleIndex)
    {

        //Keep track of the number of triangles in each mesh as we iterate through them.
        //triangleIndex will equal 1 of these indexes when we take the index count and / 3.
        int triangleCounter = 0;

        //Iterate through each submesh found within the mesh to find the correct submesh index.
        for (int subMeshIndex = 0; subMeshIndex < mesh.subMeshCount; subMeshIndex++)
        {
            int indexCount = mesh.GetSubMesh(subMeshIndex).indexCount;
            triangleCounter += indexCount / 3;
            if (triangleIndex < triangleCounter)
            {
                return subMeshIndex;
            }
        }

        Debug.LogError($"Failed to find triangle with index {triangleIndex} in mesh '{mesh.name}'. Total triangle count: {triangleCounter} in {mesh}");
        return 0;
    }

    public static Vector3[] GetSubmeshVertices(Mesh mesh, int submeshIndex)
    {
        Vector3[] vertices = mesh.vertices;
        int[] subMeshTriangles = mesh.GetTriangles(submeshIndex);
        Vector3[] submeshVerts = new Vector3[subMeshTriangles.Length * 3];

        for (int i = 0; i < subMeshTriangles.Length; i += 3)
        {
            submeshVerts[i] = vertices[subMeshTriangles[i]];
            submeshVerts[i + 1] = vertices[subMeshTriangles[i] + 1];
            submeshVerts[i + 2] = vertices[subMeshTriangles[i] + 2];
        }

        return submeshVerts;
    }


}