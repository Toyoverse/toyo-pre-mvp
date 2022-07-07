using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TrainingConfig
{
    public TrainingActionSO[] possibleActions;
    public TrainingActionSO[] correctCombination;

    public TrainingConfig(TrainingConfigSO trainingConfigSo)
    {
        possibleActions = trainingConfigSo.possibleActions;
        correctCombination = trainingConfigSo.correctCombination;
    }
    
    public List<TRAINING_RESULT> CompareCombination(TrainingActionSO[] selectedActions)
    {
        var _listResult = new List<TRAINING_RESULT>();
        for (var _i = 0; _i < selectedActions.Length; _i++)
            _listResult.Add(GetResultToAction(selectedActions[_i], _i));

        return _listResult;
    }

    private TRAINING_RESULT GetResultToAction(TrainingActionSO selectedAction, int position)
    {
        var _result = TRAINING_RESULT.TOTALLY_WRONG;

        for (var _i = 0; _i < correctCombination.Length; _i++)
        {
            if (correctCombination[_i] != selectedAction) continue;
            _result += 1;
            if (position == _i)
                _result += 1;
        }
        Debug.Log(_result);
        return _result;
    }
}