using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EligibleStreetsApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Grab file 
            string filePath = @"C:\Code\parking-enforcement-service\src\Data\EligibleStreets\EligibleStreets2021_Mike.json";
            //Grab each line 
            List<string> lines = File.ReadLines(filePath).ToList();

            List<string> newData = new();

            foreach (string line in lines)
            {
                if (line.Contains("House Numbers"))
                {
                    var rawHouseNumberData = line.Replace("\"House Numbers\": \" ", "").Replace(" \",", "");
                    var numbers = rawHouseNumberData
                        .Substring(rawHouseNumberData.IndexOf(':') + 1)
                        .Replace(@"""", "")
                        .Replace(@"\", "")
                        .Replace(@"n", "")
                        .Split(',');

                    StringBuilder newLine = new StringBuilder("    \"HouseNumbers\": [ ");

                    for (int i = 0; i < numbers.Length; i++)
                    {
                        if (i.Equals(numbers.Length - 1))
                        {
                            GenerateFormatFoLastNumber(numbers, newLine, i);
                        }
                        else
                        {
                            GenerateFormatForNumberExceptLastNumber(numbers, newLine, i);
                        }
                    }
                    newData.Add(newLine.ToString());
                }
                else
                {
                    newData.Add(line);
                }
            }

            string newFilePath = @$"C:\Code\parking-enforcement-service\src\Data\EligibleStreets\EligibleStreets{DateTime.Now:yyyyMMdd}.json";
            //Produce new json 
            File.WriteAllLines(newFilePath, newData);
            Console.WriteLine("Created!");
            Console.ReadLine();
        }

        private static void GenerateFormatForNumberExceptLastNumber(string[] numbers, StringBuilder newLine, int i)
        {
            if (!numbers[i].Trim().Equals("House Numbers:") && !string.IsNullOrEmpty(numbers[i].Trim()))
                newLine.Append($"\"{numbers[i].Trim()}\",");
        }

        private static void GenerateFormatFoLastNumber(string[] numbers, StringBuilder newLine, int i)
        {
            if (!string.IsNullOrEmpty(numbers[i].Trim()))
            {
                newLine.Append($"\"{numbers[i].Trim()}\"],");
            }
            else
            {
                newLine.Length--;
                newLine.Append("],");
            }
        }
    }
}
