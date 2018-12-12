using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace JsonReaders
{
   public class AppSettingsTest
   {
      private IConfigurationRoot _config;

      [SetUp]
      public void Setup()
      {
         //Only works on .NET Core and .NET Framework. Not .NET Standard
         _config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
      }

      [TestCase("environmentoptions")]
      [TestCase("ApplicationInsights")]
      [TestCase("cosmosoptions")]
      [TestCase("storeoptions")]
      public void AccessAppSettingsForSingleLevel(string section)
      {
         var myDict = new Dictionary<string, string>();
         _config.GetSection(section).Bind(myDict);
         Assert.That(myDict.Count(), Is.GreaterThanOrEqualTo(1));
      }

      [TestCase("Logging:Console:LogLevel")]
      public void AccessAppSettingsForMultipleLevels(string sectionName)
      {
         var myDict = new Dictionary<string, string>();
         _config.GetSection(sectionName).Bind(myDict);
         Assert.That(myDict.Count(), Is.GreaterThanOrEqualTo(1));
      }

      [TestCase("environmentoptions", "ApplicationName", "asos-saved-items-api")]
      public void ReadDataFromAppSetting(string section, string searchKey, string expectedResult)
      {
         var myDict = new Dictionary<string, string>();
         _config.GetSection(section).Bind(myDict);
         var actual = myDict[searchKey];
         Assert.That(expectedResult, Is.EqualTo(actual));
      }

      [TestCase("MySettings:MyValues")]
      public void ReadDataFromAppSettingArray(string sectionTitle)
      {
         var myDict = new Dictionary<string, string>();
         var settingsSection = _config.GetSection(sectionTitle);
         foreach (var section in settingsSection.GetChildren())
         {
            var settingName = section.GetValue<string>("Name");
            var settingValue = section.GetValue<string>("IsActive");
            myDict.Add(settingName, settingValue);
         }

         var actualValue = myDict["SouthPole"];
         Assert.That("True", Is.EqualTo(actualValue));
         //Read https://stackoverflow.com/questions/41329108/asp-net-core-get-json-array-using-iconfiguration
      }

      [TestCase("ArrayWithSameKey")]
      public void ReadDataFromArrayWithSameKey(string sectionTitle)
      {
         var settingsSection = _config.GetSection(sectionTitle);
         var data = new List<string>();
         foreach (var section in settingsSection.GetChildren())
         {
            data.Add(section.GetValue<string>("bookingid"));
         }

         Assert.AreEqual(4, data.Count);
      }
   }
}