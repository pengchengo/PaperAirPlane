using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SugarFrame.Tool;

namespace SugarFrame.Node
{
    [CreateAssetMenu(menuName = "SugarzoFrame/EditorTool/NodeScriptCreator")]
    public class NodeScriptCreator : ScriptCreator
    {
        [Space]
        [LabelText("节点说明")]
        public string nodeNote = "";
        public NodePackageType packageType;
        [Space,PropertyOrder(110)]
        public List<PackagePath> packagePaths = new List<PackagePath>();

        public override void Create()
        {
            createPath = packagePaths.Find(x => x.packageType == packageType).path;
            base.Create();
        }

        protected override void OnValidate()
        {
            if (prototype)
            {
                code = prototype.ToString().Replace("#TTT#", createFileName).Replace("#T1#",nodeNote).Replace("#T2#",packageType.ToString());
            }
            else
            {
                code = "缺少脚本的模板文件";
            }
        }
    }

    [Serializable]
    public class PackagePath
    {
        public NodePackageType packageType;
        [FolderPath]
        public string path;
    }
}