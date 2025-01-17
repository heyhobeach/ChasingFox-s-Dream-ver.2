using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;



public class Inventory : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public class Info
    {
        public string name;
        public string context;
        public Sprite image;
    }
   public  Dictionary<int, Info> inventory;//static으로 해결은 가능한데
    //public Dictionary<int, int> inventory2;
    public  event Action<int, Info> OnItemAdded;//static으로 해결은 가능한데
    InventoryScripable invendata;

    public int invenCount;

    private new void Awake()
    {
        //base.Awake();

        invendata = Resources.Load("Inventory") as InventoryScripable;
        inventory = new Dictionary<int, Info>();

    }
     public void testFunc1()
    {
        Debug.Log("inventory 싱글톤 테스트");
    }

    private Info SetInfoStruct(Collection.CollectionScriptorble collection)
    {
        Info info=new Info();
        info.name = collection.name;
        info.context = collection._context;
        info.image = collection.image;
        return info;
    }
    public void AddInventory(Collection.CollectionScriptorble collection)
    {
        if (!inventory.ContainsKey(collection.id))
        {
            inventory.Add(collection.id, SetInfoStruct(collection));

            invendata.inventory = inventory;
            invenCount = invendata.inventory.Count;
            Debug.Log("inventory 개수" + inventory.Count);
            //Debug.Log(string.Format("수집품 추가 완료+{0} : {1},{2}", collection.id, inventory[collection.id].name, inventory[collection.id].context));
        }
    }
    public Info GetInfo(int id)
    {
        if (inventory.ContainsKey(id))
        {
            return inventory[id];
        }
        else
        {
            return null;
        }
    }
    protected Info GetInfo()
    {
        Info info = new Info();
        return info;
    }

}



