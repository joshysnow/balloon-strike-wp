using System;

namespace GameInterfaceFramework
{
    public class Credit
    {
        public string Title
        {
            get { return _title; }
        }

        public string Name
        {
            get { return _name; }
        }

        private string _title;
        private string _name;

        public Credit(string title, string name)
        {
            _title = title;
            _name = name;
        }
    }
}
