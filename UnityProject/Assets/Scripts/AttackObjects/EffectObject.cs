using System.Collections.Generic;
using UnityEngine;

public class EffectObject : MonoBehaviour
{
    //��Ƽ�� �׷�
    private int partitionGroup = 0;
    public float hitBoxRadius;          //��Ʈ�ڽ� ũ��
    private int partitionCheckRadius;   //�ֺ� ��Ƽ�� üũ ����
    //�ֺ� ��Ƽ�� �׷�
    List<int> surroundPartitionGroup = new List<int>();
    private float durationTime = 0f;
    //������Ʈ�� ���ӽð� (�ش� �ð� ��� �� ����)
    public float durationTimeSet = 1f;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
