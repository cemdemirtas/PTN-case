using System;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UI;

namespace Magiclab.InputHandler.Editor
{
    [CustomEditor(typeof(InputHandler))]
    public class InputHandlerEditor : UnityEditor.Editor
    {
        private SerializedProperty _distanceType;
        private SerializedProperty _swipeThresholdInches;
        private SerializedProperty _swipeThresholdPixels;
        private SerializedProperty _useCustomDPI;
        private SerializedProperty _customDPI;
        private SerializedProperty _isActiveJoystickVisual;
        private SerializedProperty _isFloatingJoystickCenterPoint;
        private SerializedProperty _joystickDragThresholdInches;
        private SerializedProperty _joystickMaxDistanceInches;
        private SerializedProperty _joystickDragThresholdPixels;
        private SerializedProperty _joystickMaxDistancePixels;
        
        private static ListRequest _listRequest;

        private const string DeviceSimulatorPackageName = "com.unity.device-simulator";

        private void OnEnable()
        {
            _distanceType = serializedObject.FindProperty("distanceType");
            _swipeThresholdInches = serializedObject.FindProperty("swipeThresholdInches");
            _swipeThresholdPixels = serializedObject.FindProperty("swipeThresholdPixels");
            _useCustomDPI = serializedObject.FindProperty("useCustomDPI");
            _customDPI = serializedObject.FindProperty("customDPI");
            _isActiveJoystickVisual = serializedObject.FindProperty("isActiveJoystickVisual");
            _isFloatingJoystickCenterPoint = serializedObject.FindProperty("isFloatingJoystickCenterPoint");
            _joystickDragThresholdInches = serializedObject.FindProperty("joystickDragThresholdInches");
            _joystickMaxDistanceInches = serializedObject.FindProperty("joystickMaxDistanceInches");
            _joystickDragThresholdPixels = serializedObject.FindProperty("joystickDragThresholdPixels");
            _joystickMaxDistancePixels = serializedObject.FindProperty("joystickMaxDistancePixels");
        }

        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            var prop = serializedObject.FindProperty("m_Script");
            EditorGUILayout.PropertyField(prop, true);
            GUI.enabled = true;

            serializedObject.Update();
            WriteInputHandlerVariables();
            WriteInfos();
            WriteDeviceSimulator();
            serializedObject.ApplyModifiedProperties();
        }

        private void WriteInputHandlerVariables()
        {
            WriteReferencesField();
            WriteGeneralSettings();
            WriteEditorSettings();
            WriteJoystickSettings();
        }

        private void WriteReferencesField()
        {
            var inputHandler = (InputHandler) target;
            EditorGUILayout.LabelField("References", EditorStyles.boldLabel);
            inputHandler.joystickBackground = (Image) EditorGUILayout.ObjectField("Joystick Background", 
                inputHandler.joystickBackground, typeof(Image), true);
            inputHandler.joystickImage = (Image) EditorGUILayout.ObjectField("Joystick Image", 
                inputHandler.joystickImage, typeof(Image), true);
        }

