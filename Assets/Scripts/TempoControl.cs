using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TempoControl : MonoBehaviour
{
    public float changePeriod = .5f; // Seconds it takes to change to target tempo
    public float changeBy = 5;
    private float curTarget = MusicTimeManager.tempo;
    IEnumerator ChangeTempo(float targetTempo)
    {
        curTarget = targetTempo;
        // Ensure that the tempo isn't out of wack
        targetTempo = Mathf.Clamp(targetTempo, MusicTimeManager.MIN_TEMPO, MusicTimeManager.MAX_TEMPO);
        float startTempo = MusicTimeManager.tempo;
        // If negative, moveTowards function will instead move AWAY from target tempo indefinitely.
        float delta = Mathf.Abs(targetTempo - startTempo);
        // Bring tempo closer to target tempo until equal
        while(MusicTimeManager.tempo != targetTempo)
        {
            float nextTempo = Mathf.MoveTowards(MusicTimeManager.tempo, targetTempo, 
              delta * (Time.deltaTime / changePeriod));
            MusicTimeManager.instance.SetTempo(nextTempo);
            yield return new WaitForEndOfFrame();
        }
    }

    public void HandleInput(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            StopAllCoroutines();
            StartCoroutine(ChangeTempo(curTarget + 5 * context.ReadValue<float>()));
        }
    }

}
