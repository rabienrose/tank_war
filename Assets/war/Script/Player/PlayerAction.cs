#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAction : MonoBehaviour
{
    public class Action{
        public enum DIR{
            STOP=0,
            R=1,
            RD=2,
            D=3,
            LD=4,
            L=5,
            LU=6,
            U=7,
            RU=8
        }
        public Action(){
            mov=DIR.STOP;
            fire=0;
        }
        public DIR mov;
        public int fire;
    };
    PlayerAttr attr;
    PlayerVisual visual;
    public bool TouchMode=false;
    bool ready_fire=false;
    public CamPlayer cam;
    public UIJoystick ui_control;
    Vector2 joy_posi=new Vector2();
    public Transform shotPos;
    public Transform turret;
    public Button FireButton;
    Rigidbody rigidb;
    Action next_action=new Action();
    Battle battle;
    GamePool game_pool;
    void Start(){
        rigidb=GetComponent<Rigidbody>();
        attr=GetComponent<PlayerAttr>();
        visual=GetComponent<PlayerVisual>();
        game_pool=GetComponent<GamePool>();
        if(FireButton){
            FireButton.onClick.AddListener(OnFireButton);
        }
        if (ui_control!=null){
            ui_control.onDrag+=OnDrag;
            ui_control.onDragBegin+=onDragBegin;
            ui_control.onDragEnd+=onDragEnd;
        }
    }
    void Update(){
        next_action.mov=Action.DIR.STOP;
        if (joy_posi.magnitude>0.5){
            float angle = Vector2.Angle(new Vector2(1,0), joy_posi);
            if (angle>=337.5 || angle<=22.5){
                next_action.mov=Action.DIR.R;
            }else if (angle>=22.5 && angle<=67.5){
                next_action.mov=Action.DIR.RU;
            }else if (angle>=67.5 && angle<=112.5){
                next_action.mov=Action.DIR.U;
            }else if (angle>=112.5 && angle<=157.5){
                next_action.mov=Action.DIR.LU;
            }else if (angle>=157.5 && angle<=202.5){
                next_action.mov=Action.DIR.L;
            }else if (angle>=202.5 && angle<=247.5){
                next_action.mov=Action.DIR.LD;
            }else if (angle>=247.5 && angle<=292.5){
                next_action.mov=Action.DIR.D;
            }else if (angle>=292.5 && angle<=337.5){
                next_action.mov=Action.DIR.RD;
            }
        }

        if (Input.GetKey(KeyCode.A)){
            next_action.mov=Action.DIR.L;
        }else if(Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W)){
            next_action.mov=Action.DIR.LU;
        }else if(Input.GetKey(KeyCode.W)){
            next_action.mov=Action.DIR.U;
        }else if(Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.W)){
            next_action.mov=Action.DIR.RU;
        }else if(Input.GetKey(KeyCode.D)){
            next_action.mov=Action.DIR.R;
        }else if(Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S)){
            next_action.mov=Action.DIR.RD;
        }else if(Input.GetKey(KeyCode.S)){
            next_action.mov=Action.DIR.D;
        }else if(Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S)){
            next_action.mov=Action.DIR.LD;
        }
        if (Input.GetKey(KeyCode.Space)){
            next_action.fire=1;
        }else{
            next_action.fire=0;
        }
    }

    void Shoot(){
        string bullet_type="";
        if (battle.train_mode==false){
            bullet_type=attr.bullet_type;
        }
        game_pool.CreateBullet(bullet_type, shotPos.localPosition, turret.forward, attr);
        if (attr.shoot_count>1){
            for (int i=1; i<attr.shoot_count; i++){
                float rand_angle = UnityEngine.Random.value*40-20;
                Vector3 forward = new Vector3(shotPos.forward.x, 0, shotPos.forward.z);
                turret.eulerAngles=new Vector3(0, turret.eulerAngles.y+rand_angle, 0);
                Quaternion rand_dir= Quaternion.Euler(0, turret.eulerAngles.y+rand_angle, 0);
                game_pool.CreateBullet(bullet_type, shotPos.localPosition, turret.forward, attr);
            }
        }
        attr.last_battle_time=Time.fixedTime;
        attr.mp=attr.mp-10;
        visual.UpdateHpMpUI();
    }

    void OnFireButton(){
        ready_fire=true;
    }
    void OnDrag(Vector2 position){
        joy_posi=position;
    }
    void onDragBegin(){
        joy_posi=new Vector2();
    }
    void onDragEnd(){
        joy_posi=new Vector2();
    }

    public float[] GetBulletObs(){
        float[] out_obs= new float[battle.max_bullet_obs*4];
        Transform[] bullets = game_pool.GetAllBullets();
        List<Transform> filtered_bullet=new List<Transform>();
        for (int i=0; i<bullets.Length; i++){
            Vector3 t_dir=transform.localPosition-bullets[i].localPosition;
            Vector3 t_dir_n=t_dir.normalized;
            float dot = bullets[i].forward.x*t_dir_n.x+bullets[i].forward.z*t_dir_n.z;
            if (t_dir.magnitude<2){
                if (dot>0){
                    filtered_bullet.Add(bullets[i]);
                }
            }else{
                if (dot>0.5 && t_dir.magnitude<10){
                    Ray ray = new Ray(bullets[i].position, t_dir_n);
                    RaycastHit hit;
                    if(!Physics.SphereCast(ray, 0.2f, out hit, t_dir.magnitude, 1 << 0)){
                        filtered_bullet.Add(bullets[i]);
                    }
                }
            }
        }
        if (filtered_bullet.Count>=battle.max_bullet_obs){
            float[] b_dists=new float[filtered_bullet.Count];
            Transform[] items=new Transform[filtered_bullet.Count];
            for (int i=0; i<filtered_bullet.Count; i++){
                float dist = Vector3.Distance(transform.localPosition, filtered_bullet[i].localPosition);
                b_dists[i]=dist;
                items[i]=filtered_bullet[i];
            }
            Array.Sort( b_dists, items);
        }
        int item_count=filtered_bullet.Count;
        if (item_count>battle.max_bullet_obs){
            item_count=battle.max_bullet_obs;
        }
        for (int i=0; i<item_count; i++){
            out_obs[i*3+0]=filtered_bullet[i].localPosition.x/battle.field_w;
            out_obs[i*3+1]=filtered_bullet[i].localPosition.y/battle.field_h;
            out_obs[i*3+2]=filtered_bullet[i].GetComponent<Rigidbody>().velocity.magnitude/attr.max_bullet_spd;
            out_obs[i*3+3]=filtered_bullet[i].eulerAngles.y;
        }
        return out_obs;
    }

    public void OnAction(Action act){
        bool can_attack=true;
        float speed_rate=1.0f;
        int slow_buf_id = battle.buf_id_table["SLOW"];
        if (attr.bufs[slow_buf_id]>0){
            speed_rate=battle.buf_list[slow_buf_id].value;
        }
        int mute_buf_id = battle.buf_id_table["MUTE"];
        if (attr.bufs[mute_buf_id]>0){
            can_attack=false;
        }

        if (act.mov==PlayerAction.Action.DIR.D){
            transform.forward=new Vector3(0,0,-1);
        }else if(act.mov==PlayerAction.Action.DIR.LD){
            transform.forward=new Vector3(-0.707f,0,-0.707f);
        }else if(act.mov==PlayerAction.Action.DIR.L){
            transform.forward=new Vector3(-0.707f,0,0);
        }else if(act.mov==PlayerAction.Action.DIR.LU){
            transform.forward=new Vector3(-0.707f,0,0.707f);
        }else if(act.mov==PlayerAction.Action.DIR.U){
            transform.forward=new Vector3(0,0,0.707f);
        }else if(act.mov==PlayerAction.Action.DIR.RU){
            transform.forward=new Vector3(0.707f,0,0.707f);
        }else if(act.mov==PlayerAction.Action.DIR.R){
            transform.forward=new Vector3(0.707f,0,0);
        }else if(act.mov==PlayerAction.Action.DIR.RD){
            transform.forward=new Vector3(0.707f,0,-0.707f);
        }

        rigidb.velocity = transform.forward * attr.mov_spd*speed_rate;
        if (can_attack && act.fire==1 && attr.mp>=10){
            PlayerAttr[] players= battle.GetAllPlayers();
            float min_dist=-1;
            Vector3 min_posi=new Vector3(0,0,0);
            for (int i=0; i<players.Length; i++){
                if (players[i]==this){
                    continue;
                }
                float temp_dis = (players[i].transform.localPosition-transform.localPosition).magnitude;
                if (min_dist==-1 || min_dist>temp_dis){
                    Vector3 t_dir=transform.localPosition-players[i].transform.localPosition;
                    Ray ray = new Ray(players[i].transform.localPosition, t_dir);
                    RaycastHit hit;
                    if(!Physics.SphereCast(ray, 0.2f, out hit, t_dir.magnitude, 1 << 0)){
                        continue;
                    }
                    min_dist=temp_dis;
                    min_posi=players[i].transform.localPosition;
                }
            }
            if (min_dist<20){
                Vector3 d_vec = min_posi-transform.localPosition;
                float angle = Vector3.Angle(new Vector3(1,0,0), d_vec);
                if (d_vec.z<0){
                    angle=-angle;
                }
                angle=90-angle;
                Shoot();
            }
        }
    }

    public Action GetAction(){
        Action action=new Action();
        action.mov=next_action.mov;
        action.fire=next_action.fire;
        action=new Action();
        return action;
    }
}
