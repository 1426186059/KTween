using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using static SimpleTween;

[CustomEditor(typeof(SimpleTweenMgr)), CanEditMultipleObjects]
public class SimpleTweenMgrEditorEditor : Editor
{
    private SimpleTweenMgr mTarget;
    private void OnEnable()
    {
        mTarget = target as SimpleTweenMgr;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawInspectorGUI();
        serializedObject.ApplyModifiedProperties();
    }

    protected void DrawInspectorGUI()
    {
        DrawMyInspector();
    }

    private Dictionary<TweenItem, bool> mFoldOutDic = new Dictionary<TweenItem, bool>();
    private void DrawMyInspector()
    {
        var mChildrenList = Get_mAssetDic();
        EditorGUILayout.LabelField("Tween Count: " + mChildrenList.Count);

        EditorGUI.indentLevel = 0;
        for (int i = 0; i < mChildrenList.Count; i++)
        {
            EditorGUI.indentLevel = 0;
            TweenItem mItem = mChildrenList[i];

            if (!mFoldOutDic.ContainsKey(mItem))
            {
                mFoldOutDic.Add(mItem, false);
            }

            mFoldOutDic[mItem] = EditorGUILayout.Foldout(mFoldOutDic[mItem], i + " " +  mItem.GetType().Name);
            if (mFoldOutDic[mItem])
            {
                EditorGUI.indentLevel = 2;
                foreach (FieldInfo v1 in mItem.GetType().GetFields())
                {
                    EditorGUILayout.LabelField(v1.Name + " : " + v1.GetValue(mItem)?.ToString());
                }
            }
        }
    }

    private List<TweenItem> Get_mAssetDic()
    {
        var mChildrenListFieldInfo1 = mTarget.GetType().GetField("mManager", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic);
        var mManager = mChildrenListFieldInfo1.GetValue(mTarget);
        
        var mChildrenListFieldInfo = mManager.GetType().GetField("mTweenT", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic);
        var mChildrenList = mChildrenListFieldInfo.GetValue(mManager);

        if (mChildrenList is LinkedList<TweenItem>)
        {
            LinkedList<TweenItem> mChildrenList1 = mChildrenList as LinkedList<TweenItem>;

            List<TweenItem> mList = new List<TweenItem>();
            var A = mChildrenList1.GetEnumerator();
            while (A.MoveNext())
            {
                mList.Add(A.Current);
            }
            return mList;
        }
        else
        {
            return mChildrenList as List<TweenItem>;
        }
    }

}