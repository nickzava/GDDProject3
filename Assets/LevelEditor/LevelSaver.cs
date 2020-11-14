using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelSaver : MonoBehaviour
{

    public void SaveLevel(GameObject level, string path)
    {
        Destroy(level.GetComponent<LevelGenerator>());

        IEnumerator SaveLevel()
        {
            yield return new WaitForSeconds(0.1f);
            PrefabUtility.SaveAsPrefabAssetAndConnect(level, path,InteractionMode.AutomatedAction);
            Destroy(gameObject);
        }
        StartCoroutine(SaveLevel());
    }
}
