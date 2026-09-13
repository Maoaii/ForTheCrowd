using CarGame.Systems.Blackboards;
using CarGame.Systems;

using System.Runtime.Serialization;

namespace CarGame.Entities.Player;

public enum PlayerStates {
    [EnumMember(Value = "Idle")]
    IDLE_STATE,

    [EnumMember(Value = "Moving")]
    MOVING_STATE,

    [EnumMember(Value = "Air")]
    AIR_STATE,

    [EnumMember(Value = "Ollie Jack")]
    OLLIE_STATE,

    [EnumMember(Value = "Drift Shove")]
    SHOVE_IT_STATE,

    [EnumMember(Value = "Barrel Roll")]
    KICKFLIP_STATE,

    [EnumMember(Value = "Wheelie")]
    BACK_MANUAL_STATE
}


public class PlayerStateMachine : StateMachine 
{
    public PlayerStateMachine(Blackboard blackboard) : base(blackboard) { }
}