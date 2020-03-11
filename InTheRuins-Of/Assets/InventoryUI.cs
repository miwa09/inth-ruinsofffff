using UnityEngine;

public class InventoryUI : MonoBehaviour {

  public Transform itemsParet;
  public GameObject inventoryUI;

  Inventory inventory;

  InventorySlot[] slots;

  void Start() {
    inventory = Inventory.instance;
    try {
      inventory.onItemChangedCallback += UpdateUI;
    } catch (System.NullReferenceException) { }

    slots = itemsParet.GetComponentsInChildren<InventorySlot>();
  }

  void Update() {
    if (Input.GetButtonDown("Inventory")) {
      inventoryUI.SetActive(!inventoryUI.activeSelf);
    }
  }

  void UpdateUI() {
    // Debug.Log("UPDATING UI");
    for (int i = 0; i < slots.Length; i++) {
      if (i < inventory.items.Count) {
        slots[i].AddItem(inventory.items[i]);
      } else {
        slots[i].ClearSlot();
      }
    }
  }
}
