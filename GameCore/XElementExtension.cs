using System.Xml.Linq;

namespace GameCore
{
    public static class XElementExtension
    {
        /// <summary>
        /// Compares an XElement local name.
        /// </summary>
        /// <param name="element">The element containing the name to compare with.</param>
        /// <param name="name">The name to compare against the elements.</param>
        /// <returns>True if the elements local name is equal to the name parameter.</returns>
        public static bool CompareName(this XElement element, string name)
        {
            return element.Name.LocalName.Equals(name);
        }
    }
}
