using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceWhiteScript : MonoBehaviour
{

    Dropdown m_Dropdown;
    string m_Message;
    //public Text m_Text;
    int m_DropdownValue;

    private int WhiteArmy = 0;


    // Start is called before the first frame update
    void Start()
    {
        m_Dropdown = GetComponent<Dropdown>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeWhiteFraction()
    {
        WhiteArmy = m_Dropdown.value;
        StaticNameController.fractionWhite = WhiteArmy;
    }
    
}
