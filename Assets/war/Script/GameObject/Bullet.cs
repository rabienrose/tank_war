using UnityEngine;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{
    public string bullet_type;
    public float value;
    public float max_value;
    Rigidbody rigidb;
    PlayerAttr onwer;
    PlayerAction onwer_action;
    float fly_distance=0;
    Vector3 last_posi;
    Vector3 last_bounce_pos;
    SphereCollider sphere_col;
    int bounce_count=0;
    Battle battle;
    GamePool game_pool;

    void Start(){ 
        rigidb= GetComponent<Rigidbody>();
        sphere_col=GetComponent<SphereCollider>();        
    }
    public void OnCreate(Battle battle_){
        battle=battle_;
        game_pool=battle.GetComponent<GamePool>();
    }
    public void Init(PlayerAttr onwer_, Vector3 position, Vector3 forward){
        onwer=onwer_;
        onwer_action=onwer.GetComponent<PlayerAction>();
        transform.localPosition=position;
        transform.forward=forward;
        rigidb.velocity = transform.forward * onwer.bullet_spd;
        last_posi=transform.localPosition;
        last_bounce_pos=transform.localPosition;
        PlayMuzzleFx();
    }
    void FixedUpdate(){
        float t_dis=(last_posi-transform.localPosition).magnitude;
        last_posi=transform.localPosition;
        fly_distance=fly_distance+t_dis;
        if (fly_distance>onwer.range){
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter (Collider co) {
        if (onwer.dead==true){
            return;
        }
        GameObject obj = co.gameObject;
        if (obj.tag=="wall"){
            if (bounce_count>=onwer.bounce_count){
                Destroy(gameObject);
            }else{
                PlayHitFx();
                if(rigidb!=null){
                    Ray ray = new Ray(last_bounce_pos - transform.forward * 0.5f, transform.forward);
                    RaycastHit hit;
                    if(Physics.SphereCast(ray, sphere_col.radius, out hit, Mathf.Infinity, 1 << 0))
                    {
                        if (Vector3.Distance(transform.position, last_bounce_pos) > 0.05f){
                            last_bounce_pos = hit.point;
                            bounce_count++;
                            Vector3 dir = Vector3.Reflect(ray.direction, hit.normal);
                            transform.position = hit.point;
                            transform.rotation = Quaternion.LookRotation(dir);
                            rigidb.velocity = transform.forward * onwer.bullet_spd;
                        }
                    }
                }
            }
        }
        if (obj.tag=="Player"){
            PlayerAttr tar_player = obj.GetComponent<PlayerAttr>();
            if (tar_player!=onwer && tar_player.dead==false){
                PlayHitFx();
                Destroy(gameObject);
                int damage=onwer.atk;
                float o_rand = UnityEngine.Random.value;
                float t_rand = UnityEngine.Random.value;
                if (o_rand<onwer.luk){
                }else{
                    damage=(int)(damage*(1-tar_player.def)); //critic
                }
                if (t_rand<tar_player.luk){
                    damage=0; //miss
                }
                tar_player.ApplyDamage(damage, onwer);
                o_rand = UnityEngine.Random.value;
                t_rand = UnityEngine.Random.value;
                if (o_rand<onwer.luk && t_rand>tar_player.luk){
                    if (onwer.bullet_type=="LIFESTEAL"){
                        onwer.AddHp(damage*onwer.bullet_value);
                    }else if (onwer.bullet_type=="SLOW"){
                        tar_player.ApplyBuf("SLOW", onwer.bullet_value);
                    }else if (onwer.bullet_type=="MUTE"){
                        tar_player.ApplyBuf("MUTE", onwer.bullet_value);
                    }
                }
            }
        }
    }
    void PlayHitFx(){
        if (battle.train_mode==true){
            return;
        }
        game_pool.ShowHitFx(transform.localPosition, transform.forward);
    }
    void PlayMuzzleFx(){
        if (battle.train_mode==true){
            return;
        }
        game_pool.ShowMuzzleFx(transform.localPosition, transform.forward);
    }
}
