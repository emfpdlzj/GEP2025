using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class WaveCounter : MonoBehaviour
{
    TextMeshProUGUI txt;
    public EnemySpawnManager spawner;
    void Start()
    {
        txt = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        txt.text = "Wave : " + spawner.GetWave();
    }
}
