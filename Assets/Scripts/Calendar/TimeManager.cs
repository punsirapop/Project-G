using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Pool;
using System.Linq;

public class TimeManager : MonoBehaviour
{
    [System.Serializable]
    public struct Date
    {
        public int year;
        public int month;
        public int day;

        public void InitDate()
        {
            year = 2700;
            month = 1;
            day = 1;
        }

        public Date AddDay(int d)
        {
            int carryOver = (day + d) / 28;
            int addDay = (day + d) % 28;
            day = addDay;
            AddMonth(carryOver);

            if (day == 0)
            {
                day = 28;
                month--;
            }

            return this;
        }

        public Date AddMonth(int m)
        {
            int carryOver = (month + m) / 12;
            int addMonth = (month + m) % 12;
            month = addMonth;
            AddYear(carryOver);

            if (month == 0)
            {
                month = 12;
                year--;
            }

            return this;
        }

        public Date AddYear(int y)
        {
            year += y;

            return this;
        }

        public int ToDay()
        {
            return day + month * 28 + year * 12 * 28;
        }

        public static Date FromDay(int days)
        {
            Date zeroDate = new Date()
            {
                day = 0,
                month = 0,
                year = 0
            };
            return zeroDate.AddDay(days);
        }

        public int CompareDate(Date d)
        {
            int a = ToDay();
            int b = d.ToDay();
            return a - b;
        }

        public string ShowDate()
        {
            return String.Join("/", String.Format("{0:00}", day), String.Format("{0:00}", month), year);
        }

        public Date DupeDate()
        {
            Date d = new Date();
            d.day = day;
            d.month = month;
            d.year = year;
            return d;
        }
    }

    public static event Action<Date> OnChangeDate;

    ObjectPool<GameObject> _Pool;
    Date _MyDate;
    Date _SkipDate;

    // [SerializeField] TextMeshProUGUI _SkipDayDis;
    // [SerializeField] TextMeshProUGUI _SkipDayLabel;
    // [SerializeField] Button[] _SkipDayAdjustors;

    [SerializeField] Transform _CellHolder, _NotifHolder;
    [SerializeField] GameObject _CellPrefab, _NotifPrefab;
    [SerializeField] GameObject[] _SkipButtons;
    [SerializeField] GameObject[] _MonthButtons;
    [SerializeField] TextMeshProUGUI _SkipLabel;
    [SerializeField] TextMeshProUGUI[] _DateLabel;

    private void Awake()
    {
        ResetSkip();
        ResetDateSelection();

        _MyDate = PlayerManager.CurrentDate.DupeDate();

        _DateLabel[0].text = _MyDate.month.ToString();
        _DateLabel[1].text = _MyDate.year.ToString();

        _Pool = new ObjectPool<GameObject>(
            () => Instantiate(_CellPrefab, _CellHolder),
            cell =>
            {
                cell.SetActive(true);
                cell.transform.SetAsLastSibling();
            },
            cell =>
            {
                cell.SetActive(false);
            },
            cell => Destroy(cell),
            false, 28, 28
            );

        _MonthButtons[0].SetActive(_MyDate.month > 1);
        _MonthButtons[1].SetActive(_MyDate.month < 12);
        LoadCells(_MyDate);
    }

    private void LoadCells(Date d)
    {
        foreach (Transform item in _CellHolder)
        {
            _Pool.Release(item.gameObject);
        }
        for (int i = 1; i < 29; i++)
        {
            Date date = d.DupeDate();
            date.day = i;
            _Pool.Get().GetComponent<CalendarCell>().SetCell(date);
        }
    }

    private void Update()
    {
        // _SkipDayDis.text = _SkipDay.ToString();
        // _SkipDayLabel.text = (_SkipDay > 1) ? "Skipping\n\nDays" : "Skipping\n\nDay";
        // _SkipDayAdjustors[0].interactable = _SkipDay < 10;
        // _SkipDayAdjustors[1].interactable = _SkipDay > 1;
    }

    public void AdjustMonth(bool fwd)
    {
        _MyDate.AddMonth(fwd ? 1 : -1);
        _DateLabel[0].text = _MyDate.month.ToString();
        LoadCells(_MyDate);
        _MonthButtons[0].SetActive(_MyDate.month > 1);
        _MonthButtons[1].SetActive(_MyDate.month < 12);
    }

    public void ChangeDateSelection(Date d)
    {
        foreach (var item in _SkipButtons)
        {
            item.SetActive(false);
        }

        if (d.CompareDate(PlayerManager.CurrentDate) < 1)
        {
            _SkipButtons[0].SetActive(true);
            ResetSkip();
        }
        else
        {
            _SkipButtons[1].SetActive(true);
            int i = d.CompareDate(PlayerManager.CurrentDate);
            _SkipLabel.text = String.Join(" ", "Skipping", i, (i > 1) ? "days" : "day");
            _SkipDate = d.DupeDate();
        }

        foreach (Transform item in _NotifHolder)
        {
            Destroy(item.gameObject);
        }

        List<SideQuestSO> l = PlayerManager.SideQuestDatabase.GetAllAcquiredQuest().ToList();
        if (l.Select(x => x.DueDate).Contains(d))
        {
            SideQuestSO sq = l.Find(x => x.DueDate.CompareDate(d) == 0);
            TextMeshProUGUI[] t = Instantiate(_NotifPrefab, _NotifHolder).GetComponentsInChildren<TextMeshProUGUI>();
            t[0].text = "Side Quest";
            t[1].text = sq.Name;
        } 
    }

    public void ResetCalendar()
    {
        _MyDate = PlayerManager.CurrentDate.DupeDate();
        LoadCells(_MyDate);
    }

    public void ResetDateSelection()
    {
        CalendarCell.SelectedDate = default(Date);
        ChangeDateSelection(CalendarCell.SelectedDate);
    }

    public void ResetSkip()
    {
        _SkipDate = PlayerManager.CurrentDate.DupeDate();
        _SkipDate.AddDay(1);
    }

    public void SkipDay()
    {
        OnChangeDate?.Invoke(_SkipDate);
        _SkipDate.AddDay(1);
        CalendarCell.SelectedDate = default(Date);
    }
}
