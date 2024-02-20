using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private Animator leftPerson, rightPerson, leftOar, rightOar;
    [SerializeField] private float timePerRow;
    [SerializeField] private float animationBlendTime;
    private const int RowingBackwards = -1, RowingForwards = +1;
    private const int Left = -1, Right = +1;
    private Input.PlayerActions actions;
    private InputMemory leftInput;
    private InputMemory rightInput;
    private BoatPhysics boat;
    private Player player;
    private void Awake()
    {
        Input input = new Input();
        input.Enable();
        actions = input.Player;
        actions.Pause.performed += Pause;

        leftInput = new InputMemory(this, actions.LeftOar, leftPerson, leftOar);
        rightInput = new InputMemory(this, actions.RightOar, rightPerson, rightOar);
        
        boat = GetComponent<BoatPhysics>();
        player = GetComponent<Player>();
    }
    public void SetIdle()
    {
        leftInput.SetIdle();
        rightInput.SetIdle();
        player.StopSplashVFX();
    }
    private void FixedUpdate()
    {
        leftInput.FixedUpdate();
        rightInput.FixedUpdate();
        if (boat.IsFalling){
            player.StopSplashVFX();
        }
        else {
            player.PlaySplashVFX(rightInput.Active == 1, leftInput.Active == 1);

        }
        ApplyForceFromInput();
    }
    private void ApplyForceFromInput()
    {
        bool rowingIsPushingLeft = leftInput.Active == RowingBackwards || rightInput.Active == RowingForwards;
        bool rowingIsPushingRight = rightInput.Active == RowingBackwards || leftInput.Active == RowingForwards;
        if (rowingIsPushingLeft && rowingIsPushingRight)
        {
            bool isRowingForwards = leftInput.Active == RowingForwards;
            boat.RowStraight(isRowingForwards);
            PlayAudio();
        }
        else if (rowingIsPushingLeft)
        {
            boat.PushBoat(Left);
            PlayAudio();
        }
        else if (rowingIsPushingRight)
        {
            boat.PushBoat(Right);
            PlayAudio();
        }
    }
    private void Pause(InputAction.CallbackContext ctx)
    {
        if (GameManager.Instance.CurrentGameState != GameState.Playing)
        {
            return;
        }
        if (ctx.ReadValueAsButton())
        {
            GameManager.Instance.PauseGame();
        }
    }
    private void OnDisable()
    {
        actions.Pause.performed -= Pause;
    }
    private void PlayAudio()
    {
        AudioManager.Instance.PlayRowingAudio();
    }
    // NESTED CLASS START |-----------------------------------------------------------------------------------------------------------------------------
    private class InputMemory
    {    
        public int Active;
        
        private static readonly int Direction = Animator.StringToHash("Direction");
        private readonly PlayerInput player;
        private readonly InputAction input;
        private readonly Animator person, oar;
        private float timer;
        private float currentValue;
        public InputMemory(PlayerInput playerInput, InputAction inputAction, Animator personAnimator, Animator oarAnimator)
        {
            player = playerInput;
            input = inputAction;
            person = personAnimator;
            oar = oarAnimator;
        }
        public void SetIdle()
        {
            UpdateCurrentValue(0);
        }
        public void FixedUpdate()
        {
            int inputValue = (int)input.ReadValue<float>();
            if (GameManager.Instance.CurrentGameState != GameState.Playing)
            {
                inputValue = 0;
            }
            if (inputValue != 0 && inputValue != Active)
            {
                Active = inputValue;
                timer = player.timePerRow;
            }
            timer -= Time.fixedDeltaTime;
            if (timer < 0 && inputValue == 0)
            {
                Active = 0;
            }
            float newValue = Mathf.MoveTowards(currentValue, Active, Time.fixedDeltaTime / player.animationBlendTime);
            UpdateCurrentValue(newValue);
        }
        private void UpdateCurrentValue(float newValue)
        {
            currentValue = newValue;
            person.SetFloat(Direction, currentValue);
            oar.SetFloat(Direction, currentValue);
        }
    }
}