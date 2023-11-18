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
        EventController.AddListener(EventMessage.OnRollButtonClick, RollDice);
        EventController.AddListener(EventMessage.OnStopDice, StopDice);
        EventController.AddListener(EventMessage.OnRestartGame, RestartGame);
        EventController.AddListener(EventMessage.OnSumWithBonus, SumWithBonus);
        Generate();
    }

    private void RollDice() 
    {
        _randomValue = Random.Range(1, 21);
        EventController.Invoke(EventMessage.OnRollDice, _randomValue);
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

    private void SumWithBonus() 
    {
        EventController.Invoke(EventMessage.OnRandomAndBonusPoint, _randomValue, _randomBonus);
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
        EventController.RemoveListener(EventMessage.OnSumWithBonus, SumWithBonus);
        EventController.RemoveListener(EventMessage.OnRollButtonClick, RollDice);
        EventController.RemoveListener(EventMessage.OnStopDice, StopDice);
        EventController.RemoveListener(EventMessage.OnRestartGame, RestartGame);
    }
}
