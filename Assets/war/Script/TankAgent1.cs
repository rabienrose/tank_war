#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

public class TankAgent1 : Agent
{
    public enum BUF_TYPE{
        SLOW=0,
        DAZZLE=1,
        MUTE=2,
        END=3
    }
    public Button FireButton;
    public Transform turret;
    public Transform husk;
    Rigidbody rigidb;
    public Transform shotPos;
    public int mp_recover=1;
    int max_mp_recover=10;
    public int max_mp=100;
    [HideInInspector]
    public int mp;
    public int hp_recover=10;
    int max_hp_recover=20;
    public int max_hp=100;
    [HideInInspector]
    public int hp;
    public float def=0f;
    float max_def=1f;
    public int atk=10;
    int max_atk=20;
    public float mov_spd=5f;
    float max_mov_spd=10;
    public float luk=0.3f;
    float max_luk=1f;
    public int range=5;
    int max_range=40;
    public int bullet_spd=20;
    int max_bullet_spd=40;
    [HideInInspector]
    public int shoot_count=1;
    int max_shoot_count=8;
    [HideInInspector]
    public int bounce_count=0;
    int max_bounce_count=10;
    [HideInInspector]
    public BUF_TYPE bullet_buf; 
    public float bullet_buf_time=0;
    [HideInInspector]
    public float[] bufs=new float[(int)BUF_TYPE.END];
    [HideInInspector]
    public bool dead=true;
    public List<Material> player_colors;
    [HideInInspector]
    public int player_id;
    public UIJoystick ui_control;
    Vector2 joy_posi=new Vector2();
    public Battle1 battle;
    public Slider slider_hp;
    public Slider slider_mp;
    bool give_reward_to_damage;
    public Dictionary<BUF_TYPE,float[]> BulletBufTimes=new Dictionary<BUF_TYPE,float[]>();
    Dictionary<int,int> exp_info=new Dictionary<int,int>();
    float min_distance_2_center=9999;
    float min_distance_2_safe=9999;
    int kill_count=0;
    public bool TouchMode=false;
    bool ready_fire=false;
    public GameObject[] BulletPrefab;
    public CamPlayer cam;
    float last_battle_time=0;
    float recover_time=0;
    public GameObject GUI;
    TankFx tankFx;


    void Start () {
        tankFx=GetComponent<TankFx>();
        BulletBufTimes.Add(BUF_TYPE.SLOW,new float[2]{1f,5f});
        BulletBufTimes.Add(BUF_TYPE.DAZZLE,new float[2]{0.2f,1.0f});
        BulletBufTimes.Add(BUF_TYPE.MUTE,new float[2]{2f,10f});
        rigidb=GetComponent<Rigidbody>();
        if (ui_control!=null){
            ui_control.onDrag+=OnDrag;
            ui_control.onDragBegin+=onDragBegin;
            ui_control.onDragEnd+=onDragEnd;
        }
        player_id=GetComponent<BehaviorParameters>().TeamId;
        if(FireButton){
            FireButton.onClick.AddListener(OnFireButton);
        }
        if (battle.TrainMode){
            GUI.SetActive(false);
        }
    }

    public override void OnEpisodeBegin()
    {
        bullet_buf=BUF_TYPE.END;
        TankAgent1[] players=battle.GetAllPlayers();
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
        give_reward_to_damage=false;
        dead=false;
        hp=max_hp;
        mp=max_mp;
        for (int i=0; i<bufs.Length; i++){
            bufs[i]=0;
        }
        bullet_buf_time=0;
        UpdateHpMpUI();
        exp_info=new Dictionary<int,int>();
        shoot_count=1; 
        bounce_count=0;
        min_distance_2_center=30;
        min_distance_2_safe=30;
        kill_count=0;
        last_battle_time=0;
        recover_time=0;
    }
    
