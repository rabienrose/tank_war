using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

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
    Collectable[] collectables;
    public Transform[] col_spawn_posi;
    public GameObject[] col_configs;
    public Dictionary<Collectable, int> collectable_id_table=new Dictionary<Collectable, int>();
    //bufs
    public BufScriptableObject[] buf_list;
    public Dictionary<string, int> buf_id_table=new Dictionary<string, int>();
    //bullets
    public Bullet[] bullet_list;
    public Dictionary<string, int> bullet_id_table=new Dictionary<string, int>(); 
    public int max_bullet_obs=5;
    public bool train_mode=true;
    public Text rank_text;
    public Text attr_text;
    public Button tip_button;
    public Text timer_label;

    public void UpdateRankText(){
        if (train_mode || rank_text==null){
            return;
        }
        float[] scores=new float[players.Length];
        int[] player_ids=new int[players.Length];
        for (int i=0; i<players.Length; i++){
            PlayerAttr p=players[i];
            float score = p.GetScore();
            scores[i]=score;
            player_ids[i]=i;
        }
        Array.Sort( scores, player_ids);
        string rich_text="";
        
        for (int i=player_ids.Length-1; i>=0; i--){
            PlayerAttr p=players[player_ids[i]];
            string item_text = "<color="+p.color_str+">"+p.gameObject.name+": "+"<b>"+scores[i].ToString()+"/"+p.kill_count.ToString()+"/"+p.death_count.ToString()+"</b>"+"</color>";
            rich_text=rich_text+item_text+"\n";
        }
        rank_text.text=rich_text;
    }
    public void UpdateAttrText(){
        if (train_mode || rank_text==null){
            return;
        }
        attr_text.text=players[0].GetAttrStr();
    }

    void UpdateCollectable(){
        for (int i=0; i<col_spawn_posi.Length; i++){
            if (col_spawn_posi[i].childCount==0){
                int rand_id = UnityEngine.Random.Range (0, col_configs.Length);
                Instantiate(col_configs[rand_id], col_spawn_posi[i].position, Quaternion.identity,col_spawn_posi[i]);
            }
        }
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
        if (tip_button!=null){
            tip_button.onClick.AddListener(OnShowTips);
        }
        ResetBattle();
        if(train_mode==false){
            StartCoroutine(UpdateTimer());
        }
    }

    public void OnShowTips(){
        if (train_mode){
            return;
        }
        for (int i=0; i<collectables.Length; i++){
            collectables[i].ToggleTip();
        }
    }

    public void ResetBattle(){
        game_pool.ClearDynObjs();
        battle_countdown=battle_max_duration;       
        for (int i =0; i<collectables.Length; i++){
            collectables[i].ResetCollect();
        }
        UpdateRankText();
        UpdateAttrText();
    }
    public PlayerAttr[] GetAllPlayers(){
        return players;
    }
    

    void FixedUpdate(){
        UpdateCollectable();
        battle_countdown=battle_countdown-Time.fixedDeltaTime;
        if(battle_countdown<0){
            RestartBattle();
        }
        
    }

    IEnumerator UpdateTimer(){
        timer_label.text=((int)battle_countdown).ToString();
        yield return new WaitForSeconds(1);
        StartCoroutine(UpdateTimer());
    }

    void RestartBattle(){
        for (int i=0; i<players.Length;i++){
            PlayerAgent agent=players[i].GetComponent<PlayerAgent>();
            agent.AddRewardCustom(players[i].GetScore(), "end");
            agent.GetComponent<PlayerAttr>().ClearStats();
            agent.EndEpisode();
        }
        ResetBattle();
    }
}
