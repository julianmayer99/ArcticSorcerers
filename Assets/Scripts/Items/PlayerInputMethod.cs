﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Items
{
    public class PlayerInputMethod
    {
        public PlayerInputMethod(PlayerController player, Gamepad pad)
        {
            this.player = player;
            this.gamepad = pad;

            isKeyboardControlled = pad == null;

            if (OnJumpButtonDown == null)
                OnJumpButtonDown = new UnityEvent();
            if (OnShootButtonDown == null)
                OnShootButtonDown = new UnityEvent();
            if (OnShootButtonUp == null)
                OnShootButtonUp = new UnityEvent();
            if (OnBackButtonUp == null)
                OnBackButtonUp = new UnityEvent();
            if (OnPauseButtonDown == null)
                OnPauseButtonDown = new UnityEvent();
            if (OnScoreboardButtonDown == null)
                OnScoreboardButtonDown = new UnityEvent();

        }
        public bool isKeyboardControlled { get; private set; } = false;
        public UnityEvent OnJumpButtonDown;
        public UnityEvent OnShootButtonDown;
        public UnityEvent OnShootButtonUp;
        public UnityEvent OnBackButtonUp;
        public UnityEvent OnPauseButtonDown;
        public UnityEvent OnScoreboardButtonDown;

        private PlayerController player;
        public Gamepad gamepad { get; private set; }

        public void RefreshInput()
        {
            RefreshJump();
            RefreshBack();
            RefreshShoot();
            RefreshPause();
            RefreshScoreboard();
        }

        private bool jumpWasDown = false;
        private void RefreshJump()
        {
            bool isdown;
            if (gamepad != null)
                isdown = gamepad.buttonSouth.isPressed;
            else
                isdown = Keyboard.current.spaceKey.isPressed;

            if (isdown && !jumpWasDown)
                OnJumpButtonDown.Invoke();
            jumpWasDown = isdown;
        }

        private bool shootWasDown = false;
        private void RefreshShoot()
        {
            bool isdown;
            if (gamepad != null)
                isdown = gamepad.buttonWest.isPressed;
            else
                isdown = Mouse.current.leftButton.isPressed;

            if (isdown && !shootWasDown)
                OnShootButtonDown.Invoke();
            else if (!isdown && shootWasDown)
                OnShootButtonUp.Invoke();

            shootWasDown = isdown;
        }

        private bool refreshWasDown = false;
        private void RefreshBack()
        {
            bool isdown;
            if (gamepad != null)
                isdown = gamepad.buttonEast.isPressed;
            else
                isdown = Keyboard.current.escapeKey.isPressed;

            if (!isdown && refreshWasDown)
                OnBackButtonUp.Invoke();
            refreshWasDown = isdown;
        }

        private bool pauseWasDown = false;
        private void RefreshPause()
        {
            bool isdown;
            if (gamepad != null)
                isdown = gamepad.startButton.isPressed;
            else
                isdown = Keyboard.current.escapeKey.isPressed;

            if (isdown && !pauseWasDown)
                OnPauseButtonDown.Invoke();
            pauseWasDown = isdown;
        }

        private bool scoreboardWasDown = false;
        private void RefreshScoreboard()
        {
            bool isdown;
            if (gamepad != null)
                isdown = gamepad.rightShoulder.isPressed;
            else
                isdown = Keyboard.current.tabKey.isPressed;

            if (isdown && !scoreboardWasDown)
                OnScoreboardButtonDown.Invoke();
            scoreboardWasDown = isdown;
        }

        Vector2 playerScreenPos;
        Vector2 aimDirection;

        public Vector2 AimDirection
        {
            get
            {
                if (gamepad != null)
                    return gamepad.leftStick.ReadValue();
                else
                {
                    playerScreenPos = Camera.main.WorldToScreenPoint(player.transform.position);
                    aimDirection = Mouse.current.position.ReadValue() - playerScreenPos;
                    aimDirection.Normalize();
                    return aimDirection;
                }
            }
        }

        public float Move
        {
            get
            {
                if (gamepad != null)
                    return gamepad.leftStick.ReadValue().x;
                else
                {
                    if (Keyboard.current.aKey.isPressed)
                        return -1f;
                    else if (Keyboard.current.dKey.isPressed)
                        return 1f;
                    else return 0f;
                }
            }
        }


    }
}