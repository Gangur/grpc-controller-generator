namespace Infrastructure.Extensions
{
    public static class StringExtension
    {
        public static string GetServiceName(this string str)
        {
            var arr = str.Split('.');
            return arr[arr.Length - 1];
        }

        public static string GetResponseGenericTypeName(this string str)
        {
            var arr = str.Split(new char[] { '<', '>' });
            return arr[arr.Length / 2];
        }
    }
}
