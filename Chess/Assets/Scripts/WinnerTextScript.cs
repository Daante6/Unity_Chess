using UnityEngine;
using UnityEngine.UI;

public class WinnerTextScript : MonoBehaviour
{
    public Text WinnerText;
    
    // Update is called once per frame
    void Update()
    {
        WinnerText.text = StaticNameController.Winner;
    }
}
