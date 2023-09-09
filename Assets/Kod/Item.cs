using UnityEngine;

public enum ItemType { Weapon, Book, Armour, Pet, Core }

[System.Serializable]
public class Item
{
    public string itemName;
    public ItemType itemType;
    //public ItemElement itemElement;
    public int level; // 1, 2, 3
    public int stat; // e.g., damage for weapons

    public Sprite sprite;
    public Mesh mesh;

    public Item(string itemName, ItemType itemType, int level, int stat)
    {
        this.itemName = itemName;
        this.itemType = itemType;
        this.level = level;
        this.stat = stat;
    }

    public void StatAtla()
    {
        stat++;
    }
}