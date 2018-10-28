using System;
using System.ComponentModel.DataAnnotations;

namespace TodoApp.Models
{
    public class Todo : BaseEntity
    {
        [Key]
        public int todo_id {get;set;}
        public string title {get;set;}
        public string desc {get;set;}
        public DateTime due_by {get;set;}
        public int user_id {get;set;}
        public User User {get;set;}
        public Todo()
        {
            created_at = DateTime.Now;
            updated_at = DateTime.Now;
        }
    }
}