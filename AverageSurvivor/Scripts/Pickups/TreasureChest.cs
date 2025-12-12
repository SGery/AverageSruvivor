using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    InventoryManager inventory;

    private void Start()
    {
        inventory = FindObjectOfType<InventoryManager>();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OpenTreasureChest();
            Destroy(gameObject);
        }
    }

    public void OpenTreasureChest()
    {
        if(inventory.GetPossibleWeaponEvolutions().Count <= 0)
        {
            Debug.LogWarning("No possible weapon evolutions available.");
            return;
        }

        WeaponEvolutionBlueprint toEvolve = inventory.GetPossibleWeaponEvolutions()[Random.Range(0, inventory.GetPossibleWeaponEvolutions().Count)];
        inventory.EvolveWeapon(toEvolve);
    }
}
