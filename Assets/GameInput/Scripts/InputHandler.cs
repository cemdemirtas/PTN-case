using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Magiclab.InputHandler
{
    public class InputHandler : MonoBehaviour
    {
        public Image joystickBackground;
        public Image joystickImage;
        
        public DistanceType distanceType = DistanceType.Inches;
        public bool useCustomDPI = true;
        public float customDPI = 326f;
        public float swipeThresholdInches = 3f;
        public float swipeThresholdPixels = 800f;
        public bool isActiveJoystickVisual = false;
        public bool isFloatingJoystickCenterPoint = true;
        public float joystickDragThresholdInches = 0.05f;
        public float joystickMaxDistanceInches = 0.5f;
        public float joystickDragThresholdPixels = 10f;
        public float joystickMaxDistancePixels = 250f;

        public static float ScreenDPI { get; private set; }
        public static Vector2 DeltaMousePosition { get; private set; }
        public static Vector2 DeltaMouseInchesPosition => DeltaMousePosition / ScreenDPI;
        public static Vector2 DragVector => _lastMousePosition - _firstMousePosition;
        public static float DragMovementTime => _dragTime - _mouseDownTime;
        public static Vector2 DragVelocityInches => (DragVector / ScreenDPI) / DragMovementTime;
        public static Vector2 DragVelocityPixels => DragVector / DragMovementTime;
        public static SwipeDirection SwipeDirection { get; private set; }
        public static DynamicJoystick Joystick { get; private set; }
        public static Camera GameCamera { get; private set; }

        private static InputHandler _instance;
        
        private static Vector3 _lastMousePosition;
        private static Vector3 _firstMousePosition;
        private static Vector3 _lastRayWorldPosition;
        private static Vector3 _firstRayWorldPosition;
        private static bool _isActiveDeltaScreenPositionWithRay;
        private static float _mouseDownTime;
        private static float _dragTime;

        private RectTransform _joystickBackgroundRectTransform;
        private RectTransform _joystickRectTransform;
        private RectTransform _joystickContainerRectTransform;
        private Vector2 _rectFactor;

        public enum DistanceType
        {
            Inches,
            Pixels
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(this);
                return;
            }

            ScreenDPI = GetScreenDPI();

            Joystick = new DynamicJoystick(this);
            _joystickBackgroundRectTransform = joystickBackground.GetComponent<RectTransform>();
            _joystickRectTransform = joystickImage.GetComponent<RectTransform>();
            _joystickContainerRectTransform = joystickBackground.transform.parent.GetComponent<RectTransform>();
            
            SetGameCamera(Camera.main);
            DisableJoystick();
        }

        private void Update()
        {
            if (GetMouseButtonDown())
            {
                _firstMousePosition = GetMousePosition();
                _lastMousePosition = _firstMousePosition;
                DeltaMousePosition = Vector2.zero;
                _mouseDownTime = Time.time;
                UpdateJoystick();
            }
            else if (GetMouseButton() || GetMouseButtonUp())
            {
                var mousePosition = GetMousePosition();
                DeltaMousePosition = mousePosition - _lastMousePosition;
                _lastMousePosition = mousePosition;
                _dragTime = Time.time;

                if (GetMouseButtonUp())
                {
                    _isActiveDeltaScreenPositionWithRay = false;
                    DisableJoystick();
                }
                else
                {
                    UpdateJoystick();
                }
            }
            else
            {
                DeltaMousePosition = Vector2.zero;
                DisableJoystick();
            }

            GetSwipeDirection();
        }

        private void UpdateJoystick()
        {
            if (isActiveJoystickVisual && !Joystick.IsActive)
            {
                _joystickContainerRectTransform.gameObject.SetActive(true);
                
                var rect = _joystickContainerRectTransform.rect;
                _rectFactor = new Vector2(rect.width / Screen.width, rect.height / Screen.height);
            }
            Joystick.UpdateJoyStick(GetMousePosition());
            if (Joystick.IsActive)
            {
                var backgroundPosition = new Vector2(Joystick.JoystickCenter.x * _rectFactor.x,
                    Joystick.JoystickCenter.y * _rectFactor.y);
                _joystickBackgroundRectTransform.anchoredPosition = backgroundPosition;
                var joystickPosition = new Vector2(Joystick.JoystickPosition.x * _rectFactor.x,
                    Joystick.JoystickPosition.y * _rectFactor.y);
                _joystickRectTransform.anchoredPosition = joystickPosition;
            }
        }

        private void DisableJoystick()
        {
            if (Joystick.IsActive)
            {
                _joystickContainerRectTransform.gameObject.SetActive(false);
            }
            Joystick.DisableJoyStick();
        }
        
        private void GetSwipeDirection()
        {
            SwipeDirection = SwipeDirection.None;
            if (!GetMouseButtonUp()) return;

            var velocity = distanceType == DistanceType.Pixels ? DragVelocityPixels : DragVelocityInches;
            var threshold = distanceType == DistanceType.Pixels ? swipeThresholdPixels : swipeThresholdInches;
            
            if (velocity.magnitude > threshold)
            {
                if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y))
                {
                    SwipeDirection = velocity.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
                }
                else
                {
                    SwipeDirection = velocity.y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
                }
            }
        }

        public static float GetHorizontal()
        {
            var total = Input.GetAxisRaw("Horizontal");
            total += Joystick.GetHorizontal();
            return Mathf.Clamp(total,-1,1);
        }

        public static float GetVertical()
        {
            var total = Input.GetAxisRaw("Vertical");
            total += Joystick.GetVertical();
            return Mathf.Clamp(total, -1, 1);
        }

        public static Vector2 GetInput()
        {
            return new Vector2(GetHorizontal(), GetVertical());
        }

        public static Vector3 Get3DInput()
        {
            return new Vector3(GetHorizontal(), 0, GetVertical());
        }

        public static Vector2 GetJoystickInput()
        {
            return Joystick.GetInput();
        }

        public static bool GetMouseButtonDown()
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
#else
            return Input.GetMouseButtonDown(0);
