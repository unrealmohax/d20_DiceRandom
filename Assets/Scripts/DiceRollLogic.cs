using System;
using Random = UnityEngine.Random;

public class DiceRollLogic : IDisposable
{
    private DiceRollView _diceRollpopup;   // Reference to the DiceRollView component
    private int _requiredValue = 1;        // The required value for winning the game
    private int _randomValue = 1;          // The randomly rolled value on the dice
    private int _randomBonus = 0;          // Randomly generated bonus value

    // Constructor to initialize DiceRollLogic with a DiceRollView reference
    public DiceRollLogic(DiceRollView diceRollpopup)
    {
        _diceRollpopup = diceRollpopup;
        EventController.AddListener(EventMessage.OnRollButtonClick, RollDice);
        EventController.AddListener(EventMessage.OnStopDice, StopDice);
        EventController.AddListener(EventMessage.OnRestartGame, RestartGame);
        EventController.AddListener(EventMessage.OnSumWithBonus, SumWithBonus);
        Generate();  // Initial generation of values
    }

    // Method to roll the dice and trigger associated events
    private void RollDice()
    {
        _randomValue = Random.Range(1, 21);  // Roll a random value between 1 and 20
        EventController.Invoke(EventMessage.OnRollDice, _randomValue);
    }

    // Method to determine the game outcome and trigger finish game event
    private void StopDice()
    {
        // Check if the sum of the rolled value and bonus is greater than or equal to the required value
        bool win = _randomValue + _randomBonus >= _requiredValue;
        EventController.Invoke(EventMessage.OnFinishGame, win);
    }

    // Method to restart the game by generating new values
    public void RestartGame()
    {
        Generate();
    }

    // Method to handle the sum of the random value and bonus, triggering relevant events
    private void SumWithBonus()
    {
        EventController.Invoke(EventMessage.OnRandomAndBonusPoint, _randomValue, _randomBonus);
    }

    // Method to generate random values for bonus and required value, updating the UI
    private void Generate()
    {
        _randomBonus = Random.Range(0, 5);       // Generate a random bonus value between 0 and 4
        _requiredValue = Random.Range(1, 15);    // Generate a random required value between 1 and 14

        // Update UI with the generated values
        _diceRollpopup.SetValueText((uint)_requiredValue);
        _diceRollpopup.SetBonusValueText((uint)_randomBonus);

        // Trigger event to signal the start of a new game with the required value
        EventController.Invoke(EventMessage.OnStartNewGame, (uint)_requiredValue);
    }

    // Method to dispose of event listeners when the object is destroyed
    public void Dispose()
    {
        EventController.RemoveListener(EventMessage.OnSumWithBonus, SumWithBonus);
        EventController.RemoveListener(EventMessage.OnRollButtonClick, RollDice);
        EventController.RemoveListener(EventMessage.OnStopDice, StopDice);
        EventController.RemoveListener(EventMessage.OnRestartGame, RestartGame);
    }
}