﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Items
{
    public static class Maybers
    {
        public static class Prefs
        {
            public static string Get(string key, string defaultValue) => PlayerPrefs.GetString(key, defaultValue);
            public static int Get(string key, int defaultValue) => PlayerPrefs.GetInt(key, defaultValue);
            public static float Get(string key, float defaultValue) => PlayerPrefs.GetFloat(key, defaultValue);
            public static bool Get(string key, bool defaultValue) => defaultValue ? PlayerPrefs.GetInt(key, 1) == 1 : PlayerPrefs.GetInt(key, 0) == 1;
            public static System.DateTime Get(string key, System.DateTime defaultValue)
            {
                if (PlayerPrefs.HasKey(key))
                    return System.DateTime.ParseExact(PlayerPrefs.GetString(key,
                    defaultValue.ToString("yyyy - MM - dd HH: mm", System.Globalization.CultureInfo.InvariantCulture)),
                    "yyyy - MM - dd HH: mm", System.Globalization.CultureInfo.InvariantCulture);

                return defaultValue;
            }
            public static void Set(string key, string value) => PlayerPrefs.SetString(key, value);
            public static void Set(string key, int value) => PlayerPrefs.SetInt(key, value);
            public static void Set(string key, float value) => PlayerPrefs.SetFloat(key, value);
            public static void Set(string key, bool value)
            {
                if (value)
                    PlayerPrefs.SetInt(key, 1);
                else
                    PlayerPrefs.GetInt(key, 0);
            }
            public static void Set(string key, System.DateTime value)
            {
                PlayerPrefs.SetString(key,
                    value.ToString("yyyy - MM - dd HH: mm", System.Globalization.CultureInfo.InvariantCulture));
            }
            public static void Set(string key, List<string> list)
            {
                int ptr = 0;
                Set(key + " count", list.Count);
                foreach (var item in list)
                {
                    Set(key + " " + ptr, item);
                    ptr++;
                }
            }
            public static List<string> Get(string key, List<string> defaultValue)
            {
                int count = Get(key + " count", 0);

                if (count == 0)
                    return defaultValue;

                int ptr = 0;
                var list = new List<string>();
                for (int i = 0; i < count; i++)
                    list.Add(Get(key + " " + ptr, string.Empty));

                return list;
            }

            public static void Set(string key, List<int> list)
            {
                int ptr = 0;
                Set(key + " count", list.Count);
                foreach (var item in list)
                {
                    Set(key + " " + ptr, item);
                    ptr++;
                }
            }
            public static List<int> Get(string key, List<int> defaultValue)
            {
                int count = Get(key + " count", 0);

                if (count == 0)
                    return defaultValue;

                int ptr = 0;
                var list = new List<int>();
                for (int i = 0; i < count; i++)
                    list.Add(Get(key + " " + ptr, 0));

                return list;
            }

            public static void Increment(string key, int add, int defaultValue) => Set(key, Get(key, defaultValue) + add);
            public static void Increment(string key, int add) => Increment(key, add, 0);
            public static void Increment(string key, float add, float defaultValue) => Set(key, Get(key, defaultValue) + add);
            public static void Increment(string key, float add) => Increment(key, add, 0f);
        }
    }
}