#endif
        }

        public static bool GetMouseButton()
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            if (Input.touchCount <= 0) return false;
            
            var touchPhase = Input.GetTouch(0).phase;
            return touchPhase != TouchPhase.Ended && touchPhase != TouchPhase.Canceled;
#else
            return Input.GetMouseButton(0);
#endif
        }

        public static bool GetMouseButtonUp()
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            if (Input.touchCount <= 0) return false;

            var touchPhase = Input.GetTouch(0).phase;
            return touchPhase == TouchPhase.Ended || touchPhase == TouchPhase.Canceled;
#else
            return Input.GetMouseButtonUp(0);
#endif
        }
        
        public static Vector3 GetMousePosition()
        {
            var mousePosition = Vector3.zero;

            if (GetMouseButton() || GetMouseButtonDown() || GetMouseButtonUp())
            {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                mousePosition = Input.GetTouch(0).position;
#else
                mousePosition = Input.mousePosition;
#endif
            }

            return mousePosition;
        }
        
        public static bool RaycastWithMousePosition(float distance, LayerMask layerMask, Vector3? offset = null)
        {
            if (GetMouseButtonDown() || GetMouseButton() || GetMouseButtonUp())
            {
                var ray = RayForRaycastWithMousePosition(offset ?? Vector3.zero);
                return Physics.Raycast(ray, distance, layerMask);
            }
            
            return false;
        }
        
        [Obsolete("Use \"RaycastWithMousePosition(out RaycastHit raycastHit, float distance, Vector3? offset = null)\".")]
        public static bool RaycastWithMousePosition(Vector3 offset, out RaycastHit raycastHit, float distance)
        {
            if (GetMouseButtonDown() || GetMouseButton() || GetMouseButtonUp())
            {
                var ray = RayForRaycastWithMousePosition(offset);
                return Physics.Raycast(ray, out raycastHit, distance);
            }
            
            raycastHit = new RaycastHit();
            return false;
        }

        public static bool RaycastWithMousePosition(out RaycastHit raycastHit, float distance, Vector3? offset = null)
        {
            if (GetMouseButtonDown() || GetMouseButton() || GetMouseButtonUp())
            {
                var ray = RayForRaycastWithMousePosition(offset ?? Vector3.zero);
                return Physics.Raycast(ray, out raycastHit, distance);
            }
            
            raycastHit = new RaycastHit();
            return false;
        }
        
        [Obsolete("Use \"RaycastWithMousePosition(out RaycastHit raycastHit, float distance, LayerMask layerMask, Vector3? offset = null)\".")]
        public static bool RaycastWithMousePosition(Vector3 offset, out RaycastHit raycastHit, float distance, LayerMask layerMask, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            if (GetMouseButtonDown() || GetMouseButton() || GetMouseButtonUp())
            {
                var ray = RayForRaycastWithMousePosition(offset);
                return Physics.Raycast(ray, out raycastHit, distance, layerMask, triggerInteraction);
            }
            
            raycastHit = new RaycastHit();
            return false;
        }
        
        public static bool RaycastWithMousePosition(out RaycastHit raycastHit, float distance, LayerMask layerMask, Vector3? offset = null)
        {
            if (GetMouseButtonDown() || GetMouseButton() || GetMouseButtonUp())
            {
                var ray = RayForRaycastWithMousePosition(offset ?? Vector3.zero);
                return Physics.Raycast(ray, out raycastHit, distance, layerMask);
            }
            
            raycastHit = new RaycastHit();
            return false;
        }

        public static bool RaycastWithMousePosition(out RaycastHit raycastHit, float distance, Collider collider, Vector3? offset = null)
        {
            if (GetMouseButtonDown() || GetMouseButton() || GetMouseButtonUp())
            {
                var ray = RayForRaycastWithMousePosition(offset ?? Vector3.zero);
                return collider.Raycast(ray, out raycastHit, distance);
            }
            
            raycastHit = new RaycastHit();
            return false;
        }

        public static Ray RayForRaycastWithMousePosition(Vector3 offset)
        {
            return GameCamera.ScreenPointToRay(GetMousePosition() + offset * ScreenDPI);
        }

        public static bool DeltaWorldPositionWithRay(out Vector3 deltaPosition, out RaycastHit raycastHit, float distance, LayerMask layerMask)
        {
            if (RaycastWithMousePosition(out raycastHit, distance, layerMask))
            {
                if (GetMouseButtonDown())
                {
                    _firstRayWorldPosition = raycastHit.point;
                    _lastRayWorldPosition = _firstRayWorldPosition;
                }
                else if (GetMouseButton())
                {
                    _lastRayWorldPosition = raycastHit.point;
                }
            }
            else
            {
                if (!GetMouseButton())
                {
                    deltaPosition = Vector3.zero;
                    return false;
                }
            }

            deltaPosition = _lastRayWorldPosition - _firstRayWorldPosition;
            return true;
        }
        
        public static bool DeltaWorldPositionWithRay(out Vector3 deltaPosition, out RaycastHit raycastHit, float distance, Collider collider)
        {
            if (RaycastWithMousePosition(out raycastHit, distance, collider))
            {
                if (GetMouseButtonDown())
                {
                    _firstRayWorldPosition = raycastHit.point;
                    _lastRayWorldPosition = _firstRayWorldPosition;
                }
                else if (GetMouseButton())
                {
                    _lastRayWorldPosition = raycastHit.point;
                }
            }
            else
            {
                if (!GetMouseButton())
                {
                    deltaPosition = Vector3.zero;
                    return false;
                }
            }

            deltaPosition = _lastRayWorldPosition - _firstRayWorldPosition;
            return true;
        }
        
        public static bool DeltaWorldPositionWithRayForFrame(out Vector3 deltaPosition, out RaycastHit raycastHit, float distance, LayerMask layerMask)
        {
            if (!RaycastWithMousePosition(out raycastHit, distance, layerMask))
            {
                deltaPosition = Vector3.zero;
                return false;
            }
            
            if (GetMouseButtonDown())
            {
                _firstRayWorldPosition = raycastHit.point;
                _lastRayWorldPosition = _firstRayWorldPosition;
                deltaPosition = Vector3.zero;
                return false;
            }

            deltaPosition = raycastHit.point - _lastRayWorldPosition;
            _lastRayWorldPosition = raycastHit.point;
            return true;
        }
        
        public static bool DeltaWorldPositionWithRayForFrame(out Vector3 deltaPosition, out RaycastHit raycastHit, float distance, Collider collider)
        {
            if (!RaycastWithMousePosition(out raycastHit, distance, collider))
            {
                deltaPosition = Vector3.zero;
                return false;
            }
            
            if (GetMouseButtonDown())
            {
                _firstRayWorldPosition = raycastHit.point;
                _lastRayWorldPosition = _firstRayWorldPosition;
                deltaPosition = Vector3.zero;
                return false;
            }

            deltaPosition = raycastHit.point - _lastRayWorldPosition;
            _lastRayWorldPosition = raycastHit.point;
            return true;
        }

        public static bool DeltaScreenPositionWithRay(out Vector3 deltaPosition, out RaycastHit raycastHit, float distance, LayerMask layerMask)
        {
            if (RaycastWithMousePosition(out raycastHit, distance, layerMask) && GetMouseButtonDown())
            {
                _isActiveDeltaScreenPositionWithRay = true;
            }

            if (!_isActiveDeltaScreenPositionWithRay)
            {
                deltaPosition = Vector3.zero;
                return false;
            }
            
            if (GetMouseButtonDown())
            {
                _firstMousePosition = GetMousePosition();
                _lastMousePosition = _firstMousePosition;
                deltaPosition = Vector3.zero;
                return false;
            }

            var mousePosition = GetMousePosition();
            deltaPosition = mousePosition - _lastMousePosition;
            _lastMousePosition = mousePosition;
            return true;
        }
        
        public static bool DeltaScreenPositionWithRay(out Vector3 deltaPosition, out RaycastHit raycastHit, float distance, Collider collider)
        {
            if (RaycastWithMousePosition(out raycastHit, distance, collider) && GetMouseButtonDown())
            {
                _isActiveDeltaScreenPositionWithRay = true;
            }

            if (!_isActiveDeltaScreenPositionWithRay)
            {
                deltaPosition = Vector3.zero;
                return false;
            }
            
            if (GetMouseButtonDown())
            {
                _firstMousePosition = GetMousePosition();
                _lastMousePosition = _firstMousePosition;
                deltaPosition = Vector3.zero;
                return false;
            }

            var mousePosition = GetMousePosition();
            deltaPosition = mousePosition - _lastMousePosition;
            _lastMousePosition = mousePosition;
            return true;
        }
        
        public static bool IsPointerOverGameObject()
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            return GetMouseButton() && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
#else
            return GetMouseButton() && !EventSystem.current.IsPointerOverGameObject();
#endif
        }

        public static void SetGameCamera(Camera gameCamera)
        {
            GameCamera = gameCamera;
        }

        public float GetScreenDPI()
        {
            var dpi = Screen.dpi;
#if UNITY_EDITOR
            if (useCustomDPI)
            {
                dpi = customDPI;
            }
#endif
            return dpi;
        }
    }

    public enum SwipeDirection
    {
        None,
        Left,
        Right,
        Up,
        Down
    }
}
