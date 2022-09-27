using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ToyoPersonaSO", menuName = "ScriptableObject/ToyoPersonaSO")]
public class ToyoPersonaSO : ScriptableObject
{
    public string objectId_dev;
    public string objectId_prod;
    public string toyoName;
    [FormerlySerializedAs("toyoImage")] public GameObject toyoPrefab;
    
}
