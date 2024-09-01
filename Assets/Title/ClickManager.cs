using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickManager : MonoBehaviour
{
    public void AddPlayerNum(Text textComponent)
    {
        if (textComponent == null) return;

        if (int.TryParse(textComponent.text, out int number) && number < 4)
        {
            number++;
            textComponent.text = number.ToString();
        }
        else
        {
            return;
        }
    }
    public void SubstractPlayerNum(Text textComponent)
    {
        if (textComponent == null) return;

        if (int.TryParse(textComponent.text, out int number) && number > 0)
        {
            number--;
            textComponent.text = number.ToString();
        }
        else
        {
            return;
        }
    }
}
