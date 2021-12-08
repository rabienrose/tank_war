using System.Collections.Generic;
using UnityEngine;
public class Collectable : MonoBehaviour
{
    public bool respawn=true;
    public string collectable_type; //
    public float value;
    public string bullet_type;
    float cul_time=0;
    public float RespawnTime;
    public GameObject render_obj;
    public Battle battle;
    void Start () {
    }    

    public bool IsEmpty(){
        if (render_obj.activeSelf){
            return false;
        }else{
            return true;
        }
    }

    public void ResetCollect(){
        render_obj.SetActive(true);
        cul_time=0;
    }

    void OnTriggerEnter(Collider col)
    {
        if (render_obj.activeSelf==false){
            return;
        }
        GameObject obj = col.gameObject;
        if (obj.tag=="Player"){
            PlayerAttr player = col.GetComponent<PlayerAttr>();
            int bullet_id = battle.bullet_id_table[collectable_type];
            if (collectable_type=="HP"){
                player.ResetHp();                
            }else if(collectable_type=="MP"){
                player.ResetMp();  
            }else if(collectable_type=="SHOOT_COUNT"){
                player.AddShootCount();
            }else if(collectable_type=="BULLET_BOUNCE"){
                player.AddBulletBounce();
            }else if(collectable_type=="HP_REC"){
                player.AddHPRecover(value);
            }else if(collectable_type=="MP_REC"){
                player.AddMPRecover(value);
            }else if(collectable_type=="LUK"){
                player.AddLuk(value);
            }else if(collectable_type=="RANGE"){
                player.AddRange(value);
            }else if(collectable_type=="SPD"){
                player.AddSpd(value);
            }else if(collectable_type=="BULLET_SPD"){
                player.AddBulletSpd(value);
            }else if(bullet_type!=""){
                player.ApplyBullet(bullet_type);
            }
            render_obj.SetActive(false);
        }
    }

    

    void FixedUpdate(){
        if (respawn==false){
            return;
        }
        if (IsEmpty()){
            cul_time=cul_time+Time.fixedDeltaTime;
            if (cul_time>RespawnTime){
                ResetCollect();
            }
        }
        
    }
}
