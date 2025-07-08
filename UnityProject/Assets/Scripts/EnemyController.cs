using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 2f;
    private float originalSpeed;
    Vector3 moveDirection = Vector3.zero;

    private bool isSlow = false;                //�̵��ӵ� ���� ����� ����
    private float slowTime = 0;                 //�̵��ӵ� ���� �ð� ��꺯��
    private float slowTimeSet = 0;
    
    private int batchId;
    public int BatchID
    {
        get { return batchId; }
        set { batchId = value; }
    }
    public int partitionGroup = 0;

    public int damage = 2;

    //�������� ����
    public bool isBoss = false;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        //�⺻ �̵��ӵ� ����
        originalSpeed = moveSpeed;
    }

    private void FixedUpdate()
    {
        // ����� ����
        if (isSlow)
        {
            slowTime += Time.deltaTime;
            if (slowTime >= slowTimeSet)
            {
                isSlow = false;
                moveSpeed = originalSpeed;
                slowTime = 0;
            }
        }

        //�÷��̾� ��ġ�� �̵�
        moveDirection = GameManager.instance.player.transform.position - transform.position;
        moveDirection.Normalize();
        transform.position += moveDirection * Time.deltaTime * moveSpeed;

        
        //�̵� ���⿡ ���� �̹��� Flip
        if (moveDirection.x < 0)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;

        //��Ƽ�� �׷� �ֽ�ȭ
        int newPartitionGroup = GameManager.instance.GetPartitionGroup(transform.position.x, transform.position.y);
        if(newPartitionGroup != partitionGroup)
        {
            GameManager.instance.enemyGroups[partitionGroup].Remove(this);
            partitionGroup = newPartitionGroup;
            GameManager.instance.enemyGroups[partitionGroup].Add(this);
        }
    }

    //Batch���� ������ ���� (1�ʴ� �ѹ�)
    public void RunLogic()
    {
        PushOtherEnemy();
    }


    //�ֺ� �ٸ� ���� �Ÿ� ����

    private void PushOtherEnemy()
    {

        List<EnemyController> currAreaEnemies = GameManager.instance.enemyGroups[partitionGroup].ToList(); // ONLY enemies in the same spatial group

        // ���� ��Ƽ�� �� �ٸ� ���� 0.2 �Ÿ� �ȿ� ������ �о
        foreach (EnemyController enemy in currAreaEnemies)
        {
            if (enemy == null) continue;
            if (enemy == this) continue;

            float distance = Mathf.Abs(transform.position.x - enemy.transform.position.x) + Mathf.Abs(transform.position.y - enemy.transform.position.y);
            if (distance < 0.2f)
            {
                Vector3 direction = transform.position - enemy.transform.position;
                direction.Normalize();
                enemy.transform.position -= direction * Time.deltaTime * moveSpeed * 5;
            }
        }
    }


    public void KillEnemy()
    {
        GameManager.instance.UpdateBatchOnDeath("enemy", batchId);
        GameManager.instance.enemyGroups[partitionGroup].Remove(this);
        Destroy(gameObject);
    }

    
    public void TakeEffect(int type, float dur, float val)
    {
        switch (type)
        {
            case 0:
                break;
            //�̵��ӵ� * val
            case 1:
                slowTimeSet = dur;
                slowTime = 0;
                moveSpeed = originalSpeed * val;
                Debug.Log(moveSpeed);
                isSlow = true;
                break;
            case 2:
                slowTimeSet = dur;
                slowTime = 0;
                moveSpeed = 0;
                isSlow = true;
                break;
        }
    }
}
