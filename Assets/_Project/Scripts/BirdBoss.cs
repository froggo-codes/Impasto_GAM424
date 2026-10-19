using Unity.VectorGraphics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

// Bird boss for the piano level. A state machine: the bird is always in exactly one state.
// Behaviour Loop: Circling -> FlyingToStart -> Telegraphing -> Swooping -> Nested -> Returning -> Circling.
public class BirdBoss : MonoBehaviour
{
    // Every state the bird can be in
    private enum BirdState
    {
        Circling,       // drifting in an oval at the top of the arena
        FlyingToStart,      //heading to where the next dive begins
        Telegraphing,       //flashing red to warn the player; target nest lights up
        Swooping,       // diving through the player's position into the nest
        Nested,     // sitting in the nest (the stomp window)
        Stunned,    // dazed after a stomp
        Returning,   // flying back up to the oval
        Calmed      // fight over: resting peacefully in the nest
    }

    // One authored dive. Fill these in on the Inspector's Attacks list.
    [System.Serializable]
    private class SwoopAttack
    {
        public Transform startPoint;        // where the dive begins
        public Transform nest;      // where the dive ends
        public float swoopDuration = 1.2f;      // seconds; lower = faster, tighter dive
    }

    [Header("Fight")] 
    [SerializeField] private bool startFightOnAwake = false;    // tick to skip the arena trigger while testing
    private bool fightStarted;      // false until the player enters the arena
    
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private SwoopAttack[] attacks;     //the bird picks one at random each time

    [Header("Circling")] 
    [SerializeField] private float circleWidth = 6f;        // how far left/right it drifts
    [SerializeField] private float circleHeight = 1f;       // how far up/down it drifts
    [SerializeField] private float circleSpeed = 1f;        // loop speed (radians/sec)
    [SerializeField] private float circleTime = 4f;     // seconds of drifting before an attack

    [Header("Attack")] 
    [SerializeField] private float flySpeed = 8f;       // speed when flying to start/returning
    [SerializeField] private float telegraphTime = 0.6f;        // length of the red warning flash
    [SerializeField] private float nestTime = 1.5f;     // how long it sits in the nest
    [SerializeField] private Color telegraphColor = Color.red;
    [SerializeField] private bool showNestHint = true;      // light up the target nest? (difficulty lever)
    [SerializeField] private Color nestHintColor = Color.yellow;

    [Header("Combat")] 
    [SerializeField] private float stunTime = 2f;    // how long it says dazed after a stomp
    [SerializeField] private float stompBounce = 12f;   // upward speed given to the player on a stomp
    [SerializeField] private float stompHeightMargin = 0.5f;    // how far above the bird's center counts as "on top"
    [SerializeField] private Color stunColor = Color.cyan;

    [Header("Phases")] 
    [SerializeField] private float speedUpPerHit = 0.15f;    // each stomp makes timing 15% shorter
    [SerializeField] private Color calmColor = Color.green;
    [SerializeField] private UnityEvent onCalmed;       // HOOK: fires once when the bird is calmed

    private int hitsTaken; // stomps landed so far

    private Health health;
    private DamageOnTouch damageOnTouch;

    // Components
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Color normalColor;

    // State Tracking. Starts in Returning so it flies smoothly into the oval.
    private BirdState state = BirdState.Returning;
    private float stateTimer;       //shared countdown; each state sets it on entry
    private Vector2 circleCenter;       //center of the oval = where the bird is placed in the scene
    private float circleAngle;      // current position around the oval

    // The dive currently in progress
    private SwoopAttack currentAttack;
    private Vector2 swoopStart;
    private Vector2 swoopControl;       // the curve's bend point (aimed so the curve hits the player)
    private Vector2 swoopEnd;
    private float swoopTimer;

