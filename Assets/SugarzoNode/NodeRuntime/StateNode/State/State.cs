using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;

namespace SugarFrame.Node
{
    public enum EState
    {
        [LabelText("未执行")]
        None,
        [LabelText("正在进入")]
        Enter,
        [LabelText("正在执行")]
        Running,
        [LabelText("正在退出")]
        Exit,
        [LabelText("执行完成")]
        Finish,
    }
    public interface IStateEvent
    {
        void Execute();
        void OnEnter();
        void OnRunning();
        void OnExit();
    }

    public abstract class NodeState : MonoBehaviour
    {
#if UNITY_EDITOR
        [HideInInspector]
        public Vector2 nodePos;

        [LabelText("节点注释"), OnValueChanged(nameof(UpdateNodeName))]
        public string explanatoryNote = "";

        [HideInInspector]
        public UnityEditor.Experimental.GraphView.Node node;

        private void UpdateNodeName()
        {
            node.title = GetNodeName();
        }

        public string GetNodeName()
        {
            if (!string.IsNullOrWhiteSpace(explanatoryNote))
                return explanatoryNote;

            string ret =  GetType().Name;
            if (GetType().IsDefined(typeof(NodeNoteAttribute), true))
                ret += "\n" + (System.Attribute.GetCustomAttribute(GetType(), typeof(NodeNoteAttribute)) as NodeNoteAttribute).note;
            //ret += string.IsNullOrEmpty(explanatoryNote) ? "" : "\n" + explanatoryNote;

            return  ret;
        }
#endif
        public virtual EState State { get { return EState.None; } }
        //流向下一节点的流
        public MonoState nextFlow;
    }

    public abstract class MonoState : NodeState, IStateEvent
    {
        [SerializeField,Space]
        protected EState state;

        [TextArea,Space]
        public string note;

        protected void TransitionState(EState _state)
        {
            state = _state;
            switch (state)
            {
                case EState.Enter:
                    OnEnter();
                    break;
                case EState.Running:
                    OnRunning();
                    break;
                case EState.Exit:
                    OnExit();
                    break;
            }
#if UNITY_EDITOR
            //显示Editor中Graph颜色
            if(node != null)
            {
                Color runningColor = new Color(0.37f, 1, 1, 1f); //浅蓝
                Color compeletedColor = new Color(0.5f, 1, 0.37f, 1f); //浅绿
                Color portColor = new Color(0.41f, 0.72f, 0.72f, 1f); //灰蓝

                if (State == EState.Running || State == EState.Enter || State == EState.Exit)
                {
                    node.titleContainer.style.backgroundColor = new StyleColor(runningColor);
                }
                if (State == EState.Finish)
                {
                    node.titleContainer.style.backgroundColor = new StyleColor(compeletedColor);
                }
            }
#endif
        }

        public virtual void Execute()
        {
            TransitionState(EState.Enter);
        }
        public virtual void OnEnter()
        {
            TransitionState(EState.Running);
        }
        public virtual void OnRunning()
        {
            TransitionState(EState.Exit);
        }
        public virtual void OnExit()
        {
            TransitionState(EState.Finish);
        }

        public override EState State
        { 
            get 
            { 
                return state; 
            } 
        }
    }
}


