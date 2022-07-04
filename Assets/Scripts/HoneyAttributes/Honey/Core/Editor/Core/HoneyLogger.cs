#if UNITY_EDITOR
#nullable  enable
using System.Text;

namespace Honey.Editor
{
    public class HoneyLogger
    {
        private StringBuilder builder= new StringBuilder();

        private string temp = string.Empty;
        public string Template { get; }

        public HoneyLogger(string template="-{0}\n")
        {
            Template = template;
        }
        
        public void Log(string text)
        {
            builder.Append(string.Format(Template, text));
        }

        public void Clear()
        {
            builder.Clear();
            temp = builder.ToString();
        }

        public override string ToString()
        {
            if (temp.Length == builder.Length)
            {
                return temp;
            }

            return temp = builder.ToString();
        }
    }
}
#endif