        private void WriteGeneralSettings()
        {
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_distanceType, new GUIContent("Distance Type"));
            if (_distanceType.enumValueIndex == (int)InputHandler.DistanceType.Inches)
            {
                EditorGUILayout.PropertyField(_swipeThresholdInches, new GUIContent("Swipe Threshold (Inches / Seconds)"));
            }
            else if (_distanceType.enumValueIndex == (int)InputHandler.DistanceType.Pixels)
            {
                EditorGUILayout.PropertyField(_swipeThresholdPixels, new GUIContent("Swipe Threshold (Pixels / Seconds)"));
            }
        }
        
        private void WriteEditorSettings()
        {
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField("Editor Settings", EditorStyles.boldLabel);

            var gameSize = GetMainGameViewSize();
            var isUsingIphoneSpec = _useCustomDPI.boolValue && Math.Abs(_customDPI.floatValue - 326f) < 0.01f &&
                (int)gameSize.x == 828 && (int)gameSize.y == 1792;
            if (_distanceType.enumValueIndex == (int)InputHandler.DistanceType.Inches)
            {
                EditorGUILayout.PropertyField(_useCustomDPI, new GUIContent("Use Custom Screen DPI"));
                EditorGUILayout.PropertyField(_customDPI, new GUIContent("Custom Screen DPI"));
                if (!isUsingIphoneSpec)
                {
                    EditorGUILayout.HelpBox("You can simulate mobile device by setting screen resolution and " +
                                            "screen dpi.\nIphone 11 specifications; Screen DPI : 326 / " +
                                            "Screen resolution : 828 x 1792", MessageType.Info);
                }
            }

            GUI.enabled = !isUsingIphoneSpec;
            if (GUILayout.Button("Use Iphone 11 Specifications"))
            {
                _useCustomDPI.boolValue = true;
                _customDPI.floatValue = 326f;
                SetGameViewSizeIndex(7);
            }
            GUI.enabled = true;
        }

        private void WriteJoystickSettings()
        {
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField("Joystick Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_isActiveJoystickVisual, new GUIContent("Is Active Visual"));
            EditorGUILayout.PropertyField(_isFloatingJoystickCenterPoint, new GUIContent("Is Floating Center Point"));
            
            if (_distanceType.enumValueIndex == (int)InputHandler.DistanceType.Inches)
            {
                EditorGUILayout.PropertyField(_joystickDragThresholdInches, new GUIContent("Drag Threshold (Inches)"));
                EditorGUILayout.PropertyField(_joystickMaxDistanceInches, new GUIContent("Max Distance (Inches)"));
            }
            else if (_distanceType.enumValueIndex == (int)InputHandler.DistanceType.Pixels)
            {
                EditorGUILayout.PropertyField(_joystickDragThresholdPixels, new GUIContent("Drag Threshold (Pixels)"));
                EditorGUILayout.PropertyField(_joystickMaxDistancePixels, new GUIContent("Max Distance (Pixels)"));
            }
        }

        private void WriteInfos()
        {
            var inputHandler = (InputHandler) target;
            var dpi = inputHandler.GetScreenDPI();
            EditorGUILayout.Space(10f);
            EditorGUILayout.HelpBox("It may work differently according to screen resolution and dpi values. " +
                                    "So it can work differently in the editor. " +
                                    "Please try it on Device Simulator or mobile device.", MessageType.Warning);
            
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField("Screen DPI:", dpi.ToString("0"));
            EditorGUILayout.LabelField("Screen Resolution:", $"{Screen.width} x {Screen.height}");
            var gameViewSize = GetMainGameViewSize();
            EditorGUILayout.LabelField("Game View Resolution:", $"{gameViewSize.x} x {gameViewSize.y}");
        }

        private void WriteDeviceSimulator()
        {
            EditorGUILayout.Space(10f);
            var isInstalled = false;
            var statusText = "Checking...";
            if (_listRequest.Status == StatusCode.Failure)
            {
                statusText = "Something went wrong!";
            }
            else if (_listRequest.Status == StatusCode.Success)
            {
                statusText = "Not Installed!";
                foreach (var packageInfo in _listRequest.Result)
                {
                    if (packageInfo.name != DeviceSimulatorPackageName) continue;

                    statusText = $"Installed! (v{packageInfo.version})";
                    isInstalled = true;
                    break;
                }
            }
            EditorGUILayout.LabelField("Device Simulator:", statusText);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUI.enabled = !isInstalled;
                if (GUILayout.Button("Install Device Simulator"))
                {
                    Client.Add(DeviceSimulatorPackageName);
                }
                GUI.enabled = isInstalled;
                if (GUILayout.Button("Remove Device Simulator"))
                {
                    Client.Remove(DeviceSimulatorPackageName);
                }
                GUI.enabled = true;
            }
        }
        
        private static Vector2 GetMainGameViewSize()
        {
            var T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            var getSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var res = getSizeOfMainGameView.Invoke(null,null);
            return (Vector2) res;
        }
        
        public static void SetGameViewSizeIndex(int index)
        {
            var gvWndType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GameView");
            var selectedSizeIndexProp = gvWndType.GetProperty("selectedSizeIndex",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | 
                System.Reflection.BindingFlags.NonPublic);
            var gvWnd = EditorWindow.GetWindow(gvWndType);
            selectedSizeIndexProp?.SetValue(gvWnd, index, null);
        }

        [InitializeOnLoadMethod]
        public static void CheckInstalledDeviceSimulator()
        {
            _listRequest = Client.List();
        }
    }
}