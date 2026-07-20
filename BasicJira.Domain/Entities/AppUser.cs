using System;
using System.Collections.Generic;
using System.Text;

namespace BasicJira.Domain.Entities;

public class AppUser
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = string.Empty; 

    public string Email { get; set; } = string.Empty;       

    public string Role { get; set; } = string.Empty;        //rbac için

    public string PasswordHash { get; set; } = string.Empty; //db de password düz tutulmaz, şifreli algoritmalarla tutulur. yani hashlenir.  

    public DateTime CreatedAt { get; set; }

    public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
     
    public ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();


}


// NRT uyarısını önlemek ve property'sinin null olmamasını sağlamak için
// boş string ile başlatılmıştır. `string.Empty` ile `""` eşdeğerdir;
// seçim kod standardı/okunabilirlik tercihidir.