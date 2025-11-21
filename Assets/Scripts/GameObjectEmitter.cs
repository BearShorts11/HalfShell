using UnityEngine;

// Honestly I was going to use this for the gibs but now I'm not sure. Might be used later? Idk this thing instantiates.
public class GameObjectEmitter : MonoBehaviour
{
    [SerializeField] private float lifeTime = 5.0f;
    [SerializeField] private GameObject emittedObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Start()
    {
        if (emittedObject != null)
            Emitted(emittedObject, lifeTime);
            
        //Destroy(this.gameObject, lifeTime);
    }

    /// <summary>
    /// A method for instantiating gameobject (as particles/emitters) that can be used by animation events
    /// </summary>
    /// <param name="name">(Do not use space in the name) Name of the object prefab in the Resources folder, if called from the Animator use "," and enter any number after the comma to specify life time.</param>
    /// <param name="lifetime">Lifespan of the game object</param>
    protected GameObject EmittedString(string name, float lifetime = 5)
    {
        name.Replace(" ","");
        string[] arg = name.Split(',');
        if (int.TryParse(arg[1], out int time))
            lifetime = time;

        GameObject go = Instantiate(Resources.Load<GameObject>(arg[0]),this.gameObject.transform.position, Quaternion.identity);
        Destroy(go, lifetime);
        return go;
    }

    protected GameObject Emitted(GameObject go, float lifetime)
    {
        go = Instantiate(go, this.gameObject.transform.position, Quaternion.identity);
        Destroy(go, lifetime);
        return go;
    }

    // Update is called once per frame
    protected void Update()
    {
        
    }
}
