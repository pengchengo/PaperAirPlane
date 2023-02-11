using Sirenix.OdinInspector;
using UnityEngine;

namespace SugarFrame.Tool
{
    public interface IAssetCreator
    {
        public void Create();
    }

    public abstract class BaseAssetCreator : ScriptableObject, IAssetCreator
    {
        [FolderPath]
        public string createPath;
        [Space, TextArea]
        public string createFileName;

        [Button]
        public abstract void Create();

        protected bool IsEmptyVariable()
        {
            return string.IsNullOrEmpty(createPath) || string.IsNullOrEmpty(createFileName);
        }
    }
}