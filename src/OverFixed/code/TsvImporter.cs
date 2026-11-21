using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace OverFixed
{
    // Unity는 .tsv를 TextAsset으로 읽지 않으므로 직접 TextAsset으로 가져온다
    [ScriptedImporter(1, "tsv")]
    public class TsvImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext context)
        {
            TextAsset textAsset = new TextAsset(File.ReadAllText(context.assetPath));
            context.AddObjectToAsset("text", textAsset);
            context.SetMainObject(textAsset);
        }
    }
}
