using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ToyoPersonaSO", menuName = "ScriptableObject/ToyoPersonaSO")]
public class ToyoPersonaSO : ScriptableObject
{
    public string objectId;
    [FormerlySerializedAs("toyoImage")] public GameObject toyoPrefab;
    
}
