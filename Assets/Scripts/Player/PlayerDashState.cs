using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    

    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.stats.MakeInvincible(true);
        if (player.IsGroundDetected())
            player.fx.CreateDustParticles(DustParticleType.Running);

           
        

        player.skill.dash.CloneOnDash();
        stateTimer = player.dashDuration;
    }

    public override void Exit()
    {
        player.stats.MakeInvincible(false);
        player.SetVelocity(0, rb.velocity.y);
        player.skill.dash.CloneOnArrival();
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        
        CreateTrailAfterImage();

        if (!player.IsGroundDetected() && player.IsWallDetected())
            stateMachine.ChangeState(player.wallSlideState);

        player.SetVelocity(player.dashSpeed * player.dashDir, 0);

        if (stateTimer < 0)
            stateMachine.ChangeState(player.idleState);
    }

    
}
