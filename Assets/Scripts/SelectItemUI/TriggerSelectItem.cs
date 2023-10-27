using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSelectItem : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject self;
    void Start() {
        self = gameObject;
    }

    // Update is called once per frame
    void Update() {
        
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            // save player's next item
            PlayerPrefs.SetString("Item", self.name);
            PlayerPrefs.Save();
            // to get item, you should use PlayerPrefs.GetString
            // string item = PlayerPrefs.GetString("Item", "DefaultName");
            Destroy(collision.gameObject);
            Destroy(self);
        }
    }
}
