using System;

namespace StudentTree
{
    public class StringKey : IComparable<StringKey>
    {
        public string Key { get; private set; }
		
        public StringKey(string i_Key)
        {
            this.Key = i_Key;
        }

        public int CompareTo(StringKey i_OtherKey)
        {
            return string.Compare(this.Key, i_OtherKey.Key, StringComparison.Ordinal);
        }
    }
}
