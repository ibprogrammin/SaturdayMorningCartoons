using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControls : MonoBehaviour {

    private bool DebugEnabled = false;

    protected KeyCode moveUp;
    protected KeyCode moveDown;

    protected LookDirection mLookDirection = LookDirection.LEFT;
    protected CharacterState mCharacterState = CharacterState.IDLE;

    protected Vector2 mLastPosition;
    public float speed = 0.3F;

    protected BoxCollider2D characterCollider;
    protected Animator characterAnimator;
    protected Transform characterTransform;

    protected float minY = 120.0F;
    protected float maxY = 205.0F;

    protected float minX = 10.0F;
    protected float maxX = 730.0F;

    public bool startRecording = false;
    protected List<FrameRecording> characterRecording = new List<FrameRecording>();

	// Use this for initialization
	void Start () {
        characterCollider = GetComponent("BoxCollider2D") as BoxCollider2D;
        characterAnimator  = GetComponent("Animator") as Animator;
        characterTransform = GetComponent("Transform") as Transform;
	}
	
	// Update is called once per frame
	void Update () {
        HandleInput();
        ScaleCharacter(characterTransform.position.y);
        AnimateCharacter();

        if (startRecording)
        {
            RecordInput();
        }
	}

    void HandleInput()
    {
        if (Input.touchCount > 0)
        {
            int verifiedTouches = 0;

            foreach (Touch touch in Input.touches)
            {
                // Check if touch was within the Collider Box
                if (characterCollider.bounds.Contains(touch.position))
                {
                    if (touch.phase == TouchPhase.Moved)
                    {
                        Move(touch);
                        Turn(touch);                        
                    }

                    if (touch.phase == TouchPhase.Stationary)
                    {
                        if (mCharacterState != CharacterState.IDLE) mCharacterState = CharacterState.IDLE;
                    }

                    verifiedTouches++;

                    // Handle Jumping
                    if (verifiedTouches >= 2) // BUG: Might be a bug with my screen, should be 2
                    {
                        mCharacterState = CharacterState.JUMPING;
                    }
                }

                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    if (mCharacterState != CharacterState.IDLE) mCharacterState = CharacterState.IDLE;
                }

                if (DebugEnabled) Debug.LogFormat("Finger Id: {0}", touch.fingerId);
            }
                        
        }
        else
        {
            if (mCharacterState != CharacterState.IDLE) mCharacterState = CharacterState.IDLE;
        }

        if (DebugEnabled) Debug.LogFormat("Touch Count: {0}", Input.touchCount);
    }

    private void Move(Touch touch)
    {
        // Get movement of the finger since last frame
        Vector2 touchDeltaPosition = touch.deltaPosition;

        // Move object across XY plane
        float moveX = 0.0F;
        float moveY = 0.0F;

        if ((transform.position.y + touchDeltaPosition.y) < maxY && (transform.position.y + touchDeltaPosition.y) > minY)
        {
            moveY = touchDeltaPosition.y;
        }

        if ((transform.position.x + touchDeltaPosition.x) < maxX && (transform.position.x + touchDeltaPosition.x) > minX)
        {
            moveX = touchDeltaPosition.x;
        }

        transform.Translate(moveX, moveY, 0);

        if (moveX == 0.0F && moveY == 0.0F)
        {

        }
        else
        {
            mCharacterState = CharacterState.RUNNING;
        }
    }

    private void Turn(Touch touch)
    {
        switch (mLookDirection)
        {
            case LookDirection.LEFT:
                if (touch.position.x > mLastPosition.x)
                {
                    TurnCharacter();
                }
                break;
            case LookDirection.RIGHT:
                if (touch.position.x < mLastPosition.x)
                {
                    TurnCharacter();
                }
                break;
        }

        mLastPosition = touch.position;
    }

    private bool CheckTouchInRange(float touchPosition, float currentPosition, float range)
    {
        if (touchPosition <= currentPosition + range && touchPosition >= currentPosition - range)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SetDirection()
    {
        int randomDirection = new System.Random().Next(1);

        switch (randomDirection)
        {
            case 0:
                mLookDirection = LookDirection.LEFT;
                break;
            case 1:
                mLookDirection = LookDirection.RIGHT;
                break;
            default:
                mLookDirection = LookDirection.RIGHT;
                break;
        }
    }

    private void TurnCharacter()
    {
        switch (mLookDirection)
        {
            case LookDirection.LEFT:
                mLookDirection = LookDirection.RIGHT;
                //transform.localScale = new Vector3(-20.0F, 20.0F, 1.0F);
                break;
            case LookDirection.RIGHT:
                mLookDirection = LookDirection.LEFT;
                //transform.localScale = new Vector3(20.0F, 20.0F, 1.0F);
                break;
            default:
                break;
        }

        if (DebugEnabled)
        {
            Debug.LogFormat("Direction: {0} / X Scale: {1}", mLookDirection.ToString(), characterTransform.localScale.x);
        }
    }

    private void ScaleCharacter(float yPosition)
    {
        

        float range = maxY - minY;

        float difference = yPosition - minY;
        float scaleGrowth = 0.4F;
        float multiplier = (1.0F - scaleGrowth) + (((range - difference) * 0.01F) * scaleGrowth);

        switch (mLookDirection)
        {
            case LookDirection.LEFT:
                transform.localScale = new Vector3(20.0F * multiplier, 20.0F * multiplier, 1.0F);
                break;
            case LookDirection.RIGHT:
                transform.localScale = new Vector3(-20.0F * multiplier, 20.0F * multiplier, 1.0F);
                break;
            default:
                break;
        }  
    }

    private void AnimateCharacter()
    {
        switch (mCharacterState)
        {
            case CharacterState.IDLE:
                characterAnimator.SetTrigger("Stopping");
                break;
            case CharacterState.RUNNING:
                characterAnimator.SetTrigger("Walking");
                break;
            case CharacterState.JUMPING:
                characterAnimator.SetTrigger("Jumping");
                break;
            default:
                characterAnimator.SetTrigger("Stopping");
                break;
        }

        if (DebugEnabled)
        {
            Debug.LogFormat("Character State: {0}", mCharacterState.ToString());
        }
    }

    public void ResetRecording()
    {
        characterRecording.Clear();
    }

    private void RecordInput()
    {
        Vector2 position = new Vector2(transform.position.x, transform.position.y);
        Vector2 scale = new Vector2(transform.localScale.x, transform.localScale.y);

        FrameRecording CurrentFrame = new FrameRecording(position, scale, mCharacterState);

        characterRecording.Add(CurrentFrame);
    }

    public AudioSource SourceAudio;
    public float characterPitch = 0.8F;

    public void StartRecordAudio()
    {
        SourceAudio = GetComponent<AudioSource>();
        SourceAudio.clip = Microphone.Start("", false, 30, 44100);
    }

    public void StopRecordAudio()
    {
        if (Microphone.IsRecording("")) Microphone.End("");
    }

    private void PlayRecordAudio()
    {
        SourceAudio.pitch = characterPitch;
        SourceAudio.Play();
    }

    public IEnumerator PlaybackRecording()
    {
        for (int i = 0; i < characterRecording.Count; i++)
        {
            if (i == 0) PlayRecordAudio();

            transform.position = new Vector3(characterRecording[i].Position.x, characterRecording[i].Position.y, 10.0F);
            transform.localScale = new Vector3(characterRecording[i].Scale.x, characterRecording[i].Scale.y, 1.0F);
            mCharacterState = characterRecording[i].State;
            AnimateCharacter();

            yield return null;
        }
    }

    public void CopyMe()
    {
        Instantiate(this);

        transform.position = new Vector3(transform.position.x + 35.0F, transform.position.y, 0.0F);
    }

}

public struct FrameRecording
{
    public Vector2 Position;
    public Vector2 Scale;
    public CharacterState State;

    public FrameRecording(Vector2 position, Vector2 scale, CharacterState state) {
        Position = position;
        Scale = scale;
        State = state;
    }

}

public enum LookDirection
{
    UP = 0x01,
    DOWN = 0x02,
    LEFT = 0x03,
    RIGHT = 0x04
};

public enum CharacterState
{
    IDLE = 0x01,
    RUNNING = 0x02,
    JUMPING = 0x03
}