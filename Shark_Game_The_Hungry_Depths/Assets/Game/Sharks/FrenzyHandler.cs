using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrenzyHandler
{
    PlayerController owner;

    public bool InFrenzy
    {
        get;
        private set;
    }

    public float MoveSpeedMultiplier
    {
        get;
        private set;
    }

    Camera cam;
    float camDefaultFOV;

    public FrenzyHandler(PlayerController owner, Camera cam)
    {
        this.owner = owner;
        this.cam = cam;

        InFrenzy = false;
        MoveSpeedMultiplier = 1f;
        camDefaultFOV = cam.fieldOfView;
    }

    public void AddFrenzy(float frenzyAmount)
    {
        if (InFrenzy) return;

        owner.currentFrenzy += frenzyAmount;
        if (owner.currentFrenzy > owner.sharkScriptableObject.maxFrenzy)
        {
            owner.currentFrenzy = owner.sharkScriptableObject.maxFrenzy;
            owner.StartCoroutine(FrenzyCoroutine());
        }
    }

    IEnumerator FrenzyCoroutine()
    {
        InFrenzy = true;
        SignalBus.OnFrenzyStartInvoke();

        cam.fieldOfView = 80;
        MoveSpeedMultiplier = 2f;
        float timer = 0;

        // Go into fov
        while (timer < 0.1f)
        {
            cam.fieldOfView = Mathf.Lerp(camDefaultFOV, 80, timer / 0.1f);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }

        // Default MaxFrenzy has the value of ten
        // I want every 10 Frenzy to give 3 seconds
        yield return new WaitForSeconds(owner.sharkScriptableObject.maxFrenzy / 10 * 3); 

        timer = 0;
        while (timer < 0.5f)
        {
            owner.currentFrenzy = Mathf.Lerp(owner.sharkScriptableObject.maxFrenzy, 0, timer / 0.5f);
            cam.fieldOfView = Mathf.Lerp(80, camDefaultFOV, timer / 0.5f);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }


        owner.currentFrenzy = 0;
        MoveSpeedMultiplier = 1f;
        cam.fieldOfView = camDefaultFOV;
        InFrenzy = false;
        SignalBus.OnFrenzyEndInvoke();
    }

    public void HandleFrenzy()
    {
        if (InFrenzy) return;

        if (owner.currentFrenzy > 0)
        {
            owner.currentFrenzy -= Time.fixedDeltaTime * 0.0764f;
            if (owner.currentFrenzy < 0.001f)
            {
                owner.currentFrenzy = 0;
            }
        }
    }
}
