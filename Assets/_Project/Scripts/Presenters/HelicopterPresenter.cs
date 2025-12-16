using System.Collections.Generic;
using UnityEngine;
using Zenject;
using LastConvoy.Models;
using LastConvoy.StateMachine;
using LastConvoy.StateMachine.States;

namespace LastConvoy.Presenters
{
    public class HelicopterPresenter : IInitializable, ITickable
    {
        private readonly HelicopterModel _model;
        private readonly GameStateMachine _stateMachine;
        private readonly List<Transform> _waypoints;

        public HelicopterPresenter(
            HelicopterModel model,
            GameStateMachine stateMachine,
            [Inject(Id = "Waypoints")] List<Transform> waypoints)
        {
            _model = model;
            _stateMachine = stateMachine;
            _waypoints = waypoints;
        }

        public void Initialize()
        {
            if (_waypoints.Count > 0)
            {
                Vector3 startPos = _waypoints[0].position;
                float startYaw = 0f;

                if (_waypoints.Count > 1)
                {
                    Vector3 toNext = _waypoints[1].position - startPos;
                    toNext.y = 0;
                    if (toNext.sqrMagnitude > 0.01f)
                    {
                        startYaw = Mathf.Atan2(toNext.x, toNext.z) * Mathf.Rad2Deg;
                    }
                }

                _model.Initialize(startPos, startYaw);
            }
        }

        public void Tick()
        {
            if (!_stateMachine.IsInState<GameplayState>()) return;
            if (_waypoints.Count < 2) return;

            Transform currentWaypoint = _waypoints[_model.CurrentWaypointIndex];

            if (_model.HasReachedWaypoint(currentWaypoint.position))
            {
                _model.MoveToNextWaypoint(_waypoints.Count);
            }

            Transform targetWaypoint = _waypoints[_model.CurrentWaypointIndex];
            _model.Update(targetWaypoint.position, Time.deltaTime);
        }
    }
}
