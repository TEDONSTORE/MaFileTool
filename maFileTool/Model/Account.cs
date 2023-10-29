using Microsoft.Build.Framework;
using Newtonsoft.Json;
using System;

namespace maFileTool.Model
{
    public class Account
    {
        [Column(1)]
        [Required]
        public string Id { get; set; }
        [Column(2)]
        [Required]
        public string Login { get; set; }
        [Column(3)]
        [Required]
        public string Password { get; set; }
        [Column(4)]
        [Required]
        public string Email { get; set; }
        [Column(5)]
        [Required]
        public string EmailPassword { get; set; }
        [Column(6)]
        [Required]
        public string Phone { get; set; }
        [Column(7)]
        [Required]
        public string RevocationCode { get; set; }
    }

    [AttributeUsage(AttributeTargets.All)]
    public class Column : System.Attribute
    {
        public int ColumnIndex { get; set; }


        public Column(int column)
        {
            ColumnIndex = column;
        }
    }
}
