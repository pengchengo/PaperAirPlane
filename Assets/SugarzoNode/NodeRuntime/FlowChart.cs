using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SugarFrame.Node
{
    [DisallowMultipleComponent]
    public class FlowChart:MonoBehaviour
    {

#if UNITY_EDITOR
        [TextArea]
        public string note;

        [UnityEditor.MenuItem("GameObject/GraphObject", false, priority = 0)]
        public static GameObject CreateFlowChartInScene()
        {
            var gameObject = new GameObject(typeof(FlowChart).Name);

            if (Selection.activeGameObject != null)
            {
                gameObject.transform.parent = Selection.activeGameObject.transform;
            }
            //当前处于预制件模式
            else if (PrefabStageUtility.GetCurrentPrefabStage() is PrefabStage prefabStage)
            {
                gameObject.transform.parent = prefabStage.prefabContentsRoot.transform;
                EditorUtility.SetDirty(prefabStage.prefabContentsRoot);
            }
            
            UnityEditor.Undo.RegisterCreatedObjectUndo(gameObject, "New FlowChart");
            gameObject.AddComponent<FlowChart>();

            return Selection.activeGameObject = gameObject;
        }
#endif
    }

}
