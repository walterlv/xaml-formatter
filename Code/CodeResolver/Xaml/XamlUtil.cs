namespace Cvte.CodeResolvers.Xaml
{
    /// <summary>
    /// 包含 XAML 的通用字段。
    /// </summary>
    public static class XamlUtil
    {
        public const char NamespaceSeparateChar = ':';
        public static readonly char[] NamespaceSeparateChars = {':'};
        public const string NamespaceSeparator = ":";

        public const char PropertyPathSeparateChar = '.';
        public static readonly char[] PropertyPathSeparateChars = { '.' };
        public const string PropertyPathSeparator = ".";

        public const int TabSpaceCount = 4;
    }
}