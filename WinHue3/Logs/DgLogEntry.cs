using System;

namespace WinHue3
{
    public class DgLogEntry
    {
        public DateTime evdatetime { get; set; }
        public string thread { get; set; }
        public string level { get; set; }
        public string method { get; set; }
        public string logger { get; set; }
        public string message { get; set; }
        public string line { get; set; }
        public string classname { get; set; }

        public override string ToString()
        {
            return $@"{evdatetime} {thread} {level} {logger} {method} ==> {message}";
        }
    }
}