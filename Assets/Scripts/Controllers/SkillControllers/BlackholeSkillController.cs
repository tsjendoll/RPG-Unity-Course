using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlackholeSkillController : MonoBehaviour
{
    [SerializeField] private GameObject hotkeyPrefab;
    [SerializeField] private List<KeyCode> keyCodeList;

    private float maxSize;
    private float growSpeed;
    private float shrinkSpeed;
    private float blackholeTimer;

    private bool canGrow = true;
    private bool canShrink;

    private bool canCreateHotkeys = true;
    private bool cloneAttackInitiated;
    private int amountOfAttacks = 4;
    private float cloneAttackCooldown = .3f;
    private float cloneAttackTimer;

    private List<Transform> targets = new List<Transform>();
    private List<GameObject> createdHotkeys = new List<GameObject>();

    public bool playerCanExitState { get; private set; }
    private bool playerCanDisappear = true;

    public void SetupBlackhole(float _maxSize, float _growSpeed, float _shrinkSpeed, int _amountOfAttacks, float _cloneAttackCooldown, float _blackholeDuration)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        amountOfAttacks = _amountOfAttacks;
        cloneAttackCooldown = _cloneAttackCooldown;
        blackholeTimer = _blackholeDuration;

        if(SkillManager.instance.clone.CanSpawnCrystal())
            playerCanDisappear = false;
    }

    private void Update()
    {
        cloneAttackTimer -= Time.deltaTime;
        blackholeTimer -= Time.deltaTime;

        if (blackholeTimer < 0)
        {
            blackholeTimer = Mathf.Infinity;
            ReleaseCloneAttack();
    
        }

        if (Input.GetKeyDown(KeyCode.R))
            ReleaseCloneAttack();
            

        CloneAttackLogic();

        if (canGrow && !canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }

        if (canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime);

            if (transform.localScale.x < 0)
                Destroy(gameObject);
        }
    }

    private void ReleaseCloneAttack()
    {
        if (playerCanDisappear)
            PlayerManager.instance.player.fx.MakeTransparent(true);
        cloneAttackInitiated = true;
        canCreateHotkeys = false;
        DestroyHotHeys();
    }

    private void CloneAttackLogic()
    {
        if (cloneAttackTimer < 0 && cloneAttackInitiated && amountOfAttacks > 0)
        {
            cloneAttackTimer = cloneAttackCooldown;

            int randomIndex = Random.Range(0, targets.Count);

            float xOffset;

            if (Random.Range(0, 100) < 50)
                xOffset = 1;
            else
                xOffset = -1;

            if (targets.Count > 0)
            {
                SkillManager.instance.clone.CreateClone(targets[randomIndex], new Vector3(xOffset, 0));
                amountOfAttacks--;
            }

            if (amountOfAttacks <= 0 || targets.Count <= 0)
            {
                Invoke("FinishBlackholeAbility", 1f);
            }
        }
    }

    private void FinishBlackholeAbility()
    {
        playerCanExitState = true;
        PlayerManager.instance.player.fx.MakeTransparent(false);
        canShrink = true;
        cloneAttackInitiated = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Enemy>() != null)
        {
            // targets.Add(collision.transform);
            collision.GetComponent<Enemy>().FreezeTime(true);
            CreateHoykey(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.GetComponent<Enemy>() != null)
            // targets.Add(collision.transform);
            collision.GetComponent<Enemy>().FreezeTime(false);
    }

    private void DestroyHotHeys()
    {
        if(createdHotkeys.Count >0)
        {
            foreach (var hotkey in createdHotkeys)
            {
                Destroy(hotkey);
            }
        }
    }
    private void CreateHoykey(Collider2D collision)
    {
        if (keyCodeList.Count <= 0 || !canCreateHotkeys)
            return;

        Vector3 proposedPosition = collision.transform.position + new Vector3(0, 2);
        proposedPosition = GetValidPosition(proposedPosition);

        GameObject newHotkey = Instantiate(hotkeyPrefab, proposedPosition, Quaternion.identity);
        createdHotkeys.Add(newHotkey);

        KeyCode chosenKey = keyCodeList[Random.Range(0, keyCodeList.Count)];
        keyCodeList.Remove(chosenKey);

        BlackholeHotkeyController newHotkeyScript = newHotkey.GetComponent<BlackholeHotkeyController>();
        newHotkeyScript.SetupHotkey(chosenKey, collision.transform, this);
    }

    private Vector3 GetValidPosition(Vector3 initialPosition)
    {
        Vector3 offset = new Vector3(1, 0, 0); // Adjust this offset as needed to position the hotkey next to the previous one.
        Vector3 checkPosition = initialPosition;

        while (IsPositionOccupied(checkPosition))
        {
            checkPosition += offset;
        }

        return checkPosition;
    }

    private bool IsPositionOccupied(Vector3 position)
    {
        foreach (GameObject hotkey in createdHotkeys)
        {
            if (Vector3.Distance(hotkey.transform.position, position) < 0.5f) // Adjust the distance threshold as needed.
            {
                return true;
            }
        }
        return false;
    }

    public void AddEnemyToList(Transform _enemyTransform) => targets.Add(_enemyTransform);
}
