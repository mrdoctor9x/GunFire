using UnityEngine;

namespace GenifyStudio.Scripts.Tools.Log
{
    public class FPS : MonoBehaviour
    {
        public enum DisplayFPS
        {
            Yes,
            No
        }

        public Color textColor = new Color( 1f, 0.0f, 0f, 1.0f);
        public DisplayFPS displayFPS;
        private float deltaTime;
        private bool isRecordingFPS;
        private int nSample;

        private float sumFPS;

        private void Awake()
        {
            if (Application.targetFrameRate < 60) Application.targetFrameRate = 60;
        }

        private void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            if (!isRecordingFPS) return;
            sumFPS += GetFPS();
            nSample++;
        }

        private void OnGUI()
        {
            if (displayFPS != DisplayFPS.Yes) return;
            int w = Screen.width, h = Screen.height;

            var style = new GUIStyle();

            var rect = new Rect(0, 0, w, h * (2 / 100));
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 100;
            style.normal.textColor = textColor;
            var mSec = deltaTime * 1000.0f;
            var fps = GetFPS();
            var text = $"{mSec:0.0} ms ({fps:0.} fps)";
            GUI.Label(rect, text, style);
        }

        private float GetFPS()
        {
            if (deltaTime < 1 / 300f) return 60;
            return 1.0f / deltaTime;
        }

        #region Record FPS

        public void StartRecordFPS()
        {
            sumFPS = 0;
            nSample = 0;
            isRecordingFPS = true;
        }

        public void StopRecordFPS()
        {
            isRecordingFPS = false;
        }

        public void ResumeRecordFPS()
        {
            isRecordingFPS = true;
        }

        public int GetAverageFPS()
        {
            return (int)(sumFPS / nSample);
        }

        #endregion
    }
}