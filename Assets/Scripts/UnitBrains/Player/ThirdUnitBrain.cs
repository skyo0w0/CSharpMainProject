using System.Collections;
using System.Collections.Generic;
using UnitBrains.Player;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ThirdUnitBrain : DefaultPlayerUnitBrain

{
    private bool _isAttacking = false;
    private bool _isMoving = true;
    private bool _isChangingMode = false;
    private float _changeTime = 60f;
    private float _timeFromChange = 0f;
    public override string TargetUnitName => "Ironclad Behemoth";

    public override void Update(float deltaTime, float time)
    {
        base.Update(deltaTime, time);
        if (_isChangingMode)
        {
            if (_timeFromChange == 0f)
            {
                _timeFromChange += Time.deltaTime;
            }

            if (_timeFromChange >= _changeTime)
            {
                _isChangingMode = false;
                _timeFromChange = 0f;

                _isAttacking = !_isAttacking;
                _isMoving = !_isMoving;
            }
        }
    }

    public override Vector2Int GetNextStep()
    {
        Vector2Int target =  base.GetNextStep();
        if (target == unit.Pos)
        {
            if (!_isAttacking)
            {
                _isChangingMode = true;
            }
        }
        else
        {
            if (_isAttacking)
            {
                _isChangingMode = true;
            }
        }
        return target;
    }

    protected override List<Vector2Int> SelectTargets()
    {
        
        var targets = base.SelectTargets();
        if (!_isMoving)
        {
            _isAttacking = true;
            _isChangingMode = true;
        }
        return new List<Vector2Int>();
    }
}
