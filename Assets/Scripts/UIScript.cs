using TMPro;
using UnityEngine;

public class UIScript : MonoBehaviour {
    public WaveControllerScript waveControllerScript;
    public TMP_Text tmpWave, tmpSeconds, tmpMillis, tmpScore;

    void Update() {
        tmpWave.text = waveControllerScript.waveCount.ToString();
        float time = waveControllerScript.timeLeftInWave;
        int seconds = Mathf.FloorToInt(time);
        tmpSeconds.text = seconds.ToString();
        string monospaceTag = tmpMillis.text.Split('>')[0] + '>';
        tmpMillis.text = monospaceTag + Mathf.FloorToInt((time - seconds) * 1000).ToString().PadLeft(3, '0');
        tmpScore.text = waveControllerScript.score.ToString();
    }
}
