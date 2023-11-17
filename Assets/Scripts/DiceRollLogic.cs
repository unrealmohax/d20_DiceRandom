using System;
using Random = UnityEngine.Random;

public class DiceRollLogic : IDisposable
{
    private DiceRollView _diceRollpopup;
    private int _requiredValue = 1;
    private int _randomValue = 1;
    private int _randomBonus = 0;

    public DiceRollLogic(DiceRollView diceRollpopup)
    {
        _diceRollpopup = diceRollpopup;
        EventController.AddListener(EventMessage.OnRollDice, RollDice);
        EventController.AddListener(EventMessage.OnStopDice, StopDice);
        EventController.AddListener(EventMessage.OnRestartGame, RestartGame);
        Generate();
    }

    private void RollDice() 
    {
        _randomValue = Random.Range(1, 21);
    }

    private void StopDice() 
    {
        bool win = _randomValue + _randomBonus >= _requiredValue;
        
        EventController.Invoke(EventMessage.OnFinishGame, win);
    }

    public void RestartGame() 
    {
        Generate();
    }

    private void Generate() 
    {
        _randomBonus = Random.Range(0, 5);
        _requiredValue = Random.Range(1, 15);
        
        _diceRollpopup.SetValueText((uint)_requiredValue);
        _diceRollpopup.SetBonusValueText((uint)_randomBonus);

        EventController.Invoke(EventMessage.OnStartNewGame, (uint)_requiredValue);
    }

    public void Dispose()
    {
        EventController.RemoveListener(EventMessage.OnRollDice, RollDice);
        EventController.RemoveListener(EventMessage.OnStopDice, StopDice);
        EventController.RemoveListener(EventMessage.OnRestartGame, RestartGame);
    }
}
