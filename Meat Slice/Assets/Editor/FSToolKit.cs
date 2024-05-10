using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;


public class DeleteSave : MonoBehaviour
{
    [MenuItem("Save Data/Delete All Data")]
    public static void DeleteSaveData()
    {
        PlayerPrefs.DeleteAll();
    }

    [MenuItem("Save Data/Save")]
    public static void SaveData()
    {
        PlayerPrefs.Save();
    }

}

public class TransformObjects : EditorWindow
{
    string baseName = "Base Name";
    bool indexName = false;
    Vector3 objectPosition = new Vector3(0f, 0f, 0f);
    Vector3 objectRotation = new Vector3(0f, 0f, 0f);
    Vector3 objectScale = new Vector3(1f, 1f, 1f);

    List<GameObject> selectedObjects = new List<GameObject>();
    List<List<GameObjectInfo>> undoObjectInfo = new List<List<GameObjectInfo>>();

    int undoLimit = 30;

    [MenuItem("Object Controls/Transform Selected Objects")]
    public static void ShowWindow()
    {
        GetWindow<TransformObjects>("Transform Selected Objects");
    }

    private void OnGUI()
    {
        //Title of Window.
        GUILayout.Label("Transform Selected Objects", EditorStyles.boldLabel); //EditorStyles. largeLabel boldLabel
        GUILayout.Label("Select Objects and Apply Parameters", EditorStyles.miniBoldLabel); //EditorStyles. largeLabel boldLabel

        //Space between title and options.
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //Index the name.
        baseName = EditorGUILayout.TextField("Base Name", baseName);
        //Index the name.
        indexName = EditorGUILayout.Toggle("Index Name", indexName);

        EditorGUILayout.Space();

        //Position.
        objectPosition = EditorGUILayout.Vector3Field("Position", objectPosition);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //Rotation.
        objectRotation = EditorGUILayout.Vector3Field("Rotation", objectRotation);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //Scale.
        objectScale = EditorGUILayout.Vector3Field("Scale", objectScale);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //Buttons:
        //Rename.
        if (GUILayout.Button("Rename!"))
        {
            UpdateUndo();
            RenameObjects();
        }

        EditorGUILayout.Space();

        //World Position.
        if (GUILayout.Button("World Position!"))
        {
            UpdateUndo();
            WorldPositionObjects();
        }

        EditorGUILayout.Space();

        //Local Position.
        if (GUILayout.Button("Local Position!"))
        {
            UpdateUndo();
            LocalPositionObjects();
        }

        EditorGUILayout.Space();

        //Scale.
        if (GUILayout.Button("Rotate!"))
        {
            UpdateUndo();
            RotateObjects();
        }

        EditorGUILayout.Space();

        //Scale.
        if (GUILayout.Button("Scale!"))
        {
            UpdateUndo();
            ScaleObjects();
        }

        EditorGUILayout.Space();

        //Matrix.
        if (GUILayout.Button("Matrix!"))
        {
            UpdateUndo();
            Matrix();
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();


        //Index the name.
        undoLimit = EditorGUILayout.IntField("Undo Limit", undoLimit);

        EditorGUILayout.Space();

        //Undo.
        if (GUILayout.Button("Undo!"))
        {
            UndoChanges();
        }

        EditorGUILayout.Space();

        //Delete.
        if (GUILayout.Button("Delete!"))
        {
            DeleteObjects();
        }

        //Space between title and options.
        EditorGUILayout.Space();

        //Currently Selected Objects.
        GUILayout.Label("Selected Objects", EditorStyles.boldLabel);

        // Access currently selected objects in the Hierarchy window.
        selectedObjects = Selection.gameObjects.ToList();

        //Selection.
        foreach (GameObject selectedObject in selectedObjects)
        {
            GUILayout.Label(selectedObject.name);
        }
    }

    private void UndoChanges()
    {
        //Check if there is data to go back to.
        if (undoObjectInfo.Count > 0)
        {

            //get the last selected objects then select them in the editor.
            selectedObjects = undoObjectInfo[undoObjectInfo.Count - 1][0].previouslySelectedObjects;
            Selection.objects = selectedObjects.ToArray();

            //Reassign the data saved name, ect.
            for (int i = 0; i < selectedObjects.Count; i++)
            {
                selectedObjects[i].gameObject.name = undoObjectInfo[undoObjectInfo.Count - 1][i].name;
                selectedObjects[i].gameObject.transform.localPosition = undoObjectInfo[undoObjectInfo.Count - 1][i].localPosition;
                selectedObjects[i].gameObject.transform.position = undoObjectInfo[undoObjectInfo.Count - 1][i].position;
                selectedObjects[i].gameObject.transform.localScale = undoObjectInfo[undoObjectInfo.Count - 1][i].localScale;
                selectedObjects[i].gameObject.transform.rotation = undoObjectInfo[undoObjectInfo.Count - 1][i].rotation;
            }

            undoObjectInfo.RemoveAt(undoObjectInfo.Count - 1);
        }
    }

    //Saves a new update state for later use.
    private void UpdateUndo()
    {
        //If the undo limit is reached then delete the oldest undo record.
        if (undoObjectInfo.Count > undoLimit)
        {
            undoObjectInfo.RemoveAt(0);
        }

        //Add a new empty list of game information.
        undoObjectInfo.Add(new List<GameObjectInfo>());

        //For each selected object add a new state, and fill the class infomation for each.
        for (int i = 0; i < selectedObjects.Count; i++)
        {
            //fill the list with the number of selected objects.
            undoObjectInfo[undoObjectInfo.Count - 1].Add(new GameObjectInfo());

            //Add a name, localPosition, position, scale, rotation, and what is currenly selected.
            undoObjectInfo[undoObjectInfo.Count - 1][i].name = selectedObjects[i].name;
            undoObjectInfo[undoObjectInfo.Count - 1][i].localPosition = selectedObjects[i].transform.localPosition;
            undoObjectInfo[undoObjectInfo.Count - 1][i].position = selectedObjects[i].transform.position;
            undoObjectInfo[undoObjectInfo.Count - 1][i].localScale = selectedObjects[i].transform.localScale;
            undoObjectInfo[undoObjectInfo.Count - 1][i].rotation = selectedObjects[i].transform.rotation;
            undoObjectInfo[undoObjectInfo.Count - 1][i].previouslySelectedObjects = selectedObjects;
        }
    }

    private void Matrix()
    {
        int xSize = 0;
        int ySize = 0;

        //Get square grid size based on gridSize.
        for (int possibleGridSize = 1; possibleGridSize < Mathf.Infinity; possibleGridSize++)
        {
            int newGridSize = possibleGridSize * possibleGridSize;

            if (newGridSize >= selectedObjects.Count)
            {
                xSize = possibleGridSize;
                ySize = possibleGridSize;

                break;
            }
        }

        int currentIndex = 0;

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++, currentIndex++)
            {
                selectedObjects[currentIndex].transform.position = new Vector3(x, 0f, y);
            }
        }
    }

    private void RenameObjects()
    {

        for (int i = 0; i < selectedObjects.Count; i++)
        {
            if (indexName)
            {
                selectedObjects[i].name = $"{baseName} {i + 1}";
            }
            else
            {
                selectedObjects[i].name = $"{baseName}";
            }

        }
    }

    //Position Selected objects to World.
    private void WorldPositionObjects()
    {
        foreach (GameObject obj in selectedObjects)
        {
            obj.transform.position = objectPosition;
        }
    }

    //Position Selected objects to local.
    private void LocalPositionObjects()
    {
        foreach (GameObject obj in selectedObjects)
        {
            obj.transform.localPosition = objectPosition;
        }
    }

    //Scale Selected objects.
    private void ScaleObjects()
    {
        foreach (GameObject obj in selectedObjects)
        {
            obj.transform.localScale = objectScale;
        }
    }

    //Scale Selected objects.
    private void RotateObjects()
    {
        foreach (GameObject obj in selectedObjects)
        {
            Quaternion rot = Quaternion.Euler(objectRotation);
            obj.transform.rotation = rot;
        }
    }

    //Delete Selected objects.
    private void DeleteObjects()
    {
        //Destory objects in the scene.
        foreach (GameObject obj in selectedObjects)
        {
            DestroyImmediate(obj);
        }

        //Remove the previous object information.
        undoObjectInfo.Clear();
    }

    [System.Serializable]
    public class GameObjectInfo
    {
        public string name; // Unique identifier for the GameObject
        public GameObject gameObject; // Reference to the GameObject in the scene
        public Vector3 localPosition;
        public Vector3 position;
        public Vector3 localScale;
        public Quaternion rotation;
        public List<GameObject> previouslySelectedObjects;
    }
}



