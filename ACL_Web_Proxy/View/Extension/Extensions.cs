using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ACL_Web_Proxy.View
{
    public static class Extensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> collection)
        {
            return new ObservableCollection<T>(collection);
        } 
    }
}