using UnityEngine;
//버튼 클릭시 사운드 나게하는 함수. 
public class btnSound : MonoBehaviour
{

    public AudioSource clickSound;
    public void ClickSound()
    {
        clickSound.Play();
    }
}
