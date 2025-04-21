using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using System.IO;

public class SpinValidationTests
{
    private WheelGenerator wheelGenerator;
    private WheelSpinner wheelSpinner;
    List<WheelGenerator.WheelSlice> testSlices;

    [SetUp]
    public void Initialize()
    {
        // Set up a test wheel
        GameObject wheel = new GameObject("TestWheel");
        wheelGenerator = wheel.AddComponent<WheelGenerator>();

        testSlices = new List<WheelGenerator.WheelSlice>
        {
            new("Life 30 min", 0.2f),
            new("Brush 3X", 0.1f),
            new("Gems 35", 0.1f),
            new("Hammer 3X", 0.1f),
            new("Coins 750", 0.05f),
            new("Brush 1X", 0.2f),
            new("Gems 75", 0.05f),
            new("Hammer 1X", 0.2f),
        };
        wheelGenerator.SetTestSlices(testSlices);
        wheelGenerator.InitializeWheel();

        // Initialize spin settings
        wheelSpinner = wheel.AddComponent<WheelSpinner>();
        wheelSpinner.InitializeValues(3, 10, 360f, 3f, wheelGenerator);
    }

    [Test]
    public void SpinToPrize_LandsOnCorrectSlice()
    {
        // Make sure our wheel is valid
        Assert.IsTrue(wheelGenerator.IsWheelValid());

        float numSlices = testSlices.Count;
        float sliceAngle = 360f / numSlices;

        // Repeat however many times
        for (int j = 0; j < 500; j++)
        {
            // Test each prize
            for (int i = 0; i < testSlices.Count; i++)
            {
                float minSliceAngle = sliceAngle * i + wheelSpinner.GetArrowOffset();
                float maxSliceAngle = sliceAngle * i + sliceAngle;

                // Retrieve spin angle
                float result = wheelSpinner.SpinToPrizeAndGetFinalAngle(i);

                // Subtract full spins
                result %= 360f;

                // Make sure the wheel is rotated to the correct sector
                Assert.That(result, Is.InRange(minSliceAngle, maxSliceAngle));
                // Confirm this is the correct prize
                Assert.AreEqual(testSlices[i].winString, wheelSpinner.GetReward(i));
            }
        }
    }

    [Test]
    public void SpinEmulator_RecordResults()
    {
        // Make sure our wheel is valid
        Assert.IsTrue(wheelGenerator.IsWheelValid());

        // Initialize prize types and counts
        Dictionary<string, int> prizeCounts = new Dictionary<string, int>();
        foreach (var slice in testSlices)
        {
            prizeCounts[slice.winString] = 0;
        }

        // Emulate 1000 spins
        for (int i=0; i<1000; i++)
        {
            // Pick a weighted random prize and retrieve the reward
            string prizeString = wheelSpinner.GetRandomPrizeString();
            prizeCounts[prizeString] += 1; // Record prize
        }

        // Write to file (overwrites by default)
        string path = "Assets/Tests/";
        string fileName = "SpinEmulatorTestResults.txt";

        StreamWriter writer = new StreamWriter(Path.Combine(path, fileName));
        string headerLine = "\tPrize\t\t\tCount\n----------------------------";
        writer.WriteLine(headerLine);

        foreach (KeyValuePair<string, int> prize in prizeCounts)
        {
            string formattedString = prize.Key + "\t\t\t" + prize.Value;
            writer.WriteLine(formattedString);
        }

        writer.Close();
    }
}