public class GridObjects : EditorWindow
{
    string objectBaseName = "Base Name";
    Vector3 objectScale = new Vector3(1f, 1f, 1f);
    int gridSize = 0;
    int lastCollectionSize = 0;
    List<GameObject> objectsToGrid;
    List<GameObject> objectsToClear;
    GameObject objectToSpawn;
    float yAxis = 0f;

    //Next Tool will be one that sets the localposition to 000 scale to 111 and rotation to 000
    [MenuItem("Object Controls/Grid Objects")]
    public static void ShowWindow()
    {
        GetWindow(typeof(GridObjects));
    }

    private void OnGUI()
    {
        //Title of Window.
        GUILayout.Label("Create Grid of Objects", EditorStyles.boldLabel); //EditorStyles. largeLabel boldLabel
        GUILayout.Label("Set a Grid Size.", EditorStyles.miniBoldLabel); //EditorStyles. largeLabel boldLabel
        GUILayout.Label("Drag Objects into wanted sections to be applied, if object in list is not filled an empty will be placed in its place.", EditorStyles.miniBoldLabel); //EditorStyles. largeLabel boldLabel
        GUILayout.Label("Setup Parameters to liking.", EditorStyles.miniBoldLabel); //EditorStyles. largeLabel boldLabel

        //Space between title and options.
        EditorGUILayout.Space();

        //Select a base name for the objects.
        objectBaseName = EditorGUILayout.TextField("Base Name", objectBaseName);
        //Scale of each object.
        objectScale = EditorGUILayout.Vector3Field("Object Scale", objectScale);

        EditorGUILayout.Space();

        //Scale of each object.
        yAxis = EditorGUILayout.FloatField("Height of Grid", yAxis);

        EditorGUILayout.Space();

        //Line between spaces.
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        GUILayout.Label("Singular Object", EditorStyles.boldLabel); //EditorStyles. largeLabel boldLabel
        GUILayout.Label("Input into Singular Object field for just one object to fill the grid.", EditorStyles.miniBoldLabel); //EditorStyles. largeLabel boldLabel

        objectToSpawn = EditorGUILayout.ObjectField($"Singular Object", objectToSpawn, typeof(GameObject), true) as GameObject;


        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();

        //How many objects do you want to add to your array?
        gridSize = EditorGUILayout.IntField("Size of Grid", gridSize);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //Buttons for:
        //Creating grid.
        if (GUILayout.Button("Create Grid!"))
        {
            CreateGrid();
        }
        //Assign grid to currently spawned objects.
        if (GUILayout.Button("Assign Current Grid!"))
        {
            AssignCurrentGrid();
        }
        //Delete All currently spawned grid objects.
        if (GUILayout.Button("Delete Grid!"))
        {
            DeleteGrid();
        }

        EditorGUILayout.Space();

        //To stop overflow error, only create the array when number of gridSize changes.
        if (gridSize != lastCollectionSize)
        {
            //Set last size to current size.
            lastCollectionSize = gridSize;
        }

        //For size of gridSize create a new object.
        for (int objectID = 0; objectID < gridSize; objectID++)
        {
            objectsToGrid[objectID] = EditorGUILayout.ObjectField($"{objectBaseName} {objectID + 1}", objectsToGrid[objectID], typeof(GameObject), true) as GameObject;
        }


    }

