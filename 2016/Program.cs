using static Core.Helpers;
using System.Reflection;

Type.GetType(args[0])
    ?.GetMethod("Run", BindingFlags.Static | BindingFlags.Public)
    ?.Invoke(null, new object[] { await LoadDataAsync(args[0]) });
