using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    [SerializeField] GameObject _dicePopupPrefab;
    [SerializeField] Transform _dicePopupParent;
    private DiceRollLogic _diceLogic;

    private void Start()
    {
        GameObject eventController = new GameObject("EventController");
        eventController.AddComponent<EventController>();

        var dicePopup = Instantiate(_dicePopupPrefab, _dicePopupParent);
        var DiceRollUI = dicePopup.GetComponent<DiceRollView>();
        _diceLogic = new DiceRollLogic (DiceRollUI);

        var icosahedron = dicePopup.GetComponentInChildren<Icosahedron>();
        icosahedron.Mover = new IcosahedronMover(icosahedron);
        icosahedron.Rotator = new IcosahedronRotator(icosahedron);
    }
}