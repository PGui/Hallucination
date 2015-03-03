using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour {
   
    public static ObjectPool instance;
    public int matrixMinX=0, matrixMinY=0, matrixMaxX=0, matrixMaxY=1;

    public struct GridElement
    {
        public int m_x { get; set; }
        public int m_y { get; set; }
        public int m_type { get; set; }

        public string mToString()
        {
            return "GridElement :" + m_x + ":" + m_y + " of type : " + m_type;
        }
    }
    public List<GridElement> _matrix = new List<GridElement>()
    {
        {new GridElement{m_x = 0, m_y = 0, m_type = 1}},
        {new GridElement{m_x = 0, m_y = 1, m_type = 10}}
    };
    /*public Dictionary<int, GridElement> _matrix = new Dictionary<int, GridElement>() {
        {0, new GridElement{m_x = 0, m_y = 0, m_type = 1}},
        {1, new GridElement{m_x = 0, m_y = 1, m_type = 2}}
    };*/

    /// <summary>
    /// The object prefabs which the pool can handle.
    /// </summary>
    public GameObject[] objectPrefabs;
   
    /// <summary>
    /// The pooled objects currently available.
    /// </summary>
    public List<GameObject>[] pooledObjects;
   
    /// <summary>
    /// The amount of objects of each type to buffer.
    /// </summary>
    public int[] amountToBuffer;
   
    public int defaultBufferAmount = 3;
   
    /// <summary>
    /// The container object that we will keep unused pooled objects so we dont clog up the editor with objects.
    /// </summary>
    protected GameObject containerObject;
   
    void Awake ()
    {
        instance = this;
    }
   
    // Use this for initialization
    void Start ()
    {
        containerObject = new GameObject("ObjectPool");
       
        //Loop through the object prefabs and make a new list for each one.
        //We do this because the pool can only support prefabs set to it in the editor,
        //so we can assume the lists of pooled objects are in the same order as object prefabs in the array
        pooledObjects = new List<GameObject>[objectPrefabs.Length];
       
        for(int i = 0; i < objectPrefabs.Length; i++){
            pooledObjects[i] = new List<GameObject>(); 
           
            int bufferAmount;
           
            if(i < amountToBuffer.Length) bufferAmount = amountToBuffer[i];
            else
                bufferAmount = defaultBufferAmount;
           
            for ( int n=0; n<bufferAmount; n++)
            {
                GameObject newObj = Instantiate(objectPrefabs[i]) as GameObject;
                newObj.name = objectPrefabs[i].name;
                PoolObject(newObj);
            }
           
        }
    }
   
    /// <summary>
    /// Gets a new object for the name type provided.  If no object type exists or if onlypooled is true and there is no objects of that type in the pool
    /// then null will be returned.
    /// </summary>
    /// <returns>
    /// The object for type.
    /// </returns>
    /// <param name='objectType'>
    /// Object type.
    /// </param>
    /// <param name='onlyPooled'>
    /// If true, it will only return an object if there is one currently pooled.
    /// </param>
    public GameObject GetObjectForType ( string objectType , bool onlyPooled )
    {
        for(int i=0; i<objectPrefabs.Length; i++)
        {
            if(objectPrefabs[i].name == objectType)
            {
                int bufferAmount;

                if (i < amountToBuffer.Length) bufferAmount = amountToBuffer[i];
                else
                    bufferAmount = defaultBufferAmount;

                for (int j = 0; j < bufferAmount; j++)
                {
                   // if(!pooledObjects[i][j].activeInHierarchy) {
                   // if (Mathf.RoundToInt(pooledObjects[i][j].transform.position.x) == 0 && Mathf.RoundToInt(pooledObjects[i][j].transform.position.z) == 0)
                    if (pooledObjects[i][j].GetComponent<Corridor>().m_available)
                    {//.activeInHierarchy) {
                        return pooledObjects[i][j];
                    }
                }
                if(!onlyPooled) {
                    GameObject newObj = Instantiate(objectPrefabs[i]) as GameObject;
                    newObj.name = objectPrefabs[i].name;
                    PoolObject(newObj);
                    return newObj;
                }
                /*if(pooledObjects[i].Count > 0)
                {
                    GameObject pooledObject = pooledObjects[i][0];
                    pooledObjects[i].RemoveAt(0);
                    pooledObject.transform.parent = null;
                   
                    return pooledObject;
                   
                } else if(!onlyPooled) {
                    return Instantiate(objectPrefabs[i]) as GameObject;
                }*/
               
                break;
               
            }
        }
           
        //If we have gotten here either there was no object of the specified type or non were left in the pool with onlyPooled set to true
        return null;
    }
   
    /// <summary>
    /// Pools the object specified.  Will not be pooled if there is no prefab of that type.
    /// </summary>
    /// <param name='obj'>
    /// Object to be pooled.
    /// </param>
    public void PoolObject ( GameObject obj )
    {
        for ( int i=0; i<objectPrefabs.Length; i++)
        {
            if(objectPrefabs[i].name == obj.name)
            {
                //obj.SetActiveRecursively(false);
              //  obj.SetActive(false);
                obj.transform.parent = containerObject.transform;
                pooledObjects[i].Add(obj);
                return;
            }
        }
    }
   
}
