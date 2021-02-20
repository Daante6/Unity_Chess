using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceBlackScript : MonoBehaviour
{

    Dropdown m_Dropdown;
    string m_Message;
    //public Text m_Text;
    int m_DropdownValue;

    private int BlackArmy = 0;


    // Start is called before the first frame update
    void Start()
    {
        m_Dropdown = GetComponent<Dropdown>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeBlackFraction()
    {
        BlackArmy = m_Dropdown.value;
        StaticNameController.fractionBlack = BlackArmy;
    }

}
