using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController IS;

    private int acuity_size;
    private float flash_time;
    private float move_speed;

    private void Awake()
    {
        IS = this;

        this.acuity_size = 5;
        this.flash_time = 0.08f;
        this.move_speed = 10.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        input_handle();
        change_A_sprite();
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region Key_handle

    private void input_handle()
    {
        InputController.IS.KeyQ += KQ_down;
        InputController.IS.KeyW += KW_down;
        InputController.IS.KeyE += KE_down;
        InputController.IS.KeyR += KR_down;
        InputController.IS.KeyT += KT_down;
        InputController.IS.KeyY += KY_down;
        InputController.IS.KeyU += KU_down;
        InputController.IS.KeyI += KI_down;
        InputController.IS.KeyO += KO_down;
        InputController.IS.KeyA += KA_down;
        InputController.IS.KeyS += KS_down;
        InputController.IS.KeyESC += KESC_down;
    }

    private void KQ_down()
    {
        A_size_up();
    }

    private void KW_down()
    {
        A_size_down();
    }

    private void KE_down()
    {
        add_flash_time();
    }

    private void KR_down()
    {
        minu_flash_time();
    }

    private void KT_down()
    {
        flash_acuity();
    }

    private void KY_down()
    {
        add_move_speed();
    }

    private void KU_down()
    {
        minu_move_speed();
    }

    private void KI_down()
    {
        move_target();
    }

    private void KO_down()
    {
        end_moving();
    }

    private void KA_down()
    {
        enlarge_target();
    }

    private void KS_down()
    {
        shrink_target();
    }

    private void KESC_down()
    {
        Application.Quit();
    }

    #endregion

    private void A_size_up()
    {
        if (acuity_size + 1 <= 20)
        {
            acuity_size++;
            change_A_sprite();
        }
    }

    private void A_size_down()
    {
        if (acuity_size - 1 >= 1)
        {
            acuity_size--;
            change_A_sprite();
        }
    }

    private void change_A_sprite()
    {
        RC.IS.AG_script.change_A_sprite(acuity_size);
    }

    private void add_flash_time()
    {
        flash_time += 0.01f;
    }

    private void minu_flash_time()
    {
        flash_time -= 0.01f;
    }

    private void flash_acuity()
    {
        RC.IS.AG_script.flash_acuity(flash_time);
    }

    private void add_move_speed()
    {
        move_speed += 1.0f;
    }

    private void minu_move_speed()
    {
        if (move_speed - 1.0f > 0.0f) { move_speed -= 1.0f; }
    }

    private void move_target()
    {
        RC.IS.TG_script.move_target(move_speed);
    }

    private void end_moving()
    {
        RC.IS.TG_script.end_moving();
    }

    private void enlarge_target()
    {
        RC.IS.TG_script.enlarge_target(0.1f);
    }

    private void shrink_target()
    {
        RC.IS.TG_script.shrink_target(0.1f);
    }

}
