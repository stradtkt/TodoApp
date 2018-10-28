using System;
using System.ComponentModel.DataAnnotations;

namespace TodoApp.Models
{
    public class Todo : BaseEntity
    {
        [Key]
        public int todo_id {get;set;}
        [Required(ErrorMessage="Title is required")]
        [MinLength(3, ErrorMessage="Title has a min length of 3")]
        [MaxLength(40, ErrorMessage="Title has a max length of 40")]
        public string title {get;set;}
        [Required(ErrorMessage="Description is required")]
        [MinLength(10, ErrorMessage="Description has a min length of 10")]
        [MaxLength(1000, ErrorMessage="Description has a max length of 1000")]
        public string desc {get;set;}
        [Required(ErrorMessage="Due By is required")]
        [Display(Name="Due By")]
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