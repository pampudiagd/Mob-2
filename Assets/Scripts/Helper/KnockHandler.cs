using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class KnockHandler : MonoBehaviour
{
    private Rigidbody2D rb;
    private Enemy_Base enemy;

    [SerializeField] private float knockbackSpeed = GlobalConstants.knockbackSpeed;
    [SerializeField] private float knockDistance = GlobalConstants.knockMagnitudeModifier;

    public event Action OnKnockbackStarted;
    public event Action OnKnockbackEnded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy = GetComponent<Enemy_Base>();
    }

    public IEnumerator StartKnockback(Vector2 sourcePos)
    {
        yield return StartCoroutine(KnockbackCoroutine(sourcePos));
    }

    private IEnumerator KnockbackCoroutine(Vector2 sourcePos)
    {
        OnKnockbackStarted?.Invoke();

        if (enemy != null)
           enemy.SetState(EnemyState.Knockback);
        Debug.Log("Knocking back!");
        Vector2 knockDirection = (rb.position - sourcePos).normalized;
        float remainingDistance = knockDistance;
        //int knockCount = 0;

        while (remainingDistance > 0.01f)
        {
            // Step size per frame
            float step = knockbackSpeed * Time.fixedDeltaTime;
            if (step > remainingDistance)
                step = remainingDistance;

            // Look just ahead of the step
            RaycastHit2D hit = Physics2D.Raycast(rb.position, knockDirection, step, LayerMask.GetMask("Environment-Solid"));
            Debug.DrawRay(rb.position, knockDirection * step, Color.green, 0.1f);

            if (hit.collider != null)
            {
                // Found a wall
                Vector2 wallNormal = hit.normal;
                float angle = Vector2.Angle(knockDirection, -wallNormal);

                if (angle <= 60f) // pretty head-on
                {
                    // Stop dead
                    rb.position = hit.point;
                    break;
                }
                else
                {
                    // Slide along wall tangent
                    knockDirection = Vector2.Perpendicular(wallNormal);

                    // Pick the tangent that matches original motion
                    if (Vector2.Dot(knockDirection, rb.position - sourcePos) < 0)
                        knockDirection = -knockDirection;

                    // Move to just before the wall contact
                    rb.position = hit.point - knockDirection * 0.01f;
                }
            }
            else
            {
                // Unadjusted knockback movement
                rb.MovePosition(rb.position + knockDirection * step);
                remainingDistance -= step;
            }
            //knockCount++;
            //Debug.Log("knockback loops: " + knockCount);
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(0.2f);

        if (enemy != null)
        {
            enemy.SetState(EnemyState.Default);
        }

        Debug.Log("Knockback Ended");
        OnKnockbackEnded?.Invoke();

    }
}
