﻿using System.Collections.Generic;
using Model.Runtime.Projectiles;
using Unity.VisualScripting;
using UnityEngine;
using Model;
using UnityEngine.UIElements;
using Utilities;

namespace UnitBrains.Player
{
    public class SecondUnitBrain : DefaultPlayerUnitBrain
    {
        public override string TargetUnitName => "Cobra Commando";
        private const float OverheatTemperature = 3f;
        private const float OverheatCooldown = 2f;
        private float _temperature = 0f;
        private float _cooldownTime = 0f;
        private bool _overheated;
        private List<Vector2Int> _notReachebleTarget = new List<Vector2Int>();

        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            float overheatTemperature = OverheatTemperature;
            if (GetTemperature() >= overheatTemperature)
            {
                return;
            }
            IncreaseTemperature();
            ///////////////////////////////////////
            // Homework 1.3 (1st block, 3rd module)
            ///////////////////////////////////////           
            ///

            for (int i = 0; i < GetTemperature(); i++)
            {
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
            }
            ///////////////////////////////////////
        }

        public override Vector2Int GetNextStep()
        {
            List<Vector2Int> targetsInRange = SelectTargets();
            if (targetsInRange.Count > 0 && _notReachebleTarget.Count == 0)
            {
                return unit.Pos;
            }
            else
            {
                Vector2Int nextStep = unit.Pos.CalcNextStepTowards(_notReachebleTarget[0]);
                _notReachebleTarget.Clear();
                return nextStep;
            }
        }

        protected override List<Vector2Int> SelectTargets()
        {
            ///////////////////////////////////////
            // Homework 1.4 (1st block, 4rd module)
            ///////////////////////////////////////
            List<Vector2Int> result = (List<Vector2Int>) GetAllTargets();
            List<Vector2Int> reachebleTargets = GetReachableTargets();
            float maxValue = float.MaxValue;
            Vector2Int minVector2 = Vector2Int.zero;
            foreach (Vector2Int v2 in result)
            {
                float distance = DistanceToOwnBase(v2);
                if (maxValue > distance)
                {
                    maxValue = distance;
                    minVector2 = v2;
                }
            }
            result.Clear();
            if (maxValue != float.MaxValue)
            {
                if (reachebleTargets.Contains(minVector2))
                {
                    result.Add(minVector2);
                }
                else
                {
                    _notReachebleTarget.Add(minVector2);
                }
            }
            else
            {
                Vector2Int enemyBase = runtimeModel.RoMap.Bases[RuntimeModel.BotPlayerId];
                result.Add(enemyBase);
            }
            return result;
            ///////////////////////////////////////
        }

        public override void Update(float deltaTime, float time)
        {
            if (_overheated)
            {
                _cooldownTime += Time.deltaTime;
                float t = _cooldownTime / ( OverheatCooldown / 10 );
                _temperature = Mathf.Lerp(OverheatTemperature, 0, t);
                if (t >= 1)
                {
                    _cooldownTime = 0;
                    _overheated = false;
                }
            }
        }

        private int GetTemperature()
        {
            if (_overheated) return (int)OverheatTemperature;
            else return (int)_temperature;
        }

        private void IncreaseTemperature()
        {
            _temperature += 1f;
            if (_temperature >= OverheatTemperature) _overheated = true;
        }
    }
}