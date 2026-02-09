using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behavior_Pursuit_Simple : Behavior_Base
{
    private Rigidbody2D rb;

    public Coroutine moveRoutine;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public IEnumerator MoveToTileTargeting(Vector3 targetWorldPos, System.Func<bool> interruptCondition = null, float moveSpeed = 1f)
    {
        int moveAttempts = 0;
        while ((transform.position - targetWorldPos).sqrMagnitude > 0.001f)
        {
            if ((interruptCondition != null && interruptCondition()) || moveAttempts >= 50)
            {
                moveRoutine = null;
                print("Failed targetting move step");
                yield break; // stop movement early
            }

            Vector2 newPos = Vector2.MoveTowards(rb.position, targetWorldPos, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);

            moveAttempts++;
            yield return new WaitForFixedUpdate();
        }

        //rb.MovePosition(LevelManager.Instance.LevelTilemap.LocalToWorld(target)); // Snap to exact center

        yield return null;
        print("Finished step");
        moveRoutine = null;
    }
}
