using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : IDisposable
{
    private readonly InputSystem_Actions _inputActions;
    private readonly Action<Vector2> _onMove;
    private readonly Action<InputAction.CallbackContext> _onJump;
    private readonly Action<bool> _onSprint;
    private readonly Action<InputAction.CallbackContext> _sprintHandler;
    private readonly Action<InputAction.CallbackContext> _onMoveHandler;
    private readonly Action<InputAction.CallbackContext> _onMoveCanceledHandler;

    public PlayerInputHandler(Action<Vector2> onMove, Action<InputAction.CallbackContext> onJump, Action<bool> onSprint)
    {
        _inputActions = new InputSystem_Actions();
        _onMove = onMove;
        _onJump = onJump;
        _onSprint = onSprint;
        _sprintHandler = ctx => _onSprint(ctx.ReadValue<float>() > 0.5f);
        _onMoveHandler = ctx => _onMove(ctx.ReadValue<Vector2>());
        _onMoveCanceledHandler = ctx => _onMove(Vector3.zero);
        SetupInputs();
    }
    
    private void SetupInputs()
    {
        _inputActions.Player.Move.Enable();
        _inputActions.Player.Jump.Enable();
        _inputActions.Player.Sprint.Enable();
        
        _inputActions.Player.Jump.performed += _onJump;
        _inputActions.Player.Sprint.performed += _sprintHandler;
        _inputActions.Player.Move.performed += _onMoveHandler;
        _inputActions.Player.Move.canceled += _onMoveCanceledHandler;
    }
    
    public Vector2 GetMoveInput() => _inputActions.Player.Move.ReadValue<Vector2>();
    
    public void Dispose()
    {
        _inputActions.Player.Move.Disable();
        _inputActions.Player.Jump.Disable();
        _inputActions.Player.Sprint.Disable();
        
        _inputActions.Player.Jump.performed -= _onJump;
        _inputActions.Player.Sprint.performed -= _sprintHandler;
        _inputActions.Player.Move.performed -= _onMoveHandler;
        _inputActions.Player.Move.canceled -= _onMoveCanceledHandler;
        
        _inputActions.Dispose();
    }
}