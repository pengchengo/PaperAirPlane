using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SugarFrame.Node
{
    public enum NodePackageType
    {
        Base,
        SugarzoFrame
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false,Inherited =true)]
    public class NodeNoteAttribute : System.Attribute
    {
        public string note;
        public NodePackageType packageType;

        public NodeNoteAttribute(string _note = "",NodePackageType _packageType = NodePackageType.Base)
        {
            note = _note;
            packageType = _packageType;
        }
    }
}
