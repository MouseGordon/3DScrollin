namespace Player.Gravity{
    public interface IGravityData{
        float GravityForce{ get; }
        float TerminalVelocity{ get; }
        float GroundedGravity{ get; }
    }
}