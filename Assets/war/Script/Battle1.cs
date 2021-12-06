using System;
using UnityEngine;

public class Battle1:MonoBehaviour
{
    public Transform bullet_root;
    public Transform player_root;
    public Transform collect_root;
    TankAgent1[] tanks = new TankAgent1[4]; 
    public float battle_max_duration=30;
    float battle_countdown=0;
    public int field_w;
    public int field_h;
    public Transform[] spawn_posis;
    public bool TrainMode=true;
    public void ResetBattle(){
        ClearObjs();
        battle_countdown=battle_max_duration;       
        for (int i =0; i<collect_root.childCount; i++){
            Transform t_child=collect_root.GetChild(i);
            Collectable1 t_col=t_child.GetComponent<Collectable1>();
            t_col.ResetCollect();
        }
    }

    public TankAgent1[] GetAllPlayers(){
        return tanks;
    }
    void Start(){
        for (int i =0; i<player_root.childCount; i++){
            Transform t_child=player_root.GetChild(i);
            tanks[i]=t_child.GetComponent<TankAgent1>();
        }
        // ResetBattle();
    }

    void RestartBattle(){
        ResetBattle();
        for (int i=0; i<tanks.Length;i++){
            if (tanks[i].gameObject.activeSelf==true){
                tanks[i].EndEpisode();
            }
        }
    }

    void ClearObjs(){
        for (int i =0; i<bullet_root.childCount; i++){
            Transform t_child=bullet_root.GetChild(i);
            Bullet bullet= t_child.GetComponent<Bullet>();
            if (bullet!=null){
                Destroy(t_child);
                continue;
            }
        }
    }
    // void FixedUpdate(){
    //     battle_countdown=battle_countdown-Time.fixedDeltaTime;
    //     if (battle_countdown<0){
    //         RestartBattle();
    //     }else{
    //         int alive_count=0;
    //         int alive_tank_id=-1;
    //         for (int i=0; i<tanks.Length;i++){
    //             if (tanks[i].dead==false){
    //                 alive_count=alive_count+1;
    //                 alive_tank_id=i;
    //             }
    //         }
    //         if (alive_count<=1){
    //             if (alive_tank_id!=-1){
    //             }
    //             RestartBattle();
    //         }
    //     }
    // }
}