    void OnFireButton(){
        ready_fire=true;
    }
    void OnDrag(Vector2 position){
        joy_posi=position;
    }
    void onDragBegin(){
        joy_posi=new Vector3();
    }
    void onDragEnd(){
        joy_posi=new Vector3();
    }
    public void AddBuf(BUF_TYPE buf_type, float time){
        if (buf_type==BUF_TYPE.END){
            return;
        }
        if (bufs[(int)buf_type]<BulletBufTimes[buf_type][0]){
            bufs[(int)buf_type]=BulletBufTimes[buf_type][0];
        }
    }
    public void ApplyBulletBuf(BUF_TYPE b_type){
        if (bullet_buf==BUF_TYPE.END){
            bullet_buf=b_type;
            bullet_buf_time=BulletBufTimes[b_type][0];
        }else{
            if (b_type==bullet_buf){
                bullet_buf_time=bullet_buf_time+BulletBufTimes[b_type][0];
                if (bullet_buf_time>BulletBufTimes[b_type][1]){
                    bullet_buf_time=BulletBufTimes[b_type][1];
                }
            }
        }
    }
    public void AddHp(){
        if (hp<max_hp*0.3f){
            // AddRewardCustom(0.1f, "hp");
        }
        hp=max_hp;
        UpdateHpMpUI();
    }
    public void AddMp(){
        if (mp<max_mp*0.2f){
            // AddRewardCustom(0.1f, "mp");
        }
        mp=max_mp;
        UpdateHpMpUI();
    }
    public void AddBulletBounce(){
        if (bounce_count==0){
            // AddRewardCustom(0.1f, "bounce");
        }
        bounce_count=bounce_count+1;
        if (bounce_count>max_bounce_count){
            bounce_count=max_bounce_count;
        }
    }
    public void AddShootCount(){
        if (shoot_count<=2){
            // AddRewardCustom(0.1f, "shoot_count");
        }
        shoot_count=shoot_count+1;
        if (shoot_count>max_shoot_count){
            shoot_count=max_shoot_count;
        }
    }
    public void UpdateHpMpUI(){
        slider_hp.value=(float)hp/(float)max_hp;
        slider_mp.value=(float)mp/(float)max_mp;
    }
    public void ApplyDamage(int damage, TankAgent1 attacker){
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
        UpdateHpMpUI();
        last_battle_time=Time.fixedTime;
        
    }
    public void OnDamageOther(int damage, bool b_kill){
        // if (give_reward_to_damage==false){
        //     give_reward_to_damage=true;
        // AddRewardCustom(0.05f, "damage");
        // }
        if (b_kill){
            kill_count=kill_count+1;
            AddRewardCustom(1, "kill");
            if (cam!=null){
                cam.ShakeCamera();
            }
        }
    }
    void OnDead(){
        if (battle.TrainMode==false){
            tankFx.PlayExplodeFx();
        }
        dead=true;
        // AddRewardCustom(kill_count, "dead");
        transform.localPosition=new Vector3(0,-10,0);
        rigidb.velocity=new Vector3(0,0,0);
        EndEpisode();
    }
    void FixedUpdate(){
        if (dead){
            return;
        }
        CheckMaxStep();
        float d_time=Time.fixedDeltaTime;
        for (int i=0; i<bufs.Length; i++){
            if (bufs[i]>0){
                bufs[i]=bufs[i]-d_time;
                if (bufs[i]<0){
                    bufs[i]=0;
                }
            }
        }
    }
    void Shoot(){
        GameObject bullet_fab;
        if (battle.TrainMode){
            bullet_fab=BulletPrefab[(int)BUF_TYPE.END+1];
        }else{
            bullet_fab=BulletPrefab[(int)bullet_buf];
        }
        GameObject p = Instantiate(bullet_fab, shotPos.position, turret.rotation, battle.bullet_root.transform);
        p.GetComponent<Bullet>().OnCreate(this);
        if (shoot_count>1){
            for (int i=1; i<shoot_count; i++){
                float rand_angle = UnityEngine.Random.value*40-20;
                Quaternion rand_dir= Quaternion.Euler(0, turret.eulerAngles.y+rand_angle, 0);
                GameObject pp = Instantiate(bullet_fab, shotPos.position, rand_dir, battle.bullet_root.transform);
                pp.GetComponent<Bullet>().OnCreate(this);
            }
        }
        mp=mp-10;
        UpdateHpMpUI();
        last_battle_time=Time.fixedTime;
        bool has_player=false;
        TankAgent1[] players=battle.GetAllPlayers();
        for (int j=0; j<players.Length; j++){
            if(players[j]==this){
                continue;
            }
            float t_dis=(players[j].transform.localPosition-transform.localPosition).magnitude;
            
            if (t_dis<10){
                has_player=true;
                break;
            }
        }
        if (has_player==false){
            // AddRewardCustom(-0.1f,"show_none");
        }
    }
    void OnCollisionEnter(Collision col)
    {
        if (dead){
            return;
        }
        if (StepCount>10){
            GameObject obj = col.gameObject;
            if (obj.tag=="wall" && obj!=gameObject){
                // AddRewardCustom(-0.1f, "wall");
            }
        }
    }
    float[] GetPlayerObs(TankAgent1 player){
        float[] ret=new float[27];
        int temp_point=0;
        for (int i=0; i<battle.GetAllPlayers().Length;i++){ //4
            if (i==player.player_id){
                ret[temp_point]=1;
                temp_point++;
            }else{
                ret[temp_point]=0;
                temp_point++;
            }
        }
        if (player==this){ //5
            ret[temp_point]=1;
            temp_point++;
        }else{
            ret[temp_point]=0;
            temp_point++;
        }
        if (dead){ //6
            ret[temp_point]=1;
            temp_point++;
        }else{
            ret[temp_point]=0;
            temp_point++;
        }
        ret[temp_point]=player.transform.localPosition.x/battle.field_w+0.5f; //7
        temp_point++;
        ret[temp_point]=player.transform.localPosition.z/battle.field_h+0.5f; //8
        temp_point++;
        ret[temp_point]=player.transform.localEulerAngles.y/360; //9
        temp_point++;
        ret[temp_point]=(float)player.hp/(float)player.max_hp; //10
        temp_point++;
        ret[temp_point]=(float)player.mp/(float)player.max_mp; //11
        temp_point++;
        ret[temp_point]=player.def/player.max_def; //12
        temp_point++;
        ret[temp_point]=player.luk/player.max_luk; //13
        temp_point++;
        ret[temp_point]=(float)player.atk/player.max_atk; //14
        temp_point++;
        ret[temp_point]=(float)player.hp_recover/player.max_hp_recover; //15
        temp_point++;
        ret[temp_point]=(float)player.mp_recover/player.max_mp_recover; //16
        temp_point++;
        ret[temp_point]=(float)player.range/player.max_range; //17
        temp_point++;
        ret[temp_point]=(float)player.mov_spd/player.max_mov_spd; //18
        temp_point++;
        ret[temp_point]=(float)player.shoot_count/player.max_shoot_count; //19
        temp_point++;
        ret[temp_point]=(float)player.bounce_count/player.max_bounce_count; //20
        temp_point++;
        ret[temp_point]=(float)player.bullet_spd/player.max_bullet_spd; //21
        temp_point++;
        for (int i=0; i<player.bufs.Length; i++){ //24
            ret[temp_point]=player.bufs[i]/BulletBufTimes[(BUF_TYPE)i][1];
            temp_point++;
        }
        for (int i=0; i<(int)BUF_TYPE.END; i++){ //27
            if (bullet_buf==(BUF_TYPE)i){
                ret[temp_point]=BulletBufTimes[(BUF_TYPE)i][0]/BulletBufTimes[(BUF_TYPE)i][1];
                temp_point++;
            }else{
                ret[temp_point]=0;
                temp_point++;
            }
        }
        return ret;

    }
    void UpdateCellStats(){
        int cell_id = (int)(transform.position.x)*10+(int)(transform.position.z);
        if (exp_info.ContainsKey(cell_id)){
            exp_info[cell_id]=exp_info[cell_id]+1;
            if (exp_info[cell_id]>50){
            }
        }else{
            exp_info.Add(cell_id,1);
        }
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        if(dead){
            return;
        }
        // GiveRewardOnLoc();
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
            UpdateHpMpUI();
            recover_time=time_now;
        }
        
