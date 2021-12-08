using System;
using UnityEngine;
using System.Collections.Generic;

public class Battle:MonoBehaviour
{
    public PlayerAttr[] players; 
    public float battle_max_duration=30;
    float battle_countdown=0;
    public int field_w;
    public int field_h;
    public Transform[] spawn_posis;
    public GamePool game_pool;
    // collectables
    public Collectable[] collectables;
    public Dictionary<Collectable, int> collectable_id_table=new Dictionary<Collectable, int>();
    //bufs
    public BufScriptableObject[] buf_list;
    public Dictionary<string, int> buf_id_table=new Dictionary<string, int>();
    //bullets
    public Bullet[] bullet_list;
    public Dictionary<string, int> bullet_id_table=new Dictionary<string, int>(); 
    public int max_bullet_obs=5;
    public bool train_mode=true;

    public float[] GetCollectableObs(){
        float[] out_obs=new float[collectables.Length];
        for (int i=0; i<collectables.Length; i++){
            if (collectables[i].IsEmpty()){
                out_obs[i]=0;
            }else{
                out_obs[i]=1;
            }
        }
        return out_obs;
    }

    void Start(){
        for (int i=0; i<collectables.Length; i++){
            collectable_id_table.Add(collectables[i],i);
        }
        for (int i=0; i<buf_list.Length; i++){
            buf_id_table.Add(buf_list[i].buf_name,i);
        }
        for (int i=0; i<bullet_list.Length; i++){
            bullet_id_table.Add(bullet_list[i].bullet_type,i);
        }
        game_pool=GetComponent<GamePool>();
    }

    public void ResetBattle(){
        game_pool.ClearDynObjs();
        battle_countdown=battle_max_duration;       
        for (int i =0; i<collectables.Length; i++){
            collectables[i].ResetCollect();
        }
    }
    public PlayerAttr[] GetAllPlayers(){
        return players;
    }
    

    void FixedUpdate(){
        battle_countdown=battle_countdown-Time.fixedDeltaTime;
        if(battle_countdown<0){
            RestartBattle();
        }
    }

    void RestartBattle(){
        ResetBattle();
        for (int i=0; i<players.Length;i++){
            PlayerAgent agent=players[i].GetComponent<PlayerAgent>();
            agent.AddRewardCustom(players[i].kill_count-players[i].death_count, "end");
            agent.EndEpisode();
        }
    }
}
