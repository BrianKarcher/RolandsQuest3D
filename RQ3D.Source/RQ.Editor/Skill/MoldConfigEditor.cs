using RQ.Base.Skill;
using UnityEditor;

namespace RQ.Editor.Skill
{
    [CustomEditor(typeof(MoldConfig), true)]
    public class MoldConfigEditor : ConfigEditorBase<MoldConfig>
    {
        [MenuItem("Assets/Create/RQ/Skill/Mold Config")]
        public static void CreateNewAsset()
        {
            CreateAsset("NewMoldConfig.asset");
        }
    }
}
