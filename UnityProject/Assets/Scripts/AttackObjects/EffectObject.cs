using System.Collections.Generic;
using UnityEngine;

public class EffectObject : MonoBehaviour
{
    //파티션 그룹
    private int partitionGroup = 0;
    public float hitBoxRadius;          //히트박스 크기
    private int partitionCheckRadius;   //주변 파티션 체크 범위
    //주변 파티션 그룹
    List<int> surroundPartitionGroup = new List<int>();
    private float durationTime = 0f;
    //오브젝트의 지속시간 (해당 시간 경과 후 삭제)
    public float durationTimeSet = 1f;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
