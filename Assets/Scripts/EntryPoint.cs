using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    [SerializeField] GameObject _dicePopupPrefab;
    [SerializeField] Transform _dicePopupParent;

    private void Start()
    {
        GameObject eventController = new GameObject("EventController");
        eventController.AddComponent<EventController>();

        var dicePopup = Instantiate(_dicePopupPrefab, _dicePopupParent);
        var DiceRollUI = dicePopup.GetComponent<DiceRollView>();
        var diceLogic = new DiceRollLogic (DiceRollUI);

        var icosahedron = dicePopup.GetComponentInChildren<Icosahedron>();
        icosahedron.DiceRollView = DiceRollUI;

        var mover = new IcosahedronMover(icosahedron);
        var rotator = new IcosahedronRotator(icosahedron);
    }
}