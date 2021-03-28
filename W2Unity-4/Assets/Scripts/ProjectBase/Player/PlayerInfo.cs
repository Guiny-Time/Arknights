/// <summary>
/// 玩家信息
/// </summary>
public class PlayerInfo{
    public PlayerType type;             //站位类型，选择高台或平地
    public int cost;                    //费用
    public string name;                 //名字

    public PlayerInfo(string name,int cost,PlayerType type){
        this.name = name;
        this.cost = cost;
        this.type = type;
    }
}
/// <summary>
/// 玩家接口，由于每个角色的攻击效果都不一样，所以每当加入一个新的角色就写一个新的继承自这个接口的类
/// </summary>
public interface IPlayer{
    /// <summary>
    /// 普通攻击
    /// </summary>
    void Attack();
    
    /// <summary>
    /// 特殊攻击
    /// </summary>
    void BigAttack();
}
