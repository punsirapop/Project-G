using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationDrawer : MonoBehaviour
{
    [SerializeField] private InformationSO _Information;
    [SerializeField] private GameObject _InformationRendererPrefab;
    [SerializeField] private Transform _Canvas;

    private void Start()
    {
        if (_Information.IsNeverShow)
        {
            DrawInformation();
            _Information.Shown();
        }
    }

    public void DrawInformation()
    {
        GameObject newInformation = Instantiate(_InformationRendererPrefab, _Canvas);
        newInformation.GetComponent<InformationRenderer>().SetInformation(_Information);
    }
}