    // Nest hint: remember the nest's color so we can put it back
    private SpriteRenderer nestRenderer;
    private Color nestNormalColor;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        normalColor = spriteRenderer.color;
        circleCenter = rb.position;
        fightStarted = startFightOnAwake;
        health = GetComponent<Health>();
        damageOnTouch = GetComponent<DamageOnTouch>();
    }
    
    // Runs only the current state's behaviour. Each method below is one state.
    void FixedUpdate()
    {
        //Do nothing until the player enters the arena
        if (!fightStarted) return;
        // Only dangerous while flying; safe to touch while resting, stunned, or calmed
        bool isResting = state == BirdState.Nested || state == BirdState.Stunned || state == BirdState.Calmed;
        damageOnTouch.enabled = !isResting;

        switch (state)
        {
            case BirdState.Circling: UpdateCircling(); break;
            case BirdState.FlyingToStart: UpdateFlyingToStart(); break;
            case BirdState.Telegraphing: UpdateTelegraphing(); break;
            case BirdState.Swooping: UpdateSwooping(); break;
            case BirdState.Nested: UpdateNested(); break;
            case BirdState.Stunned: UpdateStunned(); break;
            case BirdState.Returning: UpdateReturning(); break;
            case BirdState.Calmed: break;   // does nothing; the fight is over
        }
    }
    
    // CIRCLING: drift around the oval, then pick a random attack
    void UpdateCircling()
    {
        circleAngle += circleSpeed * Time.fixedDeltaTime;
        rb.MovePosition(GetCirclePoint());

        stateTimer -= Time.fixedDeltaTime;
        if (stateTimer <= 0f && attacks.Length > 0)
        {
            currentAttack = attacks[Random.Range(0, attacks.Length)];
            state = BirdState.FlyingToStart;
        }
    }
    
    // Point on the oval at the current angle. Cos gives x, Sin gives y.
    Vector2 GetCirclePoint()
    {
        float x = Mathf.Cos(circleAngle) * circleWidth;
        float y = Mathf.Sin(circleAngle) * circleHeight;
        return circleCenter + new Vector2(x, y);
    }
    
    // FLYING TO START: once there, begin the warning flash and light up the nest
    void UpdateFlyingToStart()
    {
        if (FlyTowards(currentAttack.startPoint.position))
        {
            spriteRenderer.color = telegraphColor;
            ShowNestHint(true);
            stateTimer = telegraphTime * GetPace();
            state = BirdState.Telegraphing;
        }
    }
    
    // TELEGRAPHING: wait out the flash, then lock onto the player and plan the dive
    void UpdateTelegraphing()
    {
        stateTimer -= Time.fixedDeltaTime;
        if (stateTimer > 0f) return;

        spriteRenderer.color = normalColor;
        
        // Lock the target NOW. Moving after the flash ends dodges the dive.
        swoopStart = rb.position;
        swoopEnd = currentAttack.nest.position;
        Vector2 target = player.position;
        
        // A Bezier curve only travels about halfway toward its control point.
        // So aim the control point PAST the player (roughly twice as far),
        // and the halfway mark of the dive lands exactly on them.
        swoopControl = 2f * target - 0.5f * (swoopStart + swoopEnd);

        swoopTimer = 0f;
        state = BirdState.Swooping;
    }
    
    // SWOOPING: follow the curve from start to nest over swoopDuration seconds
    void UpdateSwooping()
    {
        swoopTimer += Time.fixedDeltaTime;
        float t = Mathf.Clamp01(swoopTimer / (currentAttack.swoopDuration * GetPace()));  // 0 = start, 1 = nest
        rb.MovePosition(Bezier(swoopStart, swoopControl, swoopEnd, t));

        if (t >= 1f)
        {
            stateTimer = nestTime * GetPace();
            state = BirdState.Nested;
        }
    }
    
    // NESTED: sit still, then leave and turn the nest hint off
    void UpdateNested()
    {
        stateTimer -= Time.fixedDeltaTime;
        if (stateTimer > 0f) return;

        ShowNestHint(false);
        state = BirdState.Returning;
    }
    
    // STUNNED: dazed in the nest after a stomp, then recovers and flies off
    void UpdateStunned()
    {
        stateTimer -= Time.fixedDeltaTime;
        if (stateTimer > 0f) return;

        spriteRenderer.color = normalColor;
        ShowNestHint(false);
        state = BirdState.Returning;
    }
    
    // RETURNING: fly back to the oval, then start drifting again
    void UpdateReturning()
    {
        if (FlyTowards(GetCirclePoint()))
        {
            stateTimer = circleTime * GetPace();
            state = BirdState.Circling;
        }
    }
    
    //Quadratic Bezier curve: the bird's position is a MIX of three points.
    // t = 0: all start (a).    t = 0.5: 25% a, 50% control, 25% b.     t = 1: all nest (b).
    // The mix sliding from start -> control -> nest is what bends the path into a U.
    Vector2 Bezier(Vector2 a, Vector2 control, Vector2 b, float t)
    {
        float u = 1f - t;
        return u * u * a + 2f * u * t * control + t * t * b;
    }
    
    // Called by the arena trigger when the player walks in
    public void StartFight()
    {
        fightStarted = true;
    }
    
    // Called by Health's On Died after the final stomp. Ends the fight peacefully.
    public void Calm()
    {
        if (state == BirdState.Calmed) return;  // only once

        state = BirdState.Calmed;
        spriteRenderer.color = calmColor;
        ShowNestHint(false);
        onCalmed.Invoke();  // HOOK: The level reacts (sheet music, exit, song modde...)
    }
    
    // Moves one step toward target. Returns true once it has arrived.
    bool FlyTowards(Vector2 target)
    {
        Vector2 next = Vector2.MoveTowards(rb.position, target, flySpeed * Time.fixedDeltaTime);
        rb.MovePosition(next);
        return Vector2.Distance(next, target) < 0.5f;
    }
    
    // Tints the target nest (true) or restores its color (false). Save, change, restore.
    void ShowNestHint(bool show)
    {
        if (!showNestHint) return;

        if (show)
        {
            nestRenderer = currentAttack.nest.GetComponent<SpriteRenderer>();
            if (nestRenderer == null) return;   // nest has no sprite, so skip the hint
            nestNormalColor = nestRenderer.color;
            nestRenderer.color = nestHintColor;
        }
        else if (nestRenderer != null)
        {
            nestRenderer.color = nestNormalColor;
        }
    }
    
    // STOMP CHECK: landing on the bird while it rests in the nest stuns it
    void OnCollisionStay2D(Collision2D collision)
    {
        if (state != BirdState.Nested) return;

        PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
        if (playerMovement == null) return;
        
        // Must be on top of the bird, not touching it from the side
        bool playerIsAbove = collision.transform.position.y > transform.position.y + stompHeightMargin;
        if (!playerIsAbove) return;

        playerMovement.Bounce(stompBounce);
        
        // Only continue if the hit actually landed (Health returns true/false)
        bool hitLanded = health.TakeDamage(1);
        if (!hitLanded) return;

        hitsTaken++;
        
        //The final stomp calms the bird (Health's On Died -> Calm), so don't stun over it
        if (state == BirdState.Calmed) return;

        spriteRenderer.color = stunColor;
        stateTimer = stunTime;
        state = BirdState.Stunned;

    }
    
    // Timing multiplier: 1.0 at full health, shorter after each stomp (1.0 -> 0.85 -> 0.7)
    float GetPace()
    {
        return 1f - hitsTaken * speedUpPerHit;
    }
}
