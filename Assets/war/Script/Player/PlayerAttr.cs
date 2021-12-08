#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerAttr : MonoBehaviour
{
    // dyn var
    public int mp_recover=0;
    public int hp_recover=0;
    public float mov_spd=0;
    public float luk=0;
    public int range=0;
    public int bullet_spd=0;
    public int shoot_count=0;
    public int bounce_count=0;
    public int kill_count=0;
    public int death_count=0;
    public float last_battle_time=0;
    public float recover_time=0;
    public float min_distance_2_center=9999;
    public float min_distance_2_safe=9999;
    public bool give_reward_to_damage=false;
    public float[] bufs;
    public int mp=100;
    public int hp=100;
    public bool dead=true;
    public string bullet_type; 
    public float bullet_value=0;
    public float bullet_max_value=0;
    public float def=0f;
    public int atk=19;
    // max var
    public int max_mp_recover=20;
    public int max_hp_recover=20;
    public float max_mov_spd=5;
    public float max_luk=0.8f;
    public int max_range=40;
    public int max_bullet_spd=40;
    public int max_shoot_count=7;
    public int max_bounce_count=10;
    public int max_mp=100;
    public int max_hp=100;
    // base var
    int base_mp_recover=0;
    int base_hp_recover=0;
    float base_mov_spd=5f;
    float base_luk=0f;
    int base_range=5;
    int base_bullet_spd=10;
    int base_shoot_count=1;
    int base_bounce_count=0;
    // config
    public string PlayerName;
    
    // Comp
    public Battle battle;
    PlayerVisual visual;
    PlayerAgent agent;
    
    void Start(){
        
        agent=GetComponent<PlayerAgent>();
        visual=GetComponent<PlayerVisual>();
    }
    public void ResetAttr(){
        // reset posi
        PlayerAttr[] players=battle.GetAllPlayers();
        int t_idn=0;
        List<int> free_posi=new List<int>();
        for (int i=0; i<battle.spawn_posis.Length; i++){
            bool has_player=false;
            for (int j=0; j<players.Length; j++){
                float t_dis=(players[j].transform.localPosition-battle.spawn_posis[i].localPosition).magnitude;
                if (t_dis<5){
                    has_player=true;
                    break;
                }
            }
            if (has_player==false){
                free_posi.Add(i);
            }
        }
        t_idn=free_posi[UnityEngine.Random.Range(0,free_posi.Count)];
        transform.localPosition=battle.spawn_posis[t_idn].localPosition;
        //reset dyn var
        mp_recover=0;
        hp_recover=0;
        mov_spd=0;
        luk=0;
        range=0;
        bullet_spd=0;
        shoot_count=0;
        bounce_count=0;
        kill_count=0;
        death_count=0;
        last_battle_time=0;
        recover_time=0;
        min_distance_2_center=9999;
        min_distance_2_safe=9999;
        give_reward_to_damage=false;
        bufs=new float[battle.buf_list.Length];
        mp=max_mp;
        hp=max_hp;
        dead=true;
        bullet_type=""; 
        visual.UpdateHpMpUI();
    }
    void GiveRewardOnLoc(){
        Vector3 tmp_cur_psoi=transform.localPosition;
        float dist_2_center=tmp_cur_psoi.magnitude;
        if (min_distance_2_center>dist_2_center){
            float diff_dis=min_distance_2_center-dist_2_center;
            if (min_distance_2_center<30){
                agent.AddRewardCustom(diff_dis*0.05f, "loc");
            }
            min_distance_2_center=dist_2_center;
        }
    }
    public float[] GetPlayerObs(){
        float[] ret=new float[13+bufs.Length+battle.bullet_list.Length];
        int temp_point=0;
        ret[temp_point]=transform.localPosition.x/battle.field_w+0.5f; //1
        temp_point++;
        ret[temp_point]=transform.localPosition.z/battle.field_h+0.5f; //2
        temp_point++;
        ret[temp_point]=transform.localEulerAngles.y/360; //3
        temp_point++;
        ret[temp_point]=(float)hp/(float)max_hp; //4
        temp_point++;
        ret[temp_point]=(float)mp/(float)max_mp; //5
        temp_point++;
        ret[temp_point]=luk/max_luk; //6
        temp_point++;
        ret[temp_point]=(float)hp_recover/max_hp_recover; //7
        temp_point++;
        ret[temp_point]=(float)mp_recover/max_mp_recover; //8
        temp_point++;
        ret[temp_point]=(float)range/max_range; //9
        temp_point++;
        ret[temp_point]=(float)mov_spd/max_mov_spd; //10
        temp_point++;
        ret[temp_point]=(float)shoot_count/max_shoot_count; //11
        temp_point++;
        ret[temp_point]=(float)bounce_count/max_bounce_count; //12
        temp_point++;
        ret[temp_point]=(float)bullet_spd/max_bullet_spd; //13
        temp_point++;
        for (int i=0; i<bufs.Length; i++){//15
            ret[temp_point]=bufs[i]/battle.buf_list[i].max_buf_time;
            temp_point++;
        }
        for (int i=0; i<battle.bullet_list.Length; i++){ //18
            if (bullet_type==battle.bullet_list[i].bullet_type){
                ret[temp_point]=bullet_value/bullet_max_value;
                temp_point++;
            }else{
                ret[temp_point]=0;
                temp_point++;
            }
        }
        return ret;
    }
    public void AddHPRecover(float amount){
        hp_recover=hp_recover+(int)amount;
        if (hp_recover>max_hp_recover){
            hp_recover=max_hp_recover;
        }
    }
    public void AddMPRecover(float amount){
        mp_recover=mp_recover+(int)amount;
        if (mp_recover>max_mp_recover){
            mp_recover=max_mp_recover;
        }
    }
    public void AddRange(float amount){
        range=range+(int)amount;
        if (range>max_range){
            range=max_range;
        }
    }
    public void AddLuk(float amount){
        luk=luk+(int)amount;
        if (luk>max_luk){
            luk=max_luk;
        }
    }
    public void AddSpd(float amount){
        mov_spd=mov_spd+(int)amount;
        if (mov_spd>max_mov_spd){
            mov_spd=max_mov_spd;
        }
    }
    public void AddBulletSpd(float amount){
        bullet_spd=bullet_spd+(int)amount;
        if (bullet_spd>max_bullet_spd){
            bullet_spd=max_bullet_spd;
        }
    }
    public void ResetHp(){
        hp=max_hp;
        visual.UpdateHpMpUI();
    }
    public void AddHp(float amount){
        hp=hp+(int)amount;
        if (hp>max_hp){
            hp=max_hp;
        }
        visual.UpdateHpMpUI();
    }
    public void ResetMp(){
        mp=max_mp;
        visual.UpdateHpMpUI();
    }
    public void AddBulletBounce(){
        bounce_count=bounce_count+1;
        if (bounce_count>max_bounce_count){
            bounce_count=max_bounce_count;
        }
    }
    public void AddShootCount(){
        shoot_count=shoot_count+1;
        if (shoot_count>max_shoot_count){
            shoot_count=max_shoot_count;
        }
    }
    public int GetHpRecover(){
        return base_hp_recover+hp_recover;
    }
    public int GetMpRecover(){
        return base_mp_recover+mp_recover;
    }
    public float GetLuk(){
        return base_luk+luk;
    }
    public int GetRange(){
        return base_range+range;
    }
    public float GetSpd(){
        return base_mov_spd+mov_spd;
    }
    public int GetBulletSpd(){
        return base_bullet_spd+bullet_spd;
    }
    public void ApplyBuf(string buf_type, float time){
        if (buf_type==""){
            return;
        }
        int buf_id=battle.buf_id_table[buf_type];
        bufs[buf_id]=bufs[buf_id]+time;
        if (bufs[buf_id]<battle.buf_list[buf_id].max_buf_time){
            bufs[buf_id]=battle.buf_list[buf_id].max_buf_time;
        }
    }
    public void ApplyBullet(string bullet_type_){
        if (bullet_type==""){
            bullet_type=bullet_type_;
        }else{
            if(bullet_type==bullet_type_){
                int bullet_id = battle.bullet_id_table[bullet_type_];
                bullet_value=bullet_value+battle.bullet_list[bullet_id].value;
                if (bullet_value>battle.bullet_list[bullet_id].max_value){
                    bullet_value=battle.bullet_list[bullet_id].max_value;
                }
            }
        }
    }
    void FixedUpdate(){
        if (dead){
            return;
        }
        float d_time=Time.fixedDeltaTime;
        for (int i=0; i<bufs.Length; i++){
            if (bufs[i]>0){
                bufs[i]=bufs[i]-d_time;
                if (bufs[i]<0){
                    bufs[i]=0;
                }
            }
        }
        float time_now=Time.fixedTime;
        if (time_now-recover_time>1){
            mp=mp+mp_recover;
            if (mp>max_mp){
                mp=max_mp;
            }
            if (time_now - last_battle_time>=5){
                hp=hp+hp_recover;
                if (hp>max_hp){
                    hp=max_hp;
                }
            }
            recover_time=time_now;
            visual.UpdateHpMpUI();
        }
    }
    public void ApplyDamage(int damage, PlayerAttr attacker){
        if (damage<0){
            return;
        }
        hp=hp-damage;
        if (hp<=0){
            hp=0;
            OnDead();
            attacker.OnDamageOther(damage, true);
        }else{
            attacker.OnDamageOther(damage, false);
        }
        visual.UpdateHpMpUI();
        last_battle_time=Time.fixedTime;
        
    }
    public void OnDamageOther(int damage, bool b_kill){
        if (give_reward_to_damage==false){
            give_reward_to_damage=true;
            agent.AddRewardCustom(0.05f, "damage");
        }
        if (b_kill){
            kill_count=kill_count+1;
            if (kill_count==1){
                agent.AddRewardCustom(1, "kill");
            }
            visual.ShakeCamera();
        }
    }
    void OnDead(){
        if (battle.train_mode==false){
            visual.PlayExplodeFx();
        }
        ResetAttr();
    }

}
