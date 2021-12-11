#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVisual : MonoBehaviour
{
    public GameObject gui;
    public Slider slider_hp;
    public Slider slider_mp;
    public Transform turret;
    public Transform husk;
    public Battle battle;
    GamePool game_pool;
    public CamPlayer cam;
    PlayerAttr attr;
    public Renderer[] buf_color_objs;
    bool[] buf_stats;
    Color[] raw_color_cache;


    void Start(){
        if (battle.train_mode){
            gui.SetActive(false);
            
        }
        game_pool=battle.GetComponent<GamePool>();
        attr=GetComponent<PlayerAttr>();
        buf_stats=new bool[battle.buf_list.Length];
        raw_color_cache=new Color[buf_color_objs.Length];
        for (int i=0; i<buf_color_objs.Length; i++){
            raw_color_cache[i]=buf_color_objs[i].material.color;
        }
    }
    public void PlayExplodeFx(){
        if (battle.train_mode!=true){
            game_pool.ShowExplosion(transform.position);
        }
    }
    public void UpdateHpMpUI(){
        slider_hp.value=(float)attr.hp/(float)attr.max_hp;
        slider_mp.value=(float)attr.mp/(float)attr.max_mp;
    }
    public void ShakeCamera(){
        if (!battle.train_mode && cam!=null){
            cam.ShakeCamera();
        }
    }

    public void SetBufFx(int buf_id, bool b_true){
        if (battle.train_mode){
            return;
        }
        if (b_true){
            if (buf_stats[buf_id]==false){
                buf_stats[buf_id]=true;
                for (int i=0; i<buf_color_objs.Length; i++){
                    if (buf_id==0){
                        buf_color_objs[i].material.color=Color.black;
                    }else{
                        buf_color_objs[i].material.color=Color.white;
                    }
                    
                }
            }
        }else{
            if (buf_stats[buf_id]==true){
                buf_stats[buf_id]=false;
                for (int i=0; i<buf_color_objs.Length; i++){
                    buf_color_objs[i].material.color=raw_color_cache[i];
                }
            }
        }
    }

    public void ShowDamageText(int amount){
        if(battle.train_mode){
            return;
        }
        SCT.ScriptableTextDisplay.Instance.InitializeScriptableText(1, transform.position, amount.ToString());
    }
    public void ShowHealText(int amount){
        if(battle.train_mode){
            return;
        }
        SCT.ScriptableTextDisplay.Instance.InitializeScriptableText(0, transform.position, "Hp: "+amount.ToString());
    }
    public void ShowMpText(int amount){
        if(battle.train_mode){
            return;
        }
        SCT.ScriptableTextDisplay.Instance.InitializeScriptableText(5, transform.position, "Mp: "+amount.ToString());
    }
    public void ShowEffectText(string effect_name, float amount){
        if(battle.train_mode){
            return;
        }
        string sign=" +";
        if (amount<0){
            sign=" -";
        }
        SCT.ScriptableTextDisplay.Instance.InitializeScriptableText(3, transform.position, effect_name+sign+amount.ToString());
    }
    public void ShowCriticalText(int amount){
        if(battle.train_mode){
            return;
        }
        SCT.ScriptableTextDisplay.Instance.InitializeScriptableText(2, transform.position, amount.ToString());
    }
    public void ShowMissText(){
        if(battle.train_mode){
            return;
        }
        SCT.ScriptableTextDisplay.Instance.InitializeScriptableText(4, transform.position, "MISS");
    }
    public void RotTurret(float angle){
        turret.transform.rotation=Quaternion.Euler(0, angle, 0);
    }
}
