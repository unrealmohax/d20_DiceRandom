using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    [SerializeField] GameObject _dicePopupPrefab; // Prefab of the dice popup
    [SerializeField] Transform _dicePopupParent;   // Parent object for the popup window

    private void Start()
    {
        // Create an event controller object
        GameObject eventController = new GameObject("EventController");
        eventController.AddComponent<EventController>();

        // Initialize the dice popup window
        var dicePopup = Instantiate(_dicePopupPrefab, _dicePopupParent);

        // Get the DiceRollView component from the popup window
        var DiceRollUI = dicePopup.GetComponent<DiceRollView>();

        // Initialize the logic for dice rolling
        var diceLogic = new DiceRollLogic(DiceRollUI);

        // Get the Icosahedron component from the child objects of the popup window
        var icosahedron = dicePopup.GetComponentInChildren<Icosahedron>();

        // Assign the DiceRollView object to the Icosahedron component
        icosahedron.DiceRollView = DiceRollUI;

        // Initialize the movement and rotation of the icosahedron
        var mover = new IcosahedronMover(icosahedron);
        var rotator = new IcosahedronRotator(icosahedron);
    }
}