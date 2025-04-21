using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelSpinner : MonoBehaviour
{
    [Header("Spin Settings")]
    [SerializeField] private int minSpins;
    [SerializeField] private int maxSpins;
    [Tooltip("degrees/sec")]
    [SerializeField] private float spinSpeed;
    [Tooltip("how many degrees the arrow is offset from 0")]
    [SerializeField] private float wheelArrowOffset;

    [Header("References")]
    [SerializeField] private Transform wheelObject;
    [SerializeField] private WheelGenerator wheelInfo;

    private bool _isSpinning = false;

#if UNITY_EDITOR
    public void InitializeValues(int minSpins, int maxSpins, float spinSpeed, float wheelArrowOffset, WheelGenerator wheelInfo) // For unit tests
    {
        this.minSpins = minSpins;
        this.maxSpins = maxSpins;
        this.spinSpeed = spinSpeed;
        this.wheelArrowOffset = wheelArrowOffset;
        this.wheelInfo = wheelInfo;
    }
#endif

    public void Spin()
    {
        if (!wheelInfo.IsWheelValid())
        {
            Debug.Log("Wheel is not valid");
            return;
        }
        if (_isSpinning)
        {
            Debug.Log("Spin is already in progress");
            return;
        }

        // Pick a weighted random prize
        int randomPrize = GetRandomPrizeIndex();
        if (randomPrize >= 0 && randomPrize < wheelInfo.GetNumberOfSlices()) // Sanity check in case we ever get an out-of-range value
            SpinToSlice(randomPrize);
    }

    private int GetRandomPrizeIndex()
    {
        // Retrieve a weighted random prize
        float rand = Random.Range(0f, 1f); // Get a random percentage
        for (int i = 0; i < wheelInfo.GetNumberOfSlices(); i++)
        {
            // If our random percentage falls within this prize's drop range, return it
            if (rand < wheelInfo.GetDropRate(i))
            {
                return i;
            }
            rand -= wheelInfo.GetDropRate(i);
        }

        return -1; // This should never be possible
    }

    private void SpinToSlice(int prizeIndex)
    {
        _isSpinning = true;

        // Find the slice associatd with this prize and rotate the wheel to a random point in it
        float targetAngle = GetRandomSpinAngleToSlice(prizeIndex);

        // Rotate to target spin angle
        wheelObject.DORotate(Vector3.forward * targetAngle, spinSpeed, RotateMode.FastBeyond360)
            .SetSpeedBased()
            .SetEase(Ease.OutCirc)
            .OnComplete(() => EndSpin(prizeIndex)); // Take care of any logic cleanup
    }

    private float GetRandomSpinAngleToSlice(int prizeIndex)
    {
        float sliceAngle = wheelInfo.GetSliceAngle();

        // Make a random number of full spins
        int numSpins = Random.Range(minSpins, maxSpins);
        // Spin to the correct slice
        float angleToSlice = sliceAngle * prizeIndex;
        // Spin to a random point within the slice
        float randomAngleInSlice = Random.Range(wheelArrowOffset, sliceAngle);

        return numSpins * 360 + angleToSlice + randomAngleInSlice;
    }

    private void EndSpin(int prizeIndex)
    {
        _isSpinning = false;

        Debug.Log("You won " + GetReward(prizeIndex));
    }

#if UNITY_EDITOR
    // Wrappers for unit tests
    public float SpinToPrizeAndGetFinalAngle(int prizeIndex)
    {
        return GetRandomSpinAngleToSlice(prizeIndex);
    }
    public string GetRandomPrizeString()
    {
        return GetReward(GetRandomPrizeIndex());
    }
#endif

    public string GetReward(int prizeIndex)
    {
        return wheelInfo.GetReward(prizeIndex);
    }

    public float GetArrowOffset()
    {
        return wheelArrowOffset;
    }
}
