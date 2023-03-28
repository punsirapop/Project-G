using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CalendarCell : MonoBehaviour
{
    public static TimeManager.Date SelectedDate;

    [SerializeField] TextMeshProUGUI _DateDisplay;
    [SerializeField] Button _CellButton;
    [SerializeField] Transform _EventHolder;
    [SerializeField] GameObject _EventPrefab;
    [SerializeField] GameObject[] _Indicators;

    public TimeManager.Date MyDate;

    private void Update()
    {
        _Indicators[1].SetActive(MyDate.Equals(SelectedDate));
    }

    public void SetCell(TimeManager.Date d)
    {
        MyDate = d;
        _DateDisplay.text = MyDate.day.ToString();
        switch (MyDate.CompareDate(PlayerManager.CurrentDate))
        {
            case < 0:
                _Indicators[0].SetActive(true);
                break;
            case 0:
                _Indicators[2].SetActive(true);
                break;
            default:
                break;
        }
    }

    public void ShowDate()
    {
        Debug.Log(MyDate.ShowDate());
    }

    public void ToggleSelect()
    {
        if (MyDate.Equals(SelectedDate))
        {
            SelectedDate = default(TimeManager.Date);
        }
        else
        {
            SelectedDate = MyDate.DupeDate();
        }

        transform.parent.parent.SendMessage("ChangeDateSelection", SelectedDate);
    }
}
