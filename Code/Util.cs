using Microsoft.Xna.Framework;
using MonoGame.Extended;

using NLua;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace realms.Code
{
    public static class StringExtensions
    {
        public static string Capitalize(this string str)
        {
            if (str.Length == 0)
                return str;
            else if (str.Length == 1)
            {
                return $"{char.ToUpper(str[0])}";
            }
            else
                return char.ToUpper(str[0]) + str.Substring(1);
        }
    }

    public enum ScriptLoadState
    {
        LoadOk,
        LoadFileNotFound,
        LoadInterpretError,
    }

    public class ScriptLoadResult
    {
        public ScriptLoadState LoadState { get; set; }
        public string PackName { get; set; }
        public List<string> Values { get; set; } = new();
    }

    public static class ListExtensions
    {
        public static List<T> Shuffle<T>(this List<T> list)
        {
            var random = new Random();
            var newShuffledList = new List<T>();
            var listcCount = list.Count;
            for (int i = 0; i < listcCount; i++)
            { 
                var randomElementInList = random.Next(0, list.Count);
                newShuffledList.Add(list[randomElementInList]);
                list.Remove(list[randomElementInList]);
            }
            return newShuffledList;
        }
    }

    public static class LuaExtensions
    {
        public static ScriptLoadResult LoadScript(this Lua engine, string name)
        {
            ScriptLoadResult result = new ScriptLoadResult();
            result.PackName = name.Capitalize();
            try
            {
                var stream = TitleContainer.OpenStream($"Content/Scripts/{name}.lua");
                var code = new StreamReader(stream).ReadToEnd();
                var elements = engine.DoString(code, $"{name.Capitalize()}_chunk");
                if (elements.Length > 0)
                {
                    foreach (var el in (elements[0] as LuaTable).Values)
                    {
                        result.Values.Add(el.ToString());
                    }
                }
                stream.Dispose();
                result.LoadState = ScriptLoadState.LoadOk;
            }
            catch (System.IO.FileNotFoundException)
            {
                result.LoadState = ScriptLoadState.LoadFileNotFound;
            }

            return result;
        }
    }
}
