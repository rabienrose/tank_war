using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CamPlayer : MonoBehaviour
{
    public Battle battle;
    public PlayerAttr target;
    public bool b_follow=true;
    Vector3 last_mouse_pos;
    public Button CamModeButton;
    bool is_shaking=false;
    public void OnCamMode(){
        b_follow=!b_follow;
    }

    void Start(){
        CamModeButton.onClick.AddListener(OnCamMode);
    }

	public void ShakeCamera() {	
        if (battle.train_mode){
            return;
        }
        ShakeCaller (0.5f, 0.1f);
	}

    public void ShakeCaller (float amount, float duration){
		StartCoroutine (Shake(amount, duration));
	}

	IEnumerator Shake (float amount, float duration){
        if (!is_shaking){
            is_shaking=true;
            Vector3 originalPos = transform.localPosition;
            int counter = 0;
            while (duration > 0.01f) {
                counter++;
                var x = Random.Range (-1f, 1f) * (amount/counter);
                var y = Random.Range (-1f, 1f) * (amount/counter);
                transform.localPosition = Vector3.Lerp (transform.localPosition, new Vector3 (originalPos.x + x, originalPos.y + y, originalPos.z), 0.5f);
                duration -= Time.deltaTime;
                yield return new WaitForSeconds (0.1f);
            }
            transform.localPosition = originalPos;
            
            is_shaking=false;
        }
	}
    void Update(){
        if (is_shaking){
            return;
        }
        if(b_follow){
            Vector3 t_pos=target.transform.position;
            t_pos.y=transform.position.y;
            t_pos.z=t_pos.z-5;
            transform.position=t_pos;
            transform.LookAt(target.transform);
        }else{
            if (Input.GetMouseButton(0)){
                Vector3 diff_p = Input.mousePosition-last_mouse_pos;
                if (diff_p.magnitude<100){
                    Vector3 t_pos=transform.position;
                    t_pos.x=t_pos.x+diff_p.x*-0.04f;
                    t_pos.z=t_pos.z+diff_p.y*-0.04f;
                    transform.position=t_pos;
                }
                last_mouse_pos=Input.mousePosition;
            }
        }
    }
}

