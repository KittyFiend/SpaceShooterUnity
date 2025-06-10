using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyPosition
{
    public GameObject enemy;
    public Vector2 targetPosition;
    public bool isMovingToPosition = true;

    public EnemyPosition(GameObject enemy, Vector2 position)
    {
        this.enemy = enemy;
        this.targetPosition = position;
        this.isMovingToPosition = true;
    }
}

public class FormationManager : MonoBehaviour
{
    [Header("Formation Settings")]
    public int enemiesPerRow = 5;
    public int numberOfRows = 3;
    public float spacingX = 1.5f;
    public float spacingY = 1f;
    public Vector2 formationCenter = new Vector2(0, 2f);

    [Header("Movement")]
    public float horizontalSpeed = 1f;
    public float movementRange = 3f;
    public float moveToFormationSpeed = 4f;

    [Header("Enemy Prefabs")]
    public GameObject[] enemyPrefabs;

    private List<EnemyPosition> enemyPositions = new List<EnemyPosition>();
    private float globalCircleTime = 0f;

    private void Start()
    {
        StartCoroutine(SpawnFormation());
    }

    private IEnumerator SpawnFormation()
    {
        yield return new WaitForSeconds(1f);

        for (int row = 0; row < numberOfRows; row++)
        {
            for (int col = 0; col < enemiesPerRow; col++)
            {
                SpawnEnemyInFormation(row, col);
                yield return new WaitForSeconds(0.1f);
            }
        }

        StartCoroutine(ManageAllEnemies());
    }

    private void SpawnEnemyInFormation(int row, int col)
    {
        if (enemyPrefabs.Length == 0) return;

        float offsetX = (col - (enemiesPerRow - 1) * 0.5f) * spacingX;
        float offsetY = -row * spacingY;
        Vector2 targetPos = formationCenter + new Vector2(offsetX, offsetY);

        Vector2 spawnPos = new Vector2(
            targetPos.x + Random.Range(-1f, 1f),
            8f + row * 1f + col * 0.5f
        );

        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.SetMovementPattern(EnemyMovementPattern.Formation);
        }

        ShootingEnemy shootingEnemyScript = enemy.GetComponent<ShootingEnemy>();
        if (shootingEnemyScript != null)
        {
            shootingEnemyScript.SetMovementPattern(EnemyMovementPattern.Formation);
        }

        enemyPositions.Add(new EnemyPosition(enemy, targetPos));
    }

    private IEnumerator ManageAllEnemies()
    {
        while (enemyPositions.Count > 0)
        {
            enemyPositions.RemoveAll(ep => ep.enemy == null);

            if (enemyPositions.Count == 0) break;

            globalCircleTime += Time.deltaTime * 0.8f;

            foreach (var enemyPos in enemyPositions)
            {
                if (enemyPos.enemy != null)
                {
                    ProcessEnemyMovement(enemyPos);
                }
            }

            yield return null;
        }
    }

    private void ProcessEnemyMovement(EnemyPosition enemyPos)
    {
        Vector2 circleOffset = new Vector2(
            Mathf.Sin(globalCircleTime) * 0.5f,
            Mathf.Cos(globalCircleTime) * 0.2f
        );

        Vector2 targetWithCircle = enemyPos.targetPosition + circleOffset;

        if (enemyPos.isMovingToPosition)
        {
            float distanceToTarget = Vector2.Distance(enemyPos.enemy.transform.position, enemyPos.targetPosition);

            if (distanceToTarget > 0.3f)
            {
                Vector2 moveDirection = (targetWithCircle - (Vector2)enemyPos.enemy.transform.position).normalized;
                enemyPos.enemy.transform.position = Vector2.MoveTowards(
                    enemyPos.enemy.transform.position,
                    targetWithCircle,
                    moveToFormationSpeed * Time.deltaTime
                );
            }
            else
            {
                enemyPos.isMovingToPosition = false;
            }
        }
        else
        {
            enemyPos.enemy.transform.position = targetWithCircle;
        }
    }

    public int GetAliveEnemiesCount()
    {
        enemyPositions.RemoveAll(ep => ep.enemy == null);
        return enemyPositions.Count;
    }
}