using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D collider) {
        if(collider.gameObject.CompareTag("Lava")) {
            Health.onPlayerDeath?.Invoke();
            gameObject.SetActive(false);
        }
    }
}