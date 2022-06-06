using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionDataComparer : IEqualityComparer<Dropdown.OptionData>
{
    public bool Equals(Dropdown.OptionData x, Dropdown.OptionData y)
    {
        return x.text.Equals(y.text, StringComparison.InvariantCultureIgnoreCase);
    }

    public int GetHashCode(Dropdown.OptionData obj)
    {
        return obj.text.GetHashCode();
    }
}

public class DropdownScreenResolution : MonoBehaviour
{
    //Current resolution text
    public Text text;

    [SerializeField]
    private Resolution[] _resolutions;

    [SerializeField]
    private Dropdown _dropdown; 

    protected void Start()
    {     
        //Get resolutions and dropdown component
        _resolutions = Screen.resolutions;
        _dropdown = GetComponent<Dropdown>();

        //Clear previous resolution variants if we have it
        _dropdown.options.Clear();

        //Add onValueChanged listener 
        _dropdown.onValueChanged.AddListener(delegate {
            //Set new resolution
            Screen.SetResolution(_resolutions[_dropdown.value].width, _resolutions[_dropdown.value].height, Screen.fullScreen);

            //Print current resolution if text not null
            if (text != null)
                text.text = $"Current resolution: {Screen.width} x {Screen.height}";

        });

        //Create HashSet for avoid duplicates
        HashSet<Dropdown.OptionData> uniqueList = new HashSet<Dropdown.OptionData>(new OptionDataComparer());

        //Get all screen resolution variants
        for (int i = 0; i < _resolutions.Length; i++)
        {
            Dropdown.OptionData op = new Dropdown.OptionData();
            op.text = _resolutions[i].width + " x " + _resolutions[i].height;
            
            _dropdown.value = i;
            uniqueList.Add(op);
        }

        //Print current resolution if text not null
        if (text != null)
            text.text = $"Current resolution: {Screen.width} x {Screen.height}";


        _dropdown.options = uniqueList.ToList();

        //Set current screen resolution 
        Dropdown.OptionData currentValue = new Dropdown.OptionData();
        currentValue.text = Screen.width + " x " + Screen.height;
        _dropdown.value = _dropdown.options.Count;
        _dropdown.options.Add(currentValue);
    }
}
