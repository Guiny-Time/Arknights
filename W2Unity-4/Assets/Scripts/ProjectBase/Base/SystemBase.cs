//存放各种enum类

/// <summary>
/// 特殊攻击的类型，自动触发/手动触发
/// </summary>
public enum Big_Attack_Type
{
    Auto,HandControl
}

/// <summary>
/// 角色站位朝向，上/下/左/右
/// </summary>
public enum Direction
{
    Up,Down,Left,Right,
}

/// <summary>
/// 角色站位类型，高台/地面
/// </summary>
public enum PlayerType
{
    HightGround,LowGround,
}

public enum SpawnState
{
    SPAWING,WAITING,COUNTING
}