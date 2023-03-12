using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RouletteWheel : MonoBehaviour
{
    [SerializeField] private GameObject _SectionPrefab;
    [SerializeField] private Transform _SpinnableWheel;
    [SerializeField] private float _FullSpinRound;      // The number of full round spinning before the wheel stop
    [SerializeField] private float _SpinTime;           // Total spinning time for 1 random (second)
    private float _DestinationAngle;                    // The rotation of the wheel after the spin end (degree)
    private float _SpinSpeed;                           // Spin speed (degree/second)
    [SerializeField] private float _SpinAccerelation;   // The angular accerelation of the wheel spinning (degree/second)
    private bool _IsSpinning;
    private Color32[] _Colors;
    private int[] _CumulativeFitness;
    private int _SpinningResult;

    void Start()
    {
        _IsSpinning = false;
    }

    void Update()
    {
        if (_IsSpinning && _SpinSpeed > 0)
        {
            _SpinnableWheel.Rotate(0, 0, _SpinSpeed * Time.deltaTime);
            _SpinSpeed += _SpinAccerelation * Time.deltaTime;
        }
        if (_IsSpinning && _SpinSpeed <= 0)
        {
            // Stop the spinning and precisely snap it to the rotation it should be
            _SpinSpeed = 0;
            _IsSpinning = false;
            _SpinnableWheel.transform.SetPositionAndRotation(_SpinnableWheel.transform.position, Quaternion.Euler(0, 0, _DestinationAngle));
            CandidateManager.Instance.AddParentFromWheel(_SpinningResult);
        }
    }

    public void SetWheel(ChromosomeRodValue[] chromosomeRodValues)
    {
        // Destroy all previous object (if any)
        foreach (Transform child in _SpinnableWheel)
        {
            Destroy(child.gameObject);
        }
        // Calculate each section's color and size (cumulative fitness)
        int chromosomeCount = chromosomeRodValues.Length;
        _Colors = new Color32[chromosomeCount];
        _CumulativeFitness = new int[chromosomeCount];
        _Colors[0] = chromosomeRodValues[0].gameObject.GetComponentInChildren<ChromosomeRod>().GetColorAtIndex(0);
        _CumulativeFitness[0] = chromosomeRodValues[0].Value;
        for (int i = 1; i < chromosomeCount; i++)
        {
            _Colors[i] = chromosomeRodValues[i].gameObject.GetComponentInChildren<ChromosomeRod>().GetColorAtIndex(0);
            _CumulativeFitness[i] = _CumulativeFitness[i - 1] + chromosomeRodValues[i].Value;
        }
        // Create actual sect GameObject on the wheel
        for (int i = chromosomeCount - 1; i >= 0; i--)
        {
            GameObject newSection = Instantiate(_SectionPrefab, _SpinnableWheel);
            newSection.GetComponent<Image>().color = _Colors[i];
            newSection.GetComponent<Image>().fillAmount = (float)_CumulativeFitness[i] / (float)_CumulativeFitness[chromosomeCount - 1];
        }
    }

    public void SpinWheel()
    {
        // Proportionate random
        int totalFitness = _CumulativeFitness[_CumulativeFitness.Length - 1];
        int randomValue = Random.Range(0, totalFitness + 1);
        for (int i = 0; i < _CumulativeFitness.Length; i++)
        {
            if (randomValue <= _CumulativeFitness[i])
            {
                _SpinningResult = i;
                break;
            }
        }
        // Calculate angle and accerelation that make the wheel stop at preferred angle
        _DestinationAngle = 360 * (float)randomValue / (float)totalFitness;
        float currentAngle = _SpinnableWheel.transform.rotation.eulerAngles.z;
        float angleToSpin = 360 * _FullSpinRound;
        angleToSpin += (_DestinationAngle > currentAngle) ? (_DestinationAngle - currentAngle) : (360 + _DestinationAngle - currentAngle);
        _SpinSpeed = 2 * angleToSpin / _SpinTime;
        _SpinAccerelation = (0 - _SpinSpeed) / _SpinTime;
        _IsSpinning = true;
        
        //_SpinnableWheel.Rotate(0, 0, angleToSpin);
    }
}
