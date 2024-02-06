using GalEngine.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameRoot:MonoBehaviour
{
    public Button parseBtn;
    public TextMeshProUGUI commandListText;

    private void Awake()
    {
        parseBtn.onClick.AddListener(() => {
            var result = CommandParser.ParseScriptFile(Application.dataPath + "/StoryScripts/test.txt");
            commandListText.text = string.Empty;
            foreach (var command in result.commands)
            {
                commandListText.text += command.ToString();
                commandListText.text += "\n";
            }
        });
    }
}