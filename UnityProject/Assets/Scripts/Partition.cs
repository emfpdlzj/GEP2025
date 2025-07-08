using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Partition : MonoBehaviour
{
    //radius칸 만큼 떨어진 주변 Partition그룹 List 반환
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
                //dx, dy 만큼 떨어져있는 파티션 그룹
                int newGroup = spatialGroup + dx + dy * widthRange;

                //해당 파티션 그룹의 가장자리 여부 확인 (범위 벗어남 여부)
                bool isWithinWidth = newGroup % widthRange >= 0 && newGroup % widthRange < widthRange;
                bool isWithinHeight = newGroup / widthRange >= 0 && newGroup / widthRange < heightRange;
                bool isWithinBounds = isWithinWidth && isWithinHeight;

                //해당 파티션 그룹이 파티션 범위 (0~10000) 내에 있는지 확인
                bool isWithinPartitions = newGroup >= 0 && newGroup < numberOfPartitions;

                if (isWithinBounds && isWithinPartitions)
                {
                    expandedSpatialGroups.Add(newGroup);
                }
            }
        }

        //중복 제거 후 반환
        return expandedSpatialGroups.Distinct().ToList();
    }

    //매개변수로 받은 파티션 그룹에 있는 모든 적 오브젝트 List 반환
    public static List<EnemyController> GetAllEnemiesInPartitionGroups(List<int> partitionGroups)
    {
        List<EnemyController> enemies = new List<EnemyController>();

        foreach (int partitionGroup in partitionGroups)
            enemies.AddRange(GameManager.instance.enemyGroups[partitionGroup]);

        return enemies;
    }
}
