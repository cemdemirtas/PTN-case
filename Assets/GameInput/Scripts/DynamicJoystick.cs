using UnityEngine;

namespace Magiclab.InputHandler
{
    public class DynamicJoystick
    {
        public bool IsActive { get; private set; }
        public Vector2 JoystickCenter { get; private set; } = Vector2.zero;
        public Vector2 JoystickPosition { get; private set; } = Vector2.zero;

        private readonly InputHandler _inputHandler;
        private float _currentMaxDistance;
        
        public DynamicJoystick(InputHandler inputHandler)
        {
            _inputHandler = inputHandler;
        }

        public void UpdateJoyStick(Vector2 inputPosition)
        {
            if (!IsActive)
            {
                JoystickCenter = inputPosition;
                IsActive = true;
                _currentMaxDistance = _inputHandler.distanceType == InputHandler.DistanceType.Pixels ? 
                    _inputHandler.joystickMaxDistancePixels : 
                    _inputHandler.joystickMaxDistanceInches * InputHandler.ScreenDPI;
            }

            JoystickPosition = inputPosition;

            var joystickDistance = JoystickPosition - JoystickCenter;
            if (_inputHandler.isFloatingJoystickCenterPoint)
            {
                if (joystickDistance.magnitude > _currentMaxDistance)
                {
                    var direction = joystickDistance.normalized;
                    JoystickCenter = JoystickPosition - direction * _currentMaxDistance;
                }
            }

            joystickDistance = Vector2.ClampMagnitude(joystickDistance, _currentMaxDistance);
            JoystickPosition = JoystickCenter + joystickDistance;
        }

        public void DisableJoyStick()
        {
            IsActive = false;
        }

        public Vector2 GetInput()
        {
            if (!IsActive)
            {
                return Vector2.zero;
            }
            
            var joystickDistance = JoystickPosition - JoystickCenter;

            var isOverThreshold = false;
            if (_inputHandler.distanceType == InputHandler.DistanceType.Pixels)
            {
                isOverThreshold = joystickDistance.magnitude > _inputHandler.joystickDragThresholdPixels;
            }
            else
            {
                isOverThreshold = joystickDistance.magnitude > _inputHandler.joystickDragThresholdInches * InputHandler.ScreenDPI;
            }
            
            return isOverThreshold ? 
                Vector2.ClampMagnitude(joystickDistance / _currentMaxDistance, 1f) : Vector2.zero;
        }

        public float GetHorizontal()
        {
            return GetInput().x;
        }

        public float GetVertical()
        {
            return GetInput().y;
        }
    }
}