    //Deletes all objects spawned using the grid
    private void DeleteGrid()
    {
        foreach (GameObject obj in objectsToClear)
        {
            DestroyImmediate(obj);
        }
    }

    //You cannot hold onto the objects like a prefab, however you can select each one individually for debugging purposes.
    private void AssignCurrentGrid()
    {
        for (int i = 0; i < objectsToGrid.Count; i++)
        {
            objectsToGrid[i] = objectsToClear[i];
        }
    }

    private void CreateGrid()
    {

        int xSize = 0;
        int ySize = 0;

        //Get square grid size based on gridSize.
        for (int possibleGridSize = 1; possibleGridSize < Mathf.Infinity; possibleGridSize++)
        {
            int newGridSize = possibleGridSize * possibleGridSize;

            if (newGridSize >= gridSize)
            {
                xSize = possibleGridSize;
                ySize = possibleGridSize;

                gridSize = newGridSize;
                break;
            }
        }

        InstantiateGrid();

        //Creates the grid.
        void InstantiateGrid()
        {
            //Get reference to current grid number for spawning.
            int currentGridNumber = 0;

            //Create a square grid, x and y in for loops.
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    if (currentGridNumber < (ySize * xSize))
                    {
                        GameObject obj = null;

                        if (objectsToGrid.Count - 1 >= currentGridNumber && objectsToGrid[currentGridNumber] != null)
                        {
                            obj = objectsToGrid[currentGridNumber];
                        }

                        if (objectToSpawn != null && objectsToGrid[currentGridNumber] == null)
                        {
                            obj = objectToSpawn;
                        }

                        InstantiateObject(obj);

                        currentGridNumber++;

                    }
                    else
                    {
                        Debug.Log("Reached the end of Grid Creation.");
                    }

                    //Spawn new Object with required parameters.
                    void InstantiateObject(GameObject obj)
                    {
                        //Instantiate Object.
                        GameObject newGridObj;
                        if (obj == null)
                        {
                            //Do not Instantiate new GameObject, it will create two objects.
                            newGridObj = new GameObject();

                        }
                        else
                        {
                            newGridObj = Instantiate(obj);
                        }

                        //Assign scale.
                        newGridObj.transform.localScale = objectScale;

                        //Assign position and name.
                        newGridObj.transform.localPosition = new Vector3(x, yAxis, y);

                        //Name, current spawned object, and grid point.
                        newGridObj.name = $"{objectBaseName} ({currentGridNumber + 1}) [{x},{y}]";

                        //Check if there are more objects to be spawned than array currently has.
                        if (objectsToGrid.Count - 1 >= currentGridNumber)
                        {
                            //Assign clearable objects to new array allow to be removed after.
                            objectsToClear[currentGridNumber] = newGridObj;
                        }
                        else
                        {
                            objectsToGrid = ConvertList(objectsToGrid);
                            objectsToClear = ConvertList(objectsToClear);
                        }

                    }
                }
            }
        }

        List<GameObject> ConvertList(List<GameObject> list)
        {
            // Old List = current objectsToClear.
            List<GameObject> oldList = list;

            // New List is set to the correct new size.
            List<GameObject> newList = new List<GameObject>(gridSize);

            // Assign old list to new list.
            newList.AddRange(oldList);

            return newList;
        }

    }
}