using UnityEngine;

/// <summary>
/// 放置控制，控制玩家的拖拽放置与放置朝向
/// </summary>
public class PutDownController : MonoBehaviour
{
    public Direction orientation = Direction.Right;         //放置朝向，默认朝右
    Vector3 mouseposition;

    void Update(){
        mouseposition = Input.mousePosition;
        mouseposition.z = 0;
        mouseposition = Camera.main.ScreenToWorldPoint(mouseposition);
        mouseposition.z = -1;
    }

    /// <summary>
    /// 拖拽事件，调整角色的站位朝向；用于EventTrigger
    /// </summary>
    private void OnMouseDrag(){
        Vector3 playerPosition = transform.position;
        Vector3 targetDir = mouseposition - playerPosition;
        double angle = Vector3.Angle(targetDir, transform.up);
        if(angle > 41 && angle < 115 && mouseposition.x - playerPosition.x > 0){//41~115
            orientation = Direction.Right;
        }
        if(angle < 175 && angle > 115 && mouseposition.y - playerPosition.y < 0){//115~175
            orientation = Direction.Down;
        }
        if(angle > 41 && angle < 115 && mouseposition.x - playerPosition.x < 0){//115~41
            orientation = Direction.Left;
        }
        if(angle > 0 && angle < 41 && mouseposition.y - playerPosition.y>0){//41~0
            orientation = Direction.Up;
        }
    }

    /// <summary>
    /// 鼠标松开，放置完成;用于EventTrigger
    /// </summary>
    private void OnMouseUp(){
        if(orientation == Direction.Left){
            this.transform.rotation = new Quaternion(0,180,0,0);
        }else{
            this.transform.rotation = new Quaternion(0,0,0,0);
        }
        PlayerMgr.GetInstance().playerList.Add(this.gameObject.transform);
        this.GetComponent<PutDownController>().enabled = false;
        this.GetComponent<PlayerController>().line.enabled = false;
        
    }
}
