using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Collectable : MonoBehaviour
{
    public bool respawn=true;
    public string collectable_type; //
    public float value;
    public string bullet_type;
    float cul_time=0;
    public float respawn_time;
    public GameObject render_obj;
    public Battle battle;
    Transform tip_hub;
    public string tip;

    void Start () {
        Text tip_text = transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>();
        tip_text.text=tip;
        tip_hub=transform.GetChild(2);
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
            player.col_count=player.col_count+1;
            if (player.col_count>3){
                col.GetComponent<PlayerAgent>().AddRewardCustom(0.2f,"col");
            }
            int bullet_id = battle.collectable_id_table[this];
            if (collectable_type=="HP"){
                player.ResetHp();                
            }else if(collectable_type=="MP"){
                player.ResetMp();  
            }else if(collectable_type=="SHOOT_COUNT"){
                player.AddShootCount();
            }else if(collectable_type=="BOUNCE_COUNT"){
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
    public void ToggleTip(){
        if (tip_hub.gameObject.activeSelf){
            tip_hub.gameObject.SetActive(false);
        }else{
            tip_hub.gameObject.SetActive(true);
        }
    }
    void FixedUpdate(){
        if (respawn==false){
            return;
        }
        if (IsEmpty()){
            cul_time=cul_time+Time.fixedDeltaTime;
            if (cul_time>respawn_time){
                ResetCollect();
            }
        }
        
    }
}
