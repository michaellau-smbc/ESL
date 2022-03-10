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
            string filePath = @"C:\Code\parking-enforcement-service\src\Data\EligibleStreets\EligibleStreets2021_Mike.json";
            List<string> lines = File.ReadLines(filePath).ToList();
            List<string> newData = new();

            foreach (string line in lines)
            {
                if (line.Contains("House Numbers"))
                {
                    ModifyHouseRow(newData, line);
                }
                else if (line.Contains("Zone (if applicable)"))
                {
                    ModifyZoneRow(newData, line);
                }
                else if (line.Contains("FIELD2"))
                {

                }
                else if (line.Contains("Entitlement"))
                {
                    ModifyEntitlement(newData, line);
                }
                else if (line.Contains("POSTCODE"))
                {
                    var rawData = line.Replace("\"POSTCODE\": \"", "");

                    if (rawData.Trim().StartsWith("\\t"))
                    {
                        var rawPostCode = rawData.Trim().Remove(0, 3);
                        var newString = $"    \"POSTCODE\": \"{rawPostCode.Trim()}";

                        newData.Add(newString);
                    }
                    else
                    {
                        newData.Add(line);
                    }
                }
                else
                {
                    newData.Add(line);
                }
            }

            string newFilePath = @$"C:\Code\parking-enforcement-service\src\Data\EligibleStreets\EligibleStreets{DateTime.Now:yyyyMMdd}.json";
            File.WriteAllLines(newFilePath, newData);
            Console.WriteLine("Created!");
            Console.ReadLine();
        }

        private static void ModifyEntitlement(List<string> newData, string line)
        {
            var rawEntitlementData = line.Replace("\"Entitlement\": \"", "");
            var rawEntitlementOnStreet = rawEntitlementData.Trim().Replace("On Street \",", "On Street\",");
            var newString = $"    \"Entitlement\": \"{rawEntitlementOnStreet.Trim()}";

            newData.Add(newString);
        }

        private static void ModifyHouseRow(List<string> newData, string line)
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

        private static void ModifyZoneRow(List<string> newData, string line)
        {
            var rawZoneData = line.Replace("\"Zone (if applicable)\": \"", "");

            var newString = $"    \"Zone\": \"{rawZoneData.Trim()}";

            newData.Add(newString);
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
