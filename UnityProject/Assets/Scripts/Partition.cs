using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Partition : MonoBehaviour
{
    //radiusĭ ��ŭ ������ �ֺ� Partition�׷� List ��ȯ
    public static List<int> GetExpandedPartitionGroups(int spatialGroup, int radius = 1)
    {
        List<int> expandedSpatialGroups = new List<int>();

        int widthRange = 100;
        int heightRange = 100;
        int numberOfPartitions = 10000;

        for (int dx = -radius; dx <= radius; dx++)
        {
            for (int dy = -radius; dy <= radius; dy++)
            {
                //dx, dy ��ŭ �������ִ� ��Ƽ�� �׷�
                int newGroup = spatialGroup + dx + dy * widthRange;

                //�ش� ��Ƽ�� �׷��� �����ڸ� ���� Ȯ�� (���� ��� ����)
                bool isWithinWidth = newGroup % widthRange >= 0 && newGroup % widthRange < widthRange;
                bool isWithinHeight = newGroup / widthRange >= 0 && newGroup / widthRange < heightRange;
                bool isWithinBounds = isWithinWidth && isWithinHeight;

                //�ش� ��Ƽ�� �׷��� ��Ƽ�� ���� (0~10000) ���� �ִ��� Ȯ��
                bool isWithinPartitions = newGroup >= 0 && newGroup < numberOfPartitions;

                if (isWithinBounds && isWithinPartitions)
                {
                    expandedSpatialGroups.Add(newGroup);
                }
            }
        }

        //�ߺ� ���� �� ��ȯ
        return expandedSpatialGroups.Distinct().ToList();
    }

    //�Ű������� ���� ��Ƽ�� �׷쿡 �ִ� ��� �� ������Ʈ List ��ȯ
    public static List<EnemyController> GetAllEnemiesInPartitionGroups(List<int> partitionGroups)
    {
        List<EnemyController> enemies = new List<EnemyController>();

        foreach (int partitionGroup in partitionGroups)
            enemies.AddRange(GameManager.instance.enemyGroups[partitionGroup]);

        return enemies;
    }
}
