using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Color TwistHit/CharacterData", order = 0)]
public class CharacterData : ScriptableObject {

    public string _characterName = "";
    public int _characterUnlockPrice = 0;
    public bool _characterUnlocked = false;

    public void UnlockCharacter(bool isFreeUnlock = false) {
        if (!isFreeUnlock) {
            Debug.LogError($"{this.name}  Character Unlocked by spending {this._characterUnlockPrice} Gems");
            Debug.Log($"Remaining Gems : {LevelManager.TotalGems}");
        } else {
            Debug.Log($"FREE CHARACTER UNLOCKED : {this.name}");
        }

        this._characterUnlocked = true;
    }

    public void ResetAndLockCharacter() {
        this._characterUnlocked = false;
    }



}