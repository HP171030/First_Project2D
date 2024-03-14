using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataBase : MonoBehaviour
{
    public static ItemDataBase instance;
    private void Awake()
    {
        instance = this;
    }

    public List<Item> itemsDB = new List<Item>();

    public GameObject fieldItemPrefab;
    

    private void Start()
    {
        for(int i = 0; i<5; i++)
        {
            GameObject Inst = Instantiate(fieldItemPrefab, transform.position
                + new Vector3(i,0), Quaternion.identity);
            Inst.GetComponent<Fielditem>().SetItem(itemsDB [Random.Range(0, 3)]);
        }
    }

}
