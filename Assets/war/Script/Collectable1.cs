using System.Collections.Generic;
using UnityEngine;
public class Collectable1 : MonoBehaviour
{
    public enum COL_TYPE{
        HP=0,
        MP=1,
        SHOOT_COUNT=2,
        BULLET_BOUNCE=3,
        BUF_SLOW=4,
        BUF_DAZZLE=5,
        BUF_MUTE=6,
        END=7
    }

    public bool respawn=true;
    public COL_TYPE collectable_type; //
    float cul_time=0;
    public float RespawnTime;
    public GameObject render_obj;
    void Start () {
    }    

    public void ResetCollect(){
        render_obj.SetActive(true);
        cul_time=0;
        tag="collect";
    }

    void OnTriggerEnter(Collider col)
    {
        if (render_obj.activeSelf==false){
            return;
        }
        GameObject obj = col.gameObject;
        if (obj.tag=="Player"){
            TankAgent1 tank = col.GetComponent<TankAgent1>();
            if (collectable_type==COL_TYPE.HP){
                tank.AddHp();                
            }else if(collectable_type==COL_TYPE.MP){
                tank.AddMp();  
            }else if(collectable_type==COL_TYPE.SHOOT_COUNT){
                tank.AddShootCount();
            }else if(collectable_type==COL_TYPE.BULLET_BOUNCE){
                tank.AddBulletBounce();
            }else if(collectable_type==COL_TYPE.BUF_SLOW){
                tank.ApplyBulletBuf(TankAgent1.BUF_TYPE.SLOW);
            }else if(collectable_type==COL_TYPE.BUF_DAZZLE){
                tank.ApplyBulletBuf(TankAgent1.BUF_TYPE.DAZZLE);
            }else if(collectable_type==COL_TYPE.BUF_MUTE){
                tank.ApplyBulletBuf(TankAgent1.BUF_TYPE.MUTE);
            }
            render_obj.SetActive(false);
            tag="Untagged";
        }
    }

    void FixedUpdate(){
        if (respawn==false){
            return;
        }
        cul_time=cul_time+Time.fixedDeltaTime;
        if (cul_time>RespawnTime){
            ResetCollect();
        }
    }
}
