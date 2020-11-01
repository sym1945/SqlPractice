using System;

namespace SqlPractice
{
    public class Test
    {
        [TableColumn("Id")]
        public int 아이디 { get; set; }

        [TableColumn("Name")]
        public string 이름 { get; set; }

        [TableColumn("CreateTime")]
        public DateTime? 생성시간 { get; set; }
    }
}