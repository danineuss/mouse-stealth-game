using System.Collections;
using System.Collections.Generic;
using Infrastructure;
using UnityEngine;

namespace Enemies.VisionCone
{
    public interface IVisionConeViewModel: IUpdatable
    {
        Vector3 CurrentLookAtTarget { get; }
        float FieldOfView { get; }
        float Range { get; }

        bool IsPlayerInsideVisionCone();
        bool IsPlayerObstructed();
        void IterateControlPointIndex();
        IEnumerator LerpTowardsTarget(
            Vector3 newLookAtTarget, float newFieldOfView, float durationSeconds);
        void StartLookAtCoroutine(IEnumerator lerpCoroutine, bool interrupt = true);
        void TransitionTo(VisionConeState visionConeState);
        void UpdateCone(Vector3 target, float fieldOfView);
        void UpdateDetectionMeter(float detectionMeter);
    }

    public class VisionConeViewModel : IVisionConeViewModel
    {
        public Vector3 CurrentLookAtTarget { get; private set; }
        public float FieldOfView { get; private set; }
        public float Range => (CurrentLookAtTarget - coneTransform.position).magnitude;

        private List<IVisionConePatrolPoint> patrolPoints;
        private IVisionConeControlPoint distractPoint;
        private IConeVisualizer coneVisualizer;
        private Transform coneTransform;
        private Transform playerTransform;
        private LayerMask obstacleMask;
        private IEvents events;

        private VisionConeState visionConeState;
        private List<IEnumerator> currentCoroutines = new List<IEnumerator>();
        private int controlPointIndex;

        public VisionConeViewModel(
            List<IVisionConePatrolPoint> patrolPoints,
            IVisionConeControlPoint distractPoint,
            IConeVisualizer coneVisualizer,
            Transform coneTransform,
            Transform playerTransform,
            LayerMask obstacleMask,
            IEvents events)
        {
            this.patrolPoints = patrolPoints;
            this.distractPoint = distractPoint;
            this.coneVisualizer = coneVisualizer;
            this.coneTransform = coneTransform;
            this.playerTransform = playerTransform;
            this.obstacleMask = obstacleMask;
            this.events = events;

            InitializeCone();

            IterateControlPointIndex();
            TransitionTo(new VisionConeStateIdle());
        }

        void InitializeCone()
        {
            var currentControlPoint = patrolPoints[controlPointIndex];
            CurrentLookAtTarget = currentControlPoint.Position;
            FieldOfView = currentControlPoint.FieldOfView;

            coneVisualizer.UpdateConeOrientation(CurrentLookAtTarget, FieldOfView);
        }

        public bool IsPlayerInsideVisionCone()
        {
            float angleToPlayer = Vector3.Angle(
                CurrentLookAtTarget - coneTransform.position,
                playerTransform.position - coneTransform.position
            );
            float distanceToPlayer = Vector3.Distance(playerTransform.position, coneTransform.position);
            return (angleToPlayer < FieldOfView / 2 && distanceToPlayer < Range);
        }

        public bool IsPlayerObstructed()
        {
            return Physics.Raycast(
                coneTransform.position,
                (playerTransform.position - coneTransform.position).normalized,
                Vector3.Distance(playerTransform.position, coneTransform.position),
                obstacleMask,
                QueryTriggerInteraction.Ignore
            );
        }
    
        public void IterateControlPointIndex()
        {
            if (patrolPoints.Count == 2)
                controlPointIndex = 1 - controlPointIndex;
        }

        public IEnumerator LerpTowardsTarget(
            Vector3 newLookAtTarget, float newFieldOfView, float durationSeconds)
        {
            Vector3 startLookatTarget = CurrentLookAtTarget;
            float startFieldOfView = FieldOfView;
            float elapsedTime = 0f;

            while (elapsedTime < durationSeconds)
            {
                CurrentLookAtTarget = Vector3.Slerp(
                    startLookatTarget, newLookAtTarget, elapsedTime / durationSeconds);
                FieldOfView = Mathf.Lerp(
                    startFieldOfView, newFieldOfView, elapsedTime / durationSeconds);
            
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        public void StartLookAtCoroutine(IEnumerator newCoroutine, bool interrupt = true)
        {
            if (currentCoroutines.Count != 0 && interrupt)
            {
                currentCoroutines.ForEach(x => events.StopCoroutine(x));
                currentCoroutines.Clear();
            }
        
            currentCoroutines.Add(newCoroutine);
            events.StartCoroutine(newCoroutine);
        }

        public void TransitionTo(VisionConeState visionConeState)
        {
            visionConeState.SetupVisionConeState(
                this, 
                coneVisualizer,
                patrolPoints[controlPointIndex],
                distractPoint,
                playerTransform
            );
            this.visionConeState = visionConeState;
        }

        public void Update()
        {
            coneVisualizer.UpdateConeOrientation(CurrentLookAtTarget, FieldOfView);
        }

        public void UpdateCone(Vector3 target, float fieldOfView)
        {
            CurrentLookAtTarget = target;
            FieldOfView = fieldOfView;
        }

        public void UpdateDetectionMeter(float detectionMeter)
        {
            visionConeState.UpdateDetectionMeter(detectionMeter);
        }
    }
}