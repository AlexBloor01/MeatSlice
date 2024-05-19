using UnityEngine;
using System;

public class FoodVisual : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer; //Reference to this objects skinnedMeshRenderer.

    //Due to a bug with Unitys submesh index ordering system, the materials must be stored and sorted locally.
    //Materials must be ordered alphabetically by name as such: 0 X+, 1 Z-, 2 X-, 3 Z+, 4 Base.
    private Material[] foodMaterials; //Materials that are to be sorted.
    public Texture[] sliceTextures; //Textures for the side of the food object.
    public Texture[] baseTextures; //Textures that fill the top and bottom of the food object.

    //Directions in order, North, East, South, West or X-, Z+, X+, Z-.
    readonly Vector3[] directions = new Vector3[] {
    new (-1, 0, 0), //X-
    new (0, 0, 1), //Z+
    new(1, 0, 0), //X+
    new (0, 0, -1), //Z-
    };

    const string textureMap = "_MainTex";

    private void Awake()
    {
        SetupVariables();
    }

    void SetupVariables()
    {
        if (skinnedMeshRenderer == null)
        {
            skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        }

        if (foodMaterials == null && skinnedMeshRenderer != null)
        {
            //Convert meshrenderes materials to array and sort alphabetically.
            foodMaterials = skinnedMeshRenderer.materials;
            Array.Sort(foodMaterials, (mat1, mat2) => string.Compare(mat1.name, mat2.name));
        }

        // if (sideTextures == null)
        // {
        //     Debug.LogError($"Side Textures on {gameObject.name} Missing, please correct.");
        // }

        if (baseTextures.Length > 1)
        {
            Material material = foodMaterials[foodMaterials.Length - 1];
            material.SetTexture(textureMap, sliceTextures[UnityEngine.Random.Range(0, baseTextures.Length)]);
        }


        if (skinnedMeshRenderer == null)
        {
            skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        }
    }

    //Applies a new Texture and sets the blend shape weight based on the position of slicePosition.
    public void SliceVisual(Vector3 slicePosition, Vector3 sliceScale, bool debug)
    {
        int materialIndex = MaterialIndexByDistance(slicePosition, sliceScale);

        Material material = foodMaterials[materialIndex];
        if (sliceTextures.Length > 0)
        {
            material.SetTexture(textureMap, sliceTextures[UnityEngine.Random.Range(0, sliceTextures.Length)]);
        }

        //Set BlendShapeWeight if available.
        if (skinnedMeshRenderer != null)
        {
            skinnedMeshRenderer.SetBlendShapeWeight(materialIndex, 100);
        }

        //This script is no longer needed after slicing and would otherwise take up ram unnecessarily.
        Destroy(GetComponent<FoodVisual>());
    }

    //Gets the distance of the sliceposition to compared to the lowest distance in local space.
    //After getting the lowest distance this can find which side of the index is.
    int MaterialIndexByDistance(Vector3 slicePosition, Vector3 sliceScale)
    {
        //Get the distances between the position of slice and localPosition + positions.
        int materialIndex = 0;
        float[] distances = new float[directions.Length];
        for (int i = 0; i < directions.Length; i++)
        {
            Vector3 currentPosition = transform.position + directions[i];
            distances[i] = Vector3.Distance(slicePosition, currentPosition);

            //Then gets the lowest distance out of the 4 to get the smallest distance by index.
            if (distances[i] < distances[materialIndex])
            {
                materialIndex = i;
            }
        }
        //returns the index that is closest to the slice position.
        return materialIndex;
    }

}