        int temp_count=0;
        TankAgent1[] players = battle.GetAllPlayers();
        for (int i=0; i<players.Length; i++){
            float[] obs = GetPlayerObs(players[i]);
            for (int j=0; j<obs.Length; j++){
                sensor.AddObservation(obs[j]);
                temp_count=temp_count+1;
            }
        }
    }
    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        if (dead){
            return;
        }
        if (mp<10){
            actionMask.SetActionEnabled(0, 1, false);
        }
        if (bufs[(int)BUF_TYPE.DAZZLE]>0){
            actionMask.SetActionEnabled(0, 1, false);
        }
    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        if (dead){
            return;
        }
        bool can_mov=true;
        bool can_attack=true;
        float speed_rate=1.0f;
        if (bufs[(int)BUF_TYPE.DAZZLE]>0){
            can_mov=false;
            can_attack=false;
        }
        if (bufs[(int)BUF_TYPE.SLOW]>0){
            speed_rate=0.5f;
        }
        
        if (bufs[(int)BUF_TYPE.MUTE]>0){
            can_attack=false;
        }
        if (can_mov){
            float x = Mathf.Clamp(actionBuffers.ContinuousActions[0], -1f, 1f);
            float z = Mathf.Clamp(actionBuffers.ContinuousActions[1], -1f, 1f);
            float len=Mathf.Sqrt(x*x+z*z);
            if (len>0.1){
                if (len>1){
                    len=1;
                }
                transform.forward=new Vector3(x/len, 0, z/len);
                rigidb.velocity = transform.forward * mov_spd*len*speed_rate;
            }else{
                rigidb.velocity=new Vector3(0,0,0);
            }
        }else{
            rigidb.velocity=new Vector3(0,0,0);
        }
        if (can_attack){
            float fire = actionBuffers.DiscreteActions[0];
            if (fire==1 && mp>=10){
                float angle_turret = actionBuffers.ContinuousActions[2];
                angle_turret=angle_turret*360;
                turret.transform.rotation=Quaternion.Euler(0, -angle_turret, 0);
                Shoot();
            }
        }
    }
    void GiveRewardOnLoc(){
        Vector3 tmp_cur_psoi=transform.localPosition;
        if (mp>0.7*max_mp){
            float dist_2_center=tmp_cur_psoi.magnitude;
            if (min_distance_2_center>dist_2_center){
                float diff_dis=min_distance_2_center-dist_2_center;
                if (min_distance_2_center<30){
                    AddRewardCustom(diff_dis*0.05f, "loc");
                }
                min_distance_2_center=dist_2_center;
            }
        }
        if (mp<0.3*max_mp){
            float dist_2_safe=-1f;
            for(int i=0; i<battle.spawn_posis.Length; i++){
                float t_dis = (battle.spawn_posis[i].localPosition-tmp_cur_psoi).magnitude;
                if (dist_2_safe==-1 || t_dis<dist_2_safe){
                    dist_2_safe=t_dis;
                }
            }
            if (min_distance_2_safe>dist_2_safe){
                float diff_dis=min_distance_2_safe-dist_2_safe;
                if (min_distance_2_safe<30){
                    AddRewardCustom(diff_dis*0.05f, "loc");
                }
                min_distance_2_safe=dist_2_safe;
            }
        }
    }
    void CheckMaxStep(){
        if(StepCount>3000){
            // AddRewardCustom(kill_count, "end");
            EndEpisode();
        }
    }
    void AddRewardCustom(float inc, string desc){
        AddReward(inc);
        // Debug.Log(name+": "+Math.Round(inc,4)+"/"+desc+" ( "+Math.Round(GetCumulativeReward(), 2)+" )");
    }
    float clamp_angle(float angle){
        if (angle>180){
            angle=angle-360;
        }
        if (angle<-180){
            angle=angle+360;
        }
        angle=angle/360.0f;
        return angle;
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        if (dead){
            return;
        }
        if (FireButton==null){
            return;
        }
        var continuousActionsOut = actionsOut.ContinuousActions;
        if (TouchMode){
            continuousActionsOut[0] = joy_posi.x;
            continuousActionsOut[1] = joy_posi.y;
        }else{
            if (Input.GetKey(KeyCode.A)){
                continuousActionsOut[0] = -1;
            }else if(Input.GetKey(KeyCode.D)){
                continuousActionsOut[0] = 1;
            }
            if (Input.GetKey(KeyCode.W)){
                continuousActionsOut[1] = 1;
            }else if(Input.GetKey(KeyCode.S)){
                continuousActionsOut[1] = -1;
            }
        }
        
        var discreteActionsOut = actionsOut.DiscreteActions;
        if(ready_fire){
            discreteActionsOut[0] = 1;
            TankAgent1[] tankAgent1s= battle.GetAllPlayers();
            float min_dist=-1;
            Vector3 min_posi=new Vector3(0,0,0);
            for (int i=0; i<tankAgent1s.Length; i++){
                if (tankAgent1s[i]==this){
                    continue;
                }
                float temp_dis = (tankAgent1s[i].transform.localPosition-transform.localPosition).magnitude;
                if (min_dist==-1 || min_dist>temp_dis){
                    min_dist=temp_dis;
                    min_posi=tankAgent1s[i].transform.localPosition;
                }
            }
            if (min_dist<10){
                Vector3 d_vec = min_posi-transform.localPosition;
                float angle = Vector3.Angle(new Vector3(1,0,0), d_vec);
                if (d_vec.z<0){
                    angle=-angle;
                }
                angle=angle-90;
                continuousActionsOut[2]=angle/360f;
            }
            ready_fire=false;
        }else{
            discreteActionsOut[0] = 0;
        }
    }
    void Update(){
        if (Input.GetKey(KeyCode.Space)){
            ready_fire=true;
        }
    }
}
