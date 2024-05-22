using UnityEngine;
using System;

public class FoodVisual : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer; //Reference to this objects skinnedMeshRenderer.

    //Due to a bug with Unitys submesh index ordering system, the materials must be stored and sorted locally.
    //Materials must be ordered alphabetically by name as such: 0 X+, 1 Z-, 2 X-, 3 Z+, 4 Base.
    private Material[] foodMaterials; //Materials that are to be sorted.
    public Texture[] sliceTextures; //Do you want Textures to replace the sides of the food when sliced?
    public Texture[] sideTextures; //Side Textures before slicing.
    public Texture[] baseTextures; //Textures that fill the top and bottom of the food object but can fill the sides with baseTextureSides.
    public bool baseTextureSides = false; //If you have baseTextures do you want them assigned to every side at the start?

    //Colours to fill each material, add more to fill more materials.4 set by default for 4 sided cubes.
    public Color[] sideColours = new Color[] {
    Color.white,
    Color.white,
    Color.white,
    Color.white,
    };
    public bool isSidesColoured = false; //Do you want to colour each side?
    public Color afterSliceColour = Color.white; //What colour do you want the slice to be after slicing.


    //Directions in order, North, East, South, West or X-, Z+, X+, Z-.
    readonly Vector3[] directions = new Vector3[] {
    new (-1, 0, 0), //X-
    new (0, 0, 1), //Z+
    new(1, 0, 0), //X+
    new (0, 0, -1), //Z-
    };

    const string textureMap = "_MainTex"; //Name of material texture slot.
    const string colorMap = "_Color"; //Name of color texture slot.


    private void Awake()
    {
        SetupVariables();
    }

    void SetupVariables()
    {
        //Are we using a skinnedMeshRenderer? Are we using blend shapes? If so grab the SkinnedMeshRenderer.
        if (skinnedMeshRenderer == null && GetComponent<SkinnedMeshRenderer>())
        {
            skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        }

        //If the food materials have not already been imported and ordered.
        if (foodMaterials == null)
        {
            //Convert the Meshrenderers materials to array.
            if (skinnedMeshRenderer != null)
            {
                foodMaterials = skinnedMeshRenderer.materials;
            }
            else
            {
                foodMaterials = GetComponent<MeshRenderer>().materials;
            }
            //Sort alphabetically.
            Array.Sort(foodMaterials, (mat1, mat2) => string.Compare(mat1.name, mat2.name));
        }
        //If you have baseTextures imported.
        if (baseTextures.Length > 0)
        {
            Texture chosenBaseTexture = baseTextures[UnityEngine.Random.Range(0, baseTextures.Length)];
            Material baseMaterial = foodMaterials[foodMaterials.Length - 1];
            baseMaterial.SetTexture(textureMap, chosenBaseTexture);

            //Do you want them assigned to every side?
            if (baseTextureSides)
            {
                foreach (Material item in foodMaterials)
                {
                    item.SetTexture(textureMap, chosenBaseTexture);
                }
            }
        }

        if (sideTextures.Length > 0)
        {
            for (int i = 0; i < foodMaterials.Length; i++)
            {
                if (i < foodMaterials.Length - 1)
                {
                    Texture sideTexture = sideTextures[UnityEngine.Random.Range(0, sideTextures.Length)];
                    foodMaterials[i].SetTexture(textureMap, sideTexture);
                }
            }
        }

        if (isSidesColoured)
        {
            for (int i = 0; i < foodMaterials.Length; i++)
            {
                if (sideColours.Length - 1 >= i)
                {
                    foodMaterials[i].SetColor(colorMap, sideColours[i]);
                }
            }
        }
    }

    //Applies a new Texture and sets the blend shape weight based on the position of slicePosition.
    public void SliceVisual(Vector3 slicePosition, float xDiff, float zDiff)
    {
        //get the index based on the position of the slice.
        int materialIndex = MaterialIndexByDistance(slicePosition);

        Material material = foodMaterials[materialIndex];
        material.SetColor(colorMap, afterSliceColour);

        if (sideTextures.Length > 0 && sliceTextures.Length == 0)
        {
            material.SetTexture(textureMap, null);
        }

        if (sliceTextures.Length > 0)
        {
            material.SetTexture(textureMap, sliceTextures[UnityEngine.Random.Range(0, sliceTextures.Length)]);
        }


        //Set BlendShapeWeight if available based on material index and difference.
        if (skinnedMeshRenderer != null)
        {
            skinnedMeshRenderer.SetBlendShapeWeight(materialIndex, Mathf.Abs(xDiff + zDiff));
        }

        //This script is no longer needed after slicing and would otherwise take up ram unnecessarily.
        Destroy(GetComponent<FoodVisual>());
    }

    //This will change the base material which should use WorldSpaceTextureShader to alter the position to appear as if the texture has not moved.
    void AdjustBaseMaterial(float xDiff, float zDiff)
    {
        //Make base material tilling smaller in comparision to the x and z difference.
        Material baseMaterial = foodMaterials[foodMaterials.Length - 1];

        baseMaterial.SetFloat("_LockTexture", 1.0f);
        baseMaterial.SetVector("_TextureOffset", new Vector4(xDiff, zDiff, 0, 0));
        baseMaterial.SetVector("_CurrentScale", new Vector4(transform.parent.localScale.x / GameManager.scaleFactor, transform.parent.localScale.z / GameManager.scaleFactor, 0, 0));
    }

    //Gets the distance of the sliceposition to compared to the lowest distance in local space.
    //After getting the lowest distance this can find which side of the index is.
    int MaterialIndexByDistance(Vector3 slicePosition)
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


