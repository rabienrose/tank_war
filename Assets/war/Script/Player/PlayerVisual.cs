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
    public GameObject explodeFab;
    public Slider slider_hp;
    public Slider slider_mp;
    public List<Material> player_colors;
    public Transform turret;
    public Transform husk;
    public Battle battle;
    GamePool game_pool;
    public CamPlayer cam;
    PlayerAttr attr;

    void Start(){
        if (battle.train_mode){
            gui.SetActive(false);
            game_pool=battle.GetComponent<GamePool>();
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
        if (!battle.train_mode){
            cam.ShakeCamera();
        }
    }
    public void RotTurret(float angle){
        turret.transform.rotation=Quaternion.Euler(0, angle, 0);
    }
}
