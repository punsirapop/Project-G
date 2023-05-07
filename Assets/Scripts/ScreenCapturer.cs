using UnityEngine;

public class ScreenCapturer : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            string fileName = "Screenshot/screenshot_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
            ScreenCapture.CaptureScreenshot(fileName);
            Debug.Log("Screenshot saved as " + fileName);
        }
    }
